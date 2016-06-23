using DevExpress.EntityFramework.SecurityDataStore.Utility;
using Remotion.Linq;
using Remotion.Linq.Clauses.Expressions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.ResultOperators.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public class SecurityQueryExecutor {

        private DbContext realDbContext;
        private BaseSecurityDbContext dbContextSecurity;
        private IStateManager nativeStateManager;
        private QueryModel queryModel;
        private QueryContext queryContext;
        private NeedSaveFlags isNeedSaveFlags;
        private StateManager securityStateManager;
        private object lockObject = new object();
        private enum NeedSaveFlags {
            None = 0,
            TypeFlags = 1,
            StartChangeTrackingFlags = 2,
            isGroupingFlags = 4
        }
        private void InitSecurity(QueryModelVisitor queryModelVisitor) {
            queryModelVisitor.BaseSourceExpressionCreated += QueryModelVisitorBaseSourceExpressionCreated;
        }

        private void QueryModelVisitorBaseSourceExpressionCreated(object sender, BaseSourceExpressionCreatedEventArgs e) {
            e.Expression = dbContextSecurity.Security.SecurityExpressionBuilder.GetDatabaseReadExpressionFromSecurity(e.Expression, e.EntityType);
        }

        private object GetQueryResult() {
            QueryModelVisitor queryModelVisitor = new QueryModelVisitor(realDbContext, queryContext);
            InitSecurity(queryModelVisitor);
            queryModelVisitor.VisitQueryModel(queryModel);
            Expression expressionQuery = queryModelVisitor.expression;
            Type returnType = expressionQuery.Type;
            MethodInfo getQueryResultHelper = typeof(SecurityQueryExecutor).GetTypeInfo().GetDeclaredMethod("GetQueryResultHelper").MakeGenericMethod(returnType);
            object result;
            try {
                result = getQueryResultHelper.Invoke(this, new[] { expressionQuery });
            }
            catch(Exception e) {
                throw e.InnerException;
            }
            return result;
        }
        private void StopInNativeInNativeChangeTracking() {
            List<InternalEntityEntry> entityEntrys = nativeStateManager.Entries.ToList();
            foreach(InternalEntityEntry targetInternalEntityEntry in entityEntrys) {
                nativeStateManager.StopTracking(targetInternalEntityEntry);
            }
        }
        private object GetQueryResultHelper<TResult>(Expression expressionQuery) {
            return Expression.Lambda<Func<TResult>>(expressionQuery, new ParameterExpression[] { }).Compile()();
        }
        private bool IsNeedSaveFlags(Type result) {
            Type entityType = queryModel.MainFromClause.ItemType;
            if(entityType != result) {
                isNeedSaveFlags |= NeedSaveFlags.TypeFlags;
            }
            if(GetAsNoTrackingFlags()) {
                isNeedSaveFlags |= NeedSaveFlags.StartChangeTrackingFlags;
            }
            if((entityType.IsGenericType && entityType.GetGenericTypeDefinition() == typeof(IGrouping<,>))) {
                isNeedSaveFlags |= NeedSaveFlags.isGroupingFlags;
            }
            return isNeedSaveFlags > 0;
        }
        private bool GetAsNoTrackingFlags() {
            return queryModel.ResultOperators.Any(p => p is TrackingResultOperator);
        }
        private IEnumerable<TResult> ReturnImmediately<TResult>(object resultQueryObject) {
            if(isNeedSaveFlags == NeedSaveFlags.StartChangeTrackingFlags) {
                IEnumerable<TResult> result = ConverterHelper.ResultToIEnumerableResult<TResult>(resultQueryObject);              
            }
            if(resultQueryObject is IEnumerable<TResult>) {
                return (IEnumerable<TResult>)resultQueryObject;
            }
            return new TResult[] { (TResult)resultQueryObject };
        }
        public static object GetDefault(Type type) {
            if(type.IsValueType) {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        private IEnumerable<TResult> ProcessingAndReturnResult<TResult>(object resultQuery) {
            IEnumerable<TResult> result = ConverterHelper.ResultToIEnumerableResult<TResult>(resultQuery);
            foreach(var resultObject in result) {
                StartTrakingEntity(resultObject);
                yield return resultObject;
            }
            navigationObject.Clear();
        }
        private void StartTrakingEntity(object resultQueryObject) {
            InternalEntityEntry internalEntityEntry = securityStateManager.GetOrCreateEntry(resultQueryObject);
            NavigationPropertyProcessing(internalEntityEntry.EntityType.GetNavigations(), resultQueryObject);
            InternalEntityEntry startEntityEntry;
            securityStateManager.StopTracking(internalEntityEntry);
            startEntityEntry = securityStateManager.StartTracking(internalEntityEntry);
            startEntityEntry.SetEntityState(EntityState.Unchanged);
        }
        private EntityEntry TryGetEntityEntry(object entity, DbContext dbContext) {
            IEnumerable<EntityEntry> entriesChangeTraking = DbContextHelper.GetEntriesInChangeTraking(dbContext, entity.GetType());
            EntityEntry entityEntry = entriesChangeTraking.Where(p => p.Entity == (object)entity).FirstOrDefault();
            return entityEntry;
        }
        private List<object> navigationObject = new List<object>();
        private void NavigationPropertyProcessing(IEnumerable<INavigation> Navigations, object resultQueryObject) {
            foreach(var navigationProperty in Navigations) {
                PropertyInfo navigationPropertyInfo = resultQueryObject.GetType().GetProperty(navigationProperty.Name);
                object navigationEntity = navigationPropertyInfo.GetValue(resultQueryObject);
                if(navigationObject.Any(p => p == navigationEntity)) {
                    continue;
                }
                navigationObject.Add(navigationEntity);
                if(navigationEntity != null) {
                    if(navigationEntity is IEnumerable) {
                        foreach(var Entity in ((IEnumerable)navigationEntity)) {
                            StartTrakingEntity(Entity);
                        }
                    }
                    else {
                        StartTrakingEntity(navigationEntity);
                    }
                }
            }
        }

        public SecurityQueryExecutor(QueryModel queryModel) {
            this.queryModel = queryModel;
        }
        public IEnumerable<TResult> Execute<TResult>(QueryContext queryContext) {
            lock (lockObject) {
                realDbContext = ((SecurityQueryContext)queryContext).dbContext.RealDbContext;
                nativeStateManager = realDbContext.GetService<IStateManager>();
                dbContextSecurity = ((SecurityQueryContext)queryContext).dbContext;
                securityStateManager = (StateManager)queryContext.StateManager.Value;
                this.queryContext = queryContext;               
                object resultQuery = GetQueryResult();
                if(resultQuery == null) {
                    return new TResult[] { default(TResult) };
                }

                //Security Block                

                List<object> resultList = ConverterHelper.ResultToIEnumerableResult<TResult>(resultQuery).OfType<object>().ToList();
                IEnumerable<object> result = dbContextSecurity.Security./*SecurityServicesProvider.*/SecurityProcessLoadObjects.ProcessObjects(resultList);

                IEnumerable<object> allEntity = dbContextSecurity.Model.GetAllLinkedObjects(result);
                //end

                if(IsNeedSaveFlags(typeof(TResult))) {
                    return result.OfType<TResult>();
                }
                StartInChangeTracking(allEntity);
                return result.OfType<TResult>();
            }
        }

        private void StartInChangeTracking(IEnumerable<object> result) {
            foreach(object targetObject in result) {
                InternalEntityEntry entityEntry = securityStateManager.GetOrCreateEntry(targetObject);
                if(entityEntry != null) {
                    if(securityStateManager.Entries.Any(p => p == entityEntry)) {
                        securityStateManager.StopTracking(entityEntry);
                    }
                    securityStateManager.StartTracking(entityEntry);
                    entityEntry.SetEntityState(EntityState.Unchanged);
                }
            }
        }    
    }
}