using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using DevExpress.EntityFramework.SecurityDataStore.Security;
using DevExpress.EntityFramework.SecurityDataStore.Security.BaseSecurityEntity;

namespace DevExpress.EntityFramework.SecurityDataStore {   
    public static class PermissionObjectHelper {      
        public static object Set(this DbContext dbContext, Type type) {
            return MethodInfoSet.MakeGenericMethod(type).Invoke(null, new object[] { dbContext });
        }
        private static MethodInfo methodInfoSet;
        private static MethodInfo MethodInfoSet {
            get {
                if(methodInfoSet == null)
                    methodInfoSet = typeof(PermissionObjectHelper).GetRuntimeMethods().First(p => p.Name == "SetGeneric");
                return methodInfoSet;
            }
        }
        private static DbSet<TEntity> SetGeneric<TEntity>(DbContext dbContext) where TEntity : class {
            return dbContext.Set<TEntity>();
        }
        public static void AddPermissions(this IPermissionsContainer permissionsContainer, IEnumerable<IPermission> permissions) {
            foreach(IPermission permission in permissions) {
                permissionsContainer.AddPermission(permission);
            }
        }
        public static string GetValue(this ISecurityEntity securityEntity, string propertyValue, string propertyName) {
            if(propertyValue == null && securityEntity.BlockedMembers.Contains(propertyName)) {
                return "Protected Content";
            }
            else {
                return propertyValue;
            }
        }
    }
}
