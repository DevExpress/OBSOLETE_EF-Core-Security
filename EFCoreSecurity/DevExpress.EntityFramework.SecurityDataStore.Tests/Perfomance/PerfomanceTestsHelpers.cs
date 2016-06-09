using NUnit.Framework;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Collections.Generic;
using DevExpress.EntityFramework.SecurityDataStore.Tests.Helpers;
using DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.Perfomance {
    public static class PerfomanceTestsHelper {
        public static List<Func<IDbContextMultiClass>> GetContextCreators() {
            List<Func<IDbContextMultiClass>> contexts = new List<Func<IDbContextMultiClass>>();
            contexts.Add(() => new DbContextMultiClass());
            contexts.Add(() => new NativeDbContextMultiClass());
            return contexts;
        }

        public static void AddOnePermission(SecurityDbContext securityDbContext, SecurityOperation operation) {
            securityDbContext.PermissionsContainer.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);

            Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria = (db, obj) => obj.Description.StartsWith("Description");
            securityDbContext.PermissionsContainer.AddObjectPermission(operation, OperationState.Allow, criteria);
        }

        public static void AddMultiplePermissions(SecurityDbContext securityDbContext, SecurityOperation operation) {
            securityDbContext.PermissionsContainer.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);

            Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria1 = (db, obj) => obj.Description.StartsWith("Description");
            Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria2 = (db, obj) => obj.Description.Length > 2;
            Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria3 = (db, obj) => !obj.Description.Contains("SomeBadPhrase");;
            securityDbContext.PermissionsContainer.AddObjectPermission(operation, OperationState.Allow, criteria1);
            securityDbContext.PermissionsContainer.AddObjectPermission(operation, OperationState.Allow, criteria2);
            securityDbContext.PermissionsContainer.AddObjectPermission(operation, OperationState.Allow, criteria3);
        }
    }
}
