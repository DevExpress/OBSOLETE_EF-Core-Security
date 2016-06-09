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

            for(int i = 0; i < 5; i++) {
                contexts.Add(() => new DbContextMultiClass());
                contexts.Add(() => new NativeDbContextMultiClass());
            }

            return contexts;
        }
        public static long getSecuredContextTime(List<long> times) {
            long sum = 0;
            for(int i = 0; i < times.Count; i++) {
                if(i % 2 == 0)
                    sum += times[i];
            }
            return sum / (times.Count / 2);
        }
        public static long getNativeContextTime(List<long> times) {
            long sum = 0;
            for(int i = 0; i < times.Count; i++) {
                if(i % 2 != 0)
                    sum += times[i];
            }
            return sum / (times.Count / 2);
        }

        public static void AddOnePermission(SecurityDbContext securityDbContext, SecurityOperation operation) {
            securityDbContext.PermissionsContainer.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);

            Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria = (db, obj) => obj.Description.StartsWith("Description");

            foreach(SecurityOperation securityOperation in
                new SecurityOperation[] { SecurityOperation.Read, SecurityOperation.Write, SecurityOperation.Create, SecurityOperation.Delete }) {
                securityDbContext.PermissionsContainer.AddObjectPermission(securityOperation, OperationState.Allow, criteria);
            }
        }

        public static void AddMultiplePermissions(SecurityDbContext securityDbContext, SecurityOperation operation) {
            securityDbContext.PermissionsContainer.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);

            Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria1 = (db, obj) => obj.Description.StartsWith("Description");
            Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria2 = (db, obj) => obj.Description.Length > 2;
            Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria3 = (db, obj) => !obj.Description.Contains("SomeBadPhrase");;
            foreach(SecurityOperation securityOperation in
                new SecurityOperation[] { SecurityOperation.Read, SecurityOperation.Write, SecurityOperation.Create, SecurityOperation.Delete }) {
                securityDbContext.PermissionsContainer.AddObjectPermission(securityOperation, OperationState.Allow, criteria1);
                securityDbContext.PermissionsContainer.AddObjectPermission(securityOperation, OperationState.Allow, criteria2);
                securityDbContext.PermissionsContainer.AddObjectPermission(securityOperation, OperationState.Allow, criteria3);
            }
        }
    }
}
