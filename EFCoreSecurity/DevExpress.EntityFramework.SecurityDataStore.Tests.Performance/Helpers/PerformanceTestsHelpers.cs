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

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.Performance {
    public enum TestType { WithoutPermissions, WithOnePermission, WithMultiplePermissions };
    public static class PerformanceTestsHelper {
        
        public static List<Func<IDbContextMultiClass>> GetContextCreators(int count) {
            List<Func<IDbContextMultiClass>> contexts = new List<Func<IDbContextMultiClass>>();

            for(int i = 0; i < 5; i++) {
                contexts.Add(() => new DbContextMultiClass());
                contexts.Add(() => new NativeDbContextMultiClass());
            }

            return contexts;
        }

        public static List<Func<IDbContextMultiClass>> GetContextCreators() {
            return GetContextCreators(3);
        }

        public static List<Func<IDbContextConnectionClass>> GetCollectionContextCreators(int count) {
            List<Func<IDbContextConnectionClass>> contexts = new List<Func<IDbContextConnectionClass>>();

            for (int i = 0; i < 5; i++) {
                contexts.Add(() => new DbContextConnectionClass());
                contexts.Add(() => new NativeDbContextConnectionClass());
            }

            return contexts;
        }

        public static List<Func<IDbContextConnectionClass>> GetCollectionContextCreators() {
            return GetCollectionContextCreators(3);
        }

        public static long GetSecuredContextTime(List<long> times) {
            return (long)times.Where((t, i) => i % 2 == 0).Average();
        }
        public static long GetNativeContextTime(List<long> times) {
            return (long)times.Where((t, i) => i % 2 != 0).Average();
        }

        public static void AddOnePermission(SecurityDbContext securityDbContext, SecurityOperation operation) {
            securityDbContext.PermissionsContainer.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);

            Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria = (db, obj) => obj.Description.StartsWith("Description");

            foreach (SecurityOperation securityOperation in GetSecurityOperations())
                securityDbContext.PermissionsContainer.AddObjectPermission(securityOperation, OperationState.Allow, criteria);
        }

        public static void AddOneCollectionPermission(SecurityDbContext securityDbContext, SecurityOperation operation) {
            securityDbContext.PermissionsContainer.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);

            Expression<Func<DbContextConnectionClass, Company, bool>> companyCriteria = (db, obj) => obj.Description.StartsWith("Description");

            foreach (SecurityOperation securityOperation in GetSecurityOperations())
                securityDbContext.PermissionsContainer.AddObjectPermission(securityOperation, OperationState.Allow, companyCriteria);

            Expression<Func<DbContextConnectionClass, Office, bool>> officeCriteria = (db, obj) => obj.Description.StartsWith("Description");

            foreach (SecurityOperation securityOperation in GetSecurityOperations())
                securityDbContext.PermissionsContainer.AddObjectPermission(securityOperation, OperationState.Allow, officeCriteria);
        }

        public static void AddMultiplePermissions(SecurityDbContext securityDbContext, SecurityOperation operation) {
            securityDbContext.PermissionsContainer.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);

            Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria1 = (db, obj) => obj.Description.StartsWith("Description");
            Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria2 = (db, obj) => obj.Description.Length > 2;
            Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria3 = (db, obj) => !obj.Description.Contains("SomeBadPhrase");;
            foreach (SecurityOperation securityOperation in GetSecurityOperations()) {
                securityDbContext.PermissionsContainer.AddObjectPermission(securityOperation, OperationState.Allow, criteria1);
                securityDbContext.PermissionsContainer.AddObjectPermission(securityOperation, OperationState.Allow, criteria2);
                securityDbContext.PermissionsContainer.AddObjectPermission(securityOperation, OperationState.Allow, criteria3);
            }
        }

        public static void AddMultipleCollectionPermissions(SecurityDbContext securityDbContext, SecurityOperation operation) {
            securityDbContext.PermissionsContainer.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);

            Expression<Func<DbContextConnectionClass, Company, bool>> companyCriteria1 = (db, obj) => obj.Description.StartsWith("Description");
            Expression<Func<DbContextConnectionClass, Company, bool>> companyCriteria2 = (db, obj) => obj.Description.Length > 2;
            Expression<Func<DbContextConnectionClass, Company, bool>> companyCriteria3 = (db, obj) => !obj.Description.Contains("SomeBadPhrase"); ;
            foreach (SecurityOperation securityOperation in GetSecurityOperations()) {
                securityDbContext.PermissionsContainer.AddObjectPermission(securityOperation, OperationState.Allow, companyCriteria1);
                securityDbContext.PermissionsContainer.AddObjectPermission(securityOperation, OperationState.Allow, companyCriteria2);
                securityDbContext.PermissionsContainer.AddObjectPermission(securityOperation, OperationState.Allow, companyCriteria3);
            }

            Expression<Func<DbContextConnectionClass, Office, bool>> officeCriteria1 = (db, obj) => obj.Description.StartsWith("Description");
            Expression<Func<DbContextConnectionClass, Office, bool>> officeCriteria2 = (db, obj) => obj.Description.Length > 2;
            Expression<Func<DbContextConnectionClass, Office, bool>> officeCriteria3 = (db, obj) => !obj.Description.Contains("SomeBadPhrase"); ;
            foreach (SecurityOperation securityOperation in GetSecurityOperations()) {
                securityDbContext.PermissionsContainer.AddObjectPermission(securityOperation, OperationState.Allow, officeCriteria1);
                securityDbContext.PermissionsContainer.AddObjectPermission(securityOperation, OperationState.Allow, officeCriteria2);
                securityDbContext.PermissionsContainer.AddObjectPermission(securityOperation, OperationState.Allow, officeCriteria3);
            }
        }

        public static SecurityOperation[] GetSecurityOperations() {
            return new[] {SecurityOperation.Read, SecurityOperation.Write, SecurityOperation.Create, SecurityOperation.Delete};
        }
    }
}
