using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public static class DbContextExtensions {
        private static MethodInfo getObjectGenericMethodInfo;
        public static TEntity GetObject<TEntity>(this DbContext dbContext, TEntity targetObject) where TEntity : class {
            EntityEntry entityEntry = dbContext.Entry(targetObject);
            TEntity realObject = null;
            if(entityEntry != null) {
                if(getObjectGenericMethodInfo == null) {
                    getObjectGenericMethodInfo = typeof(DbContextExtensions).GetRuntimeMethods().First(p => p.Name == "GetObjectGeneric");
                }
                IEntityType entityType = entityEntry.Metadata;
                Type targetType = entityType.ClrType;
                MethodInfo genericMethodGetObject = getObjectGenericMethodInfo.MakeGenericMethod(targetType);
                realObject = (TEntity)genericMethodGetObject.Invoke(null, new object[] { dbContext, entityEntry });
            }
            return realObject;
        }
        public static T GetRealDbContext<T>(this T dbContext) where T : SecurityDbContext {
            T result = null;
            if(dbContext.Security != null) {
                result = (T)dbContext.RealDbContext;
            }
            return result;
        }
        private static object GetObjectGeneric<TEntity>(DbContext dbContext, EntityEntry entityEntry) where TEntity : class {
            IEntityType entityType = entityEntry.Metadata;
            Type targetType = entityType.ClrType;
            TEntity realObject;
            IEnumerable<PropertyInfo> propertiesInfo = targetType.GetRuntimeProperties();
            IKey primaryKey = entityType.FindPrimaryKey();
            BinaryExpression expression = null;
            ParameterExpression parameter = Expression.Parameter(targetType, "p");
            foreach(IProperty property in primaryKey.Properties) {
                string propertyName = property.Name;
                PropertyEntry propertyEntry = entityEntry.Property(propertyName);
                BinaryExpression currentExpression = GetExpressionForKey(property, propertyEntry.CurrentValue, targetType, parameter);
                if(expression == null) {
                    expression = currentExpression;
                }
                else {
                    expression = Expression.And(expression, currentExpression);
                }
            }
            Expression<Func<TEntity, bool>> findExpression = Expression.Lambda<Func<TEntity, bool>>(expression, parameter);
            // realObject = dbContext.Set<TEntity>().Where(findExpression).FirstOrDefault();
            realObject = dbContext.Set<TEntity>().Where(findExpression).First();
            return realObject;
        }
        private static BinaryExpression GetExpressionForKey(IProperty property, object currentValueKey, Type targetType, ParameterExpression parameter) {
            string keyName = property.Name;
            BinaryExpression binaryExpression = Expression.Equal(Expression.Property(parameter, property.Name), Expression.Constant(currentValueKey));
            return binaryExpression;
        }


    }
}
