using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace DevExpress.EntityFramework.SecurityDataStore.Utility {
    public class DbContextHelper {
        public static object GetDbSet(DbContext dbContext, Type entityType) {
            MethodInfo methodInfoDbSet = dbContext.GetType().GetMethod("Set").MakeGenericMethod(entityType);
            return methodInfoDbSet.Invoke(dbContext, new object[] { });
        }
        public static IEnumerable<EntityEntry> GetEntriesInChangeTraking(DbContext dbContext, Type entityType) {
            MethodInfo getEntries = dbContext.ChangeTracker.GetType().GetMethods().Where(p => p.IsGenericMethod && p.Name == "Entries").First().MakeGenericMethod(entityType);
            return (IEnumerable<EntityEntry>)getEntries.Invoke(dbContext.ChangeTracker, new object[] { });
        }
        public static bool CheckEntityInTracker(DbContext dbContext, object entity) {
            IEnumerable<EntityEntry> entityEntrys = GetEntriesInChangeTraking(dbContext, entity.GetType());
            return entityEntrys.Any(p => p.Entity == entity);
        }
        public static Expression GetDbSet(DbContext dbContext, ConstantExpression expressionParameter) {
            Type entityType = expressionParameter.Type.GetGenericArguments().First();
            object dbSet = GetDbSet(dbContext, entityType);
            return Expression.Constant(dbSet);
        }
        public static InternalEntityEntry GetInternalEntityEntry(DbContext dbContext, EntityEntry entityEntry) {
            var internalEntityEntryField = typeof(EntityEntry).GetTypeInfo().GetDeclaredField("_internalEntityEntry");
            return (InternalEntityEntry)internalEntityEntryField.GetValue(entityEntry);
        }

    }
}
