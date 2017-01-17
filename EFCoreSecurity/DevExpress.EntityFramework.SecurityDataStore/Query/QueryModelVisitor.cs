using Remotion.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remotion.Linq.Clauses;
using System.Linq.Expressions;
using Remotion.Linq.Clauses.Expressions;
using DevExpress.EntityFramework.DbContextDataStore.Utility;
using System.Reflection;
using System.Collections;
using Remotion.Linq.Clauses.ResultOperators;
using Microsoft.EntityFrameworkCore.Query.ResultOperators.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using DevExpress.EntityFramework.SecurityDataStore.Utility;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public class BaseSourceExpressionCreatedEventArgs {
        public Expression Expression { get; set; }
        public Type EntityType { get; set; }
        public BaseSourceExpressionCreatedEventArgs(Expression expression, Type entityType) {
            Expression = expression;
            EntityType = entityType;
        }
    }
    public class QueryModelVisitor : QueryModelVisitorBase {
        private DbContext dbContext;
        private Type entityType;
        private Type selectorType;
        private bool flagsNotVisitSelector;
        private QueryContext queryContext;
        public Expression expression { get; set; }
        public ParameterExpression MainParamerExpression { get; set; }
        public event EventHandler<BaseSourceExpressionCreatedEventArgs> BaseSourceExpressionCreated;
        public QueryModelVisitor(DbContext dbContext, QueryContext queryContext) {
            this.dbContext = dbContext;
            this.queryContext = queryContext;
        }
        public override void VisitAdditionalFromClause(AdditionalFromClause fromClause, QueryModel queryModel, int index) {
            Type typeParameter = GetTypeParameter(fromClause.FromExpression);
            ParameterExpression parameter = Expression.Parameter(typeParameter, "p");
            Expression updateExpression = UpdateExpressionVisitor.Update(fromClause.FromExpression, new[] { parameter }, dbContext, queryContext);
            updateExpression = Expression.Convert(updateExpression, typeof(IEnumerable<>).MakeGenericType(updateExpression.Type.GetGenericArguments()));
            MethodInfo genericSelectMany = QueryableMethodsHelper.SelectMany.MakeGenericMethod(typeParameter, fromClause.ItemType);
            LambdaExpression lambda = Expression.Lambda(updateExpression, parameter);
            MethodCallExpression selectManyResult = Expression.Call(genericSelectMany, new[] { expression, lambda });
            expression = selectManyResult;
        }
        public override void VisitGroupJoinClause(GroupJoinClause groupJoinClause, QueryModel queryModel, int index) {
            JoinClause joinClause = (JoinClause)groupJoinClause.JoinClause;
            Type tOuter = queryModel.MainFromClause.ItemType;
            Type tInner = joinClause.ItemType;
            Type tOuterKey = joinClause.OuterKeySelector.Type;
            Type tInnerKey = joinClause.InnerKeySelector.Type;
            Type tIEnumerableInner = typeof(IEnumerable<>).MakeGenericType(tInner);
            Type tResult = queryModel.SelectClause.Selector.Type;
            ParameterExpression iEnumerableInnerParameter = Expression.Parameter(tIEnumerableInner, "d");
            ParameterExpression outerParameter = Expression.Parameter(tOuter, "p");
            ParameterExpression innerParameter = Expression.Parameter(tInner, "d");
            ParameterExpression outerKeyParameter = Expression.Parameter(tOuter, "p");
            Expression outerKey = UpdateExpressionVisitor.Update(joinClause.OuterKeySelector, new[] { outerKeyParameter }, dbContext, queryContext);
            LambdaExpression outerKeyLamda = Expression.Lambda(outerKey, outerKeyParameter);
            ParameterExpression tInnerKeyParameter = Expression.Parameter(tInner, "d");
            Expression innerKey = UpdateExpressionVisitor.Update(joinClause.InnerKeySelector, new[] { tInnerKeyParameter }, dbContext, queryContext);
            LambdaExpression innerKeyLamda = Expression.Lambda(innerKey, tInnerKeyParameter);
            Expression selector = UpdateExpressionVisitor.Update(queryModel.SelectClause.Selector, new[] { outerParameter, iEnumerableInnerParameter }, dbContext, queryContext);
            LambdaExpression selectorLambda = Expression.Lambda(selector, new[] { outerParameter, iEnumerableInnerParameter });
            MethodInfo groupJoin = GetMethods("GroupJoin", expression.Type, 4).Where(p =>
                    p.GetParameters().Count() == 5).Single().
                MakeGenericMethod(new Type[] { tOuter, tInner, tOuterKey, tResult });
            expression = Expression.Call(groupJoin, new[] { expression, joinClause.InnerSequence, outerKeyLamda, innerKeyLamda, selectorLambda });
            flagsNotVisitSelector = true;
        }
        public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, GroupJoinClause groupJoinClause) {
            Type tOuter = queryModel.MainFromClause.ItemType;
            Type tInner = joinClause.ItemType;
            Type tOuterKey = joinClause.OuterKeySelector.Type;
            Type tInnerKey = joinClause.InnerKeySelector.Type;
            ParameterExpression outerParameter = Expression.Parameter(tOuter, "p");
            ParameterExpression innerParameter = Expression.Parameter(tInner, "d");
            ParameterExpression outerKeyParameter = Expression.Parameter(tOuter, "p");
            Expression outerKey = UpdateExpressionVisitor.Update(joinClause.OuterKeySelector, new[] { outerKeyParameter }, dbContext, queryContext);
            LambdaExpression outerKeyLamda = Expression.Lambda(outerKey, outerKeyParameter);
            ParameterExpression tInnerKeyParameter = Expression.Parameter(tInner, "d");
            Expression innerKey = UpdateExpressionVisitor.Update(joinClause.InnerKeySelector, new[] { tInnerKeyParameter }, dbContext, queryContext);
            LambdaExpression innerKeyLamda = Expression.Lambda(innerKey, tInnerKeyParameter);
            Type tResult = queryModel.SelectClause.Selector.Type;
            Expression selector = UpdateExpressionVisitor.Update(queryModel.SelectClause.Selector, new[] { outerParameter, innerParameter }, dbContext, queryContext);
            LambdaExpression selectorLambda = Expression.Lambda(selector, new[] { outerParameter, innerParameter });
            MethodInfo join = GetMethods("Join", expression.Type, 4).Where(p =>
                   p.GetParameters().Count() == 5).Single().
               MakeGenericMethod(new Type[] { tOuter, tInner, tOuterKey, tInnerKey });
            expression = Expression.Call(join, new[] { expression, joinClause.InnerSequence, outerKeyLamda, innerKeyLamda, selectorLambda });
            flagsNotVisitSelector = true;
        }
        public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, int index) {
            Type tOuter = queryModel.MainFromClause.ItemType;
            Type tInner = joinClause.ItemType;
            Type tOuterKey = joinClause.OuterKeySelector.Type;
            Type tInnerKey = joinClause.InnerKeySelector.Type;
            ParameterExpression outerParameter = Expression.Parameter(tOuter, "p");
            ParameterExpression innerParameter = Expression.Parameter(tInner, "d");
            ParameterExpression outerKeyParameter = Expression.Parameter(tOuter, "p");
            Expression outerKey = UpdateExpressionVisitor.Update(joinClause.OuterKeySelector, new[] { outerKeyParameter }, dbContext, queryContext);
            LambdaExpression outerKeyLamda = Expression.Lambda(outerKey, outerKeyParameter);
            ParameterExpression tInnerKeyParameter = Expression.Parameter(tInner, "d");
            Expression innerKey = UpdateExpressionVisitor.Update(joinClause.InnerKeySelector, new[] { tInnerKeyParameter }, dbContext, queryContext);
            LambdaExpression innerKeyLamda = Expression.Lambda(innerKey, tInnerKeyParameter);
            Type tResult = queryModel.SelectClause.Selector.Type;
            Expression selector = UpdateExpressionVisitor.Update(queryModel.SelectClause.Selector, new[] { outerParameter, innerParameter }, dbContext, queryContext);
            LambdaExpression selectorLambda = Expression.Lambda(selector, new[] { outerParameter, innerParameter });
            MethodInfo join = GetMethods("Join", expression.Type, 4).Where(p =>
                   p.GetParameters().Count() == 5).Single().
               MakeGenericMethod(new Type[] { tOuter, tInner, tOuterKey, tInnerKey });
            expression = Expression.Call(join, new[] { expression, joinClause.InnerSequence, outerKeyLamda, innerKeyLamda, selectorLambda });
            flagsNotVisitSelector = true;
        }

        public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel) {
            bool processed = false;
            expression = queryModel.MainFromClause.FromExpression;
            if(expression is SubQueryExpression) {
                QueryModel subQueryModel = ((SubQueryExpression)expression).QueryModel;
                VisitQueryModel(subQueryModel);
                processed = true;
            }
            if(expression is ConstantExpression) {
                expression = Utility.DbContextHelper.GetDbSet(dbContext, (ConstantExpression)expression);
                if(BaseSourceExpressionCreated != null) {
                    BaseSourceExpressionCreatedEventArgs eventArgs = new BaseSourceExpressionCreatedEventArgs(expression, queryModel.MainFromClause.ItemType);
                    BaseSourceExpressionCreated(this, eventArgs);
                    expression = eventArgs.Expression;
                }
                processed = true;
            }
            if(expression is MemberExpression) {
                QuerySourceReferenceExpression querySourceReferenceExpression = (expression as MemberExpression)?.Expression as QuerySourceReferenceExpression;
                if(querySourceReferenceExpression != null) {
                    Type paramType = querySourceReferenceExpression.ReferencedQuerySource.ItemType;
                    string paramName = querySourceReferenceExpression.ReferencedQuerySource.ItemName;
                    MainParamerExpression = Expression.Parameter(paramType, paramName);
                    expression = UpdateExpressionVisitor.Update(expression, new[] { MainParamerExpression }, dbContext, queryContext);
                    processed = true;
                }
            }
            if(expression is ParameterExpression) {
                processed = true; //TODO?
            }
            if(!processed) {
                throw new NotImplementedException();
            }
            entityType = queryModel.MainFromClause.ItemType;
            selectorType = queryModel.SelectClause.Selector.Type;
        }

        //public override void VisitOrderByClause(OrderByClause orderByClause, QueryModel queryModel, int index) {
        //    throw new NotSupportedException();
        //}

        public override void VisitOrdering(Ordering ordering, QueryModel queryModel, OrderByClause orderByClause, int index) {
            ParameterExpression expressionParam = Expression.Parameter(entityType, "p");
            Expression updateExpression = UpdateExpressionVisitor.Update(ordering.Expression, new[] { expressionParam }, dbContext, queryContext);
            LambdaExpression expressionKeyLambda = Expression.Lambda(updateExpression, expressionParam);
            if(ordering.OrderingDirection == OrderingDirection.Asc) {
                MethodInfo orderBy = GetMethod("OrderBy", expression.Type, 1).MakeGenericMethod(entityType, ordering.Expression.Type);
                expression = Expression.Call(orderBy, new[] { expression, expressionKeyLambda });
            }
            else {
                MethodInfo orderByDescending = GetMethod("OrderByDescending", expression.Type, 1).MakeGenericMethod(entityType, ordering.Expression.Type);
                expression = Expression.Call(orderByDescending, new[] { expression, expressionKeyLambda });
            }
        }
        Type resultOptionType;
        public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index) {
            resultOptionType = resultOptionType ?? queryModel.SelectClause.Selector.Type;
            if(resultOperator is CastResultOperator) {
                CastResultOperator castResultOperator = (CastResultOperator)resultOperator;
                Type castType = castResultOperator.CastItemType;
                MethodInfo cast = GetMethod("Cast", expression.Type).MakeGenericMethod(castType);
                resultOptionType = castType;
                expression = Expression.Call(cast, expression);
                return;
            }
            if(resultOperator is AllResultOperator) {
                AllResultOperator allResultOperator = (AllResultOperator)resultOperator;
                ParameterExpression parameterExpression = Expression.Parameter(selectorType, "p");
                Expression predicateUpdate = UpdateExpressionVisitor.Update(allResultOperator.Predicate, new[] { parameterExpression }, dbContext, queryContext);
                Expression allLamda = Expression.Lambda(predicateUpdate, parameterExpression);
                MethodInfo all = GetMethod("All", expression.Type, 1).MakeGenericMethod(selectorType);
                expression = Expression.Call(all, new[] { expression, allLamda });
                return;
            }
            if(resultOperator is FirstResultOperator) {
                FirstResultOperator firstResultOperator = (FirstResultOperator)resultOperator;
                if(firstResultOperator.ReturnDefaultWhenEmpty) {
                    MethodInfo firstOrDefault = GetMethod("FirstOrDefault", expression.Type, 0).MakeGenericMethod(selectorType);
                    expression = Expression.Call(firstOrDefault, new[] { expression });
                }
                else {
                    MethodInfo first = GetMethod("First", expression.Type, 0).MakeGenericMethod(selectorType);
                    expression = Expression.Call(first, new[] { expression });
                }
                return;
            }
            if(resultOperator is SingleResultOperator) {
                SingleResultOperator singleResultOperator = (SingleResultOperator)resultOperator;
                if(singleResultOperator.ReturnDefaultWhenEmpty) {
                    MethodInfo firstOrDefault = GetMethod("SingleOrDefault", expression.Type, 0).MakeGenericMethod(selectorType);
                    expression = Expression.Call(firstOrDefault, new[] { expression });
                }
                else {
                    MethodInfo first = GetMethod("Single", expression.Type, 0).MakeGenericMethod(selectorType);
                    expression = Expression.Call(first, new[] { expression });
                }
                return;
            }
            if(resultOperator is AnyResultOperator) {
                MethodInfo any = GetMethod("Any", expression.Type).MakeGenericMethod(selectorType);
                expression = Expression.Call(any, new[] { expression });
                return;
            }
            if(resultOperator is TrackingResultOperator) {
                MethodInfo asNoTracking = typeof(EntityFrameworkQueryableExtensions).GetMethod("AsNoTracking").MakeGenericMethod(selectorType);
                expression = Expression.Call(asNoTracking, new[] { expression });
                return;
            }
            if(resultOperator is CountResultOperator) {
                MethodInfo count = GetMethod("Count", expression.Type).MakeGenericMethod(selectorType);
                expression = Expression.Call(count, new[] { expression });
                return;
            }
            if(resultOperator is AverageResultOperator) {
                MethodInfo average = GetAgregateMethod("Average", expression.Type, selectorType);
                expression = Expression.Call(average, new[] { expression });
                return;
            }
            if(resultOperator is ContainsResultOperator) {
                ContainsResultOperator containsResultOperator = (ContainsResultOperator)resultOperator;
                Expression valExp;
                object value;
                ParameterExpression paramExp = containsResultOperator.Item as ParameterExpression;
                if(paramExp != null &&
                    queryContext.ParameterValues.TryGetValue(paramExp.Name, out value)) {
                    valExp = Expression.Constant(value);
                }
                else {
                    valExp = containsResultOperator.Item;
                }
                if(containsResultOperator.Item is SubQueryExpression) {
                    SubQueryExpression subQueryExpression = (SubQueryExpression)containsResultOperator.Item;
                    QueryModelVisitor queryModelVisitor = new QueryModelVisitor(dbContext, queryContext);
                    queryModelVisitor.VisitQueryModel(subQueryExpression.QueryModel);
                    valExp = queryModelVisitor.expression;
                }

                MethodInfo contains = GetMethod("Contains", expression.Type, 1).MakeGenericMethod(selectorType);
                expression = Expression.Call(contains, new[] { expression, valExp });
                return;
            }
            if(resultOperator is DefaultIfEmptyResultOperator) {
                DefaultIfEmptyResultOperator defaultIfEmptyResultOperator = (DefaultIfEmptyResultOperator)resultOperator;
                MethodInfo defaultIfEmpty;
                if(defaultIfEmptyResultOperator.OptionalDefaultValue != null) {
                    defaultIfEmpty = GetMethod("DefaultIfEmpty", expression.Type, 1).MakeGenericMethod(selectorType);
                    expression = Expression.Call(defaultIfEmpty, new[] { expression, defaultIfEmptyResultOperator.OptionalDefaultValue });
                }
                defaultIfEmpty = GetMethod("DefaultIfEmpty", expression.Type).MakeGenericMethod(selectorType);
                expression = Expression.Call(defaultIfEmpty, expression);
                return;
            }
            if(resultOperator is DistinctResultOperator) {
                MethodInfo distinct = GetMethod("Distinct", expression.Type).MakeGenericMethod(selectorType);
                expression = Expression.Call(distinct, expression);
                return;
            }
            if(resultOperator is GroupResultOperator) {
                GroupResultOperator groupResultOperator = (GroupResultOperator)resultOperator;
                Type keySelectorType = GetTypeParameter(groupResultOperator.KeySelector);
                ParameterExpression keyExpressionParam = Expression.Parameter(keySelectorType, "p");
                Expression keyExpression = UpdateExpressionVisitor.Update(groupResultOperator.KeySelector, new[] { keyExpressionParam }, dbContext, queryContext);
                LambdaExpression keyLambdaExpression = Expression.Lambda(keyExpression, keyExpressionParam);
                Type elementSelectorType = GetTypeParameter(groupResultOperator.ElementSelector);
                ParameterExpression ElementExpressionParam = Expression.Parameter(elementSelectorType, "p");
                Expression ElementExpression = UpdateExpressionVisitor.Update(groupResultOperator.ElementSelector, new[] { ElementExpressionParam }, dbContext, queryContext);
                LambdaExpression ElementLambdaExpression = Expression.Lambda(ElementExpression, ElementExpressionParam);
                Type tSource = queryModel.MainFromClause.ItemType;
                Type tKey = keyExpression.Type;
                Type tElement = ElementExpression.Type;
                Type tResult = queryModel.ResultTypeOverride;
                MethodInfo groupBy = GetMethods("GroupBy", expression.Type, 2).Where(p => p.GetParameters()[2].Name == "elementSelector").Single().
                    MakeGenericMethod(tSource, tKey, tElement);
                expression = Expression.Call(groupBy, new[] { expression, keyLambdaExpression, ElementLambdaExpression });
                return;
            }
            if(resultOperator is LastResultOperator) {
                LastResultOperator lastResultOperator = (LastResultOperator)resultOperator;
                if(lastResultOperator.ReturnDefaultWhenEmpty) {
                    MethodInfo lastOrDefault = GetMethod("LastOrDefault", expression.Type, 0).MakeGenericMethod(selectorType);
                    expression = Expression.Call(lastOrDefault, new[] { expression });
                }
                else {
                    MethodInfo last = GetMethod("Last", expression.Type, 0).MakeGenericMethod(selectorType);
                    expression = Expression.Call(last, new[] { expression });
                }
                return;
            }
            if(resultOperator is LongCountResultOperator) {
                MethodInfo longCount = GetMethod("LongCount", expression.Type).MakeGenericMethod(selectorType);
                expression = Expression.Call(longCount, new[] { expression });
                return;
            }
            if(resultOperator is MaxResultOperator) {
                MethodInfo max = GetMethod("Max", expression.Type).MakeGenericMethod(selectorType);
                expression = Expression.Call(max, expression);
                return;
            }
            if(resultOperator is MinResultOperator) {
                MethodInfo min = GetMethod("Min", expression.Type).MakeGenericMethod(selectorType);
                expression = Expression.Call(min, expression);
                return;
            }
            if(resultOperator is SumResultOperator) {
                MethodInfo sum = GetAgregateMethod("Sum", expression.Type, selectorType);
                expression = Expression.Call(sum, expression);
                return;
            }
            if(resultOperator is SkipResultOperator) {
                SkipResultOperator skipResultOperator = (SkipResultOperator)resultOperator;
                Expression expVal;
                object value;
                ParameterExpression paramExp = skipResultOperator.Count as ParameterExpression;
                if(paramExp != null &&
                   queryContext.ParameterValues.TryGetValue(paramExp.Name, out value)) {
                    expVal = Expression.Constant(value);
                }
                else {
                    expVal = skipResultOperator.Count;
                }
                MethodInfo skip = GetMethod("Skip", expression.Type, 1).MakeGenericMethod(selectorType);
                expression = Expression.Call(skip, new[] { expression, expVal });
                return;
            }
            if(resultOperator is TakeResultOperator) {
                TakeResultOperator takeResultOperator = (TakeResultOperator)resultOperator;
                Expression expVal;
                object value;
                ParameterExpression paramExp = takeResultOperator.Count as ParameterExpression;
                if(paramExp != null &&
                   queryContext.ParameterValues.TryGetValue(paramExp.Name, out value)) {
                    expVal = Expression.Constant(value);
                }
                else {
                    expVal = takeResultOperator.Count;
                }
                MethodInfo take = GetMethod("Take", expression.Type, 1).MakeGenericMethod(selectorType);
                expression = Expression.Call(take, new[] { expression, expVal });
                return;
            }
            if(resultOperator is IncludeResultOperator) {
                IncludeResultOperator includeResultOperator = (IncludeResultOperator)resultOperator;
                Expression includeExpression = includeResultOperator.NavigationPropertyPath;
                Type paramExpressionType = null;
                ParameterExpression parameterExpression = null;
                if(includeExpression is MemberExpression) {
                    MemberExpression memberExpression = (MemberExpression)includeExpression;
                    paramExpressionType = memberExpression.Expression.Type;
                    parameterExpression = Expression.Parameter(paramExpressionType, "p");
                    includeExpression = Expression.Property(parameterExpression, memberExpression.Member.Name);
                }
                else {
                    paramExpressionType = GetTypeParameter(includeExpression);
                    parameterExpression = Expression.Parameter(paramExpressionType, "p");
                }
                Expression updateOuterExpression = UpdateExpressionVisitor.Update(includeExpression, new[] { parameterExpression }, dbContext, queryContext);
                LambdaExpression lambdaIncludeExpression = Expression.Lambda(updateOuterExpression, parameterExpression);
                MethodInfo include = typeof(EntityFrameworkQueryableExtensions).GetMethods().First(m => m.Name == "Include").MakeGenericMethod(selectorType, updateOuterExpression.Type);
                expression = Expression.Call(include, new[] { expression, lambdaIncludeExpression });

                if(includeResultOperator.ChainedNavigationProperties != null) {
                    foreach(PropertyInfo propertyInfo in includeResultOperator.ChainedNavigationProperties) {
                        Type propertyType = propertyInfo.PropertyType;
                        Type argument = expression.Type.GetGenericArguments().Last();
                        MethodInfo thenInclude;
                        Type realType;
                        if(typeof(IEnumerable).IsAssignableFrom(argument)) {
                            realType = argument.GetGenericArguments().First();
                            thenInclude = ThenIncludeCollection.MakeGenericMethod(includeResultOperator.QuerySource.ItemType, realType, propertyType);
                        }
                        else {
                            realType = argument;
                            thenInclude = ThenIncludeProperty.MakeGenericMethod(includeResultOperator.QuerySource.ItemType, realType, propertyType);
                        }
                        ParameterExpression parameterThenIncludeExpression = Expression.Parameter(realType, "p");
                        MemberExpression property = Expression.Property(parameterThenIncludeExpression, propertyInfo);
                        LambdaExpression lambdaThenIncludeExpression = Expression.Lambda(property, parameterThenIncludeExpression);
                        expression = Expression.Call(thenInclude, new[] { expression, lambdaThenIncludeExpression });
                    }
                }
                return;
            }
            if(resultOperator is OfTypeResultOperator) {
                OfTypeResultOperator ofTypeResultOperator = (OfTypeResultOperator)resultOperator;
                selectorType = ofTypeResultOperator.SearchedItemType;
                var miOfType = GetMethod("OfType", expression.Type).MakeGenericMethod(ofTypeResultOperator.SearchedItemType);
                expression = Expression.Call(miOfType, new[] { expression });
                return;
            }
            throw new NotSupportedException();
        }
        public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel) {
            var genericTypeDefinition = expression.Type.GetGenericArguments();
            if(flagsNotVisitSelector || (genericTypeDefinition.Count() == 1 && genericTypeDefinition.First().Equals(selectClause.Selector.Type))) {
                return;
            }

            Type sourceType = genericTypeDefinition.Count() == 1 ? genericTypeDefinition.First() : resultOptionType;

            MethodInfo selectMethodInfo = typeof(IQueryable).IsAssignableFrom(expression.Type)
                ? QueryableMethodsHelper.Select
                : EnumerableMethodsHelper.Select;

            MemberExpression memberExpression = selectClause.Selector as MemberExpression;
            if(memberExpression != null) {
                var parameter = Expression.Parameter(memberExpression.Expression.Type, "p");
                memberExpression = (MemberExpression)UpdateExpressionVisitor.Update(memberExpression, new[] { parameter }, dbContext, queryContext);
                var lambda = Expression.Lambda(memberExpression, parameter);
                var genericMethodSelect = selectMethodInfo.MakeGenericMethod(memberExpression.Expression.Type, memberExpression.Type);
                var call = Expression.Call(genericMethodSelect, expression, lambda);
                expression = call;
                return;
            }
            NewExpression newExpression = selectClause.Selector as NewExpression;
            if(newExpression != null) {
                if(sourceType != null) {
                    var parameter = Expression.Parameter(sourceType, "p");
                    List<Expression> updateArgExp = new List<Expression>();
                    foreach(var argument in newExpression.Arguments) {
                        var update = UpdateExpressionVisitor.Update(argument, new[] { parameter }, dbContext, queryContext);
                        updateArgExp.Add(update);
                    }
                    Expression newResult = Expression.New(newExpression.Constructor, updateArgExp);
                    var selectGenericMethod = selectMethodInfo.MakeGenericMethod(sourceType, newResult.Type);
                    var lambda = Expression.Lambda(newResult, parameter);
                    var call = Expression.Call(selectGenericMethod, expression, lambda);
                    expression = call;
                    return;
                }
            }
            if(sourceType != null) {
                var parameter = Expression.Parameter(sourceType, "p");
                var update = UpdateExpressionVisitor.Update(selectClause.Selector, new[] { parameter }, dbContext, queryContext);
                var selectGenericMethod = selectMethodInfo.MakeGenericMethod(sourceType, selectClause.Selector.Type);
                var lambda = Expression.Lambda(update, parameter);
                var call = Expression.Call(selectGenericMethod, expression, lambda);
                expression = call;
                return;
            }
            throw new NotImplementedException();
        }
        public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index) {
            ParameterExpression parameterExpression = Expression.Parameter(entityType, "p");
            UpdateExpressionVisitor updateExpressionVisitor = new UpdateExpressionVisitor(new[] { parameterExpression }, dbContext, queryContext);
            Expression predicateUpdate = updateExpressionVisitor.Visit(whereClause.Predicate);
            if(updateExpressionVisitor.MainParametrExpression != null)
                parameterExpression = updateExpressionVisitor.MainParametrExpression; ;

            Expression whereLamda = Expression.Lambda(predicateUpdate, parameterExpression);
            MethodInfo where = GetMethods("Where", expression.Type, 1).First().MakeGenericMethod(entityType);
            expression = Expression.Call(where, new[] { expression, whereLamda });
        }
        public static IEnumerable<MethodInfo> GetMethods(string name, Type expressionType, int parameterCount = 0) {
            if(expressionType == null) {
                return typeof(Queryable).GetTypeInfo().GetDeclaredMethods(name)
                   .Where(mi => mi.GetParameters().Length == parameterCount + 1);
            }
            if(typeof(IQueryable).IsAssignableFrom(expressionType)) {
                return typeof(Queryable).GetTypeInfo().GetDeclaredMethods(name)
                    .Where(mi => mi.GetParameters().Length == parameterCount + 1);
            }
            if(typeof(IEnumerable).IsAssignableFrom(expressionType)) {
                return typeof(Enumerable).GetTypeInfo().GetDeclaredMethods(name)
                  .Where(mi => mi.GetParameters().Length == parameterCount + 1);
            }
            return typeof(Queryable).GetTypeInfo().GetDeclaredMethods(name)
                 .Where(mi => mi.GetParameters().Length == parameterCount + 1);
        }
        private static MethodInfo GetMethod(string name, Type expressionType, int parameterCount = 0) {
            return GetMethods(name, expressionType, parameterCount).Single();
        }
        private static MethodInfo GetAgregateMethod(string name, Type expressionType, Type selectorType, int parameterCount = 0) {
            var methodsInfo = GetMethods(name, expressionType, parameterCount);
            if(expressionType == null || typeof(IQueryable).IsAssignableFrom(expressionType)) {
                return methodsInfo.Where(p =>
                    p.GetParameters()[0].ParameterType == typeof(IQueryable<>).MakeGenericType(selectorType)).Single();
            }
            else {
                if(typeof(IEnumerable).IsAssignableFrom(expressionType)) {
                    return methodsInfo.Where(p =>
                            p.GetParameters()[0].ParameterType == typeof(Enumerable).MakeGenericType(selectorType)).Single();
                }
            }
            throw new ArgumentException();
        }
        private Type GetTypeParameter(Expression outerExpression) {
            if(outerExpression is MemberExpression) {
                MemberExpression memberExpression = (MemberExpression)outerExpression;
                return GetTypeParameter(memberExpression.Expression);
            }
            if(outerExpression is MethodCallExpression) {
                MethodCallExpression methodCallExpression = (MethodCallExpression)outerExpression;
                return GetTypeParameter(methodCallExpression.Arguments.First());
            }
            return outerExpression.Type;
        }
        private static MethodInfo thenIncludeCollection;
        private static MethodInfo ThenIncludeCollection {
            get {
                if(thenIncludeCollection == null) {
                    thenIncludeCollection = typeof(EntityFrameworkQueryableExtensions).GetMethods().Where(p => p.Name == "ThenInclude").First(p => typeof(IEnumerable).IsAssignableFrom(p.GetParameters().First().ParameterType.GetGenericArguments().Last()));
                }
                return thenIncludeCollection;
            }
        }
        private static MethodInfo thenIncludeProperty;
        private static MethodInfo ThenIncludeProperty {
            get {
                if(thenIncludeProperty == null) {
                    thenIncludeProperty = typeof(EntityFrameworkQueryableExtensions).GetMethods().Where(p => p.Name == "ThenInclude").First(p => !typeof(IEnumerable).IsAssignableFrom(p.GetParameters().First().ParameterType.GetGenericArguments().Last()));
                }
                return thenIncludeProperty;
            }
        }
    }
}
