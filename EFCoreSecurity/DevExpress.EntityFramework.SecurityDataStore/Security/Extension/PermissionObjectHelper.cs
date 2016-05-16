using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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
    }

}
