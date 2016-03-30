using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public static class SecurityUtility {

        public static bool IsSystemProperty(this IEntityType entityEntry, string properyName) {
            bool result = false;           
            IProperty properyEntity = entityEntry.GetProperties().FirstOrDefault(p => p.Name == properyName);
            if(properyEntity != null) {
                PropertyInfo properyInfo = entityEntry.ClrType.GetType().GetRuntimeProperties().FirstOrDefault(p => p.Name == properyName);
                result |= properyEntity.IsKey();
                result |= properyEntity.FindContainingForeignKeys().Count() > 0;
                result |= properyEntity.FindContainingKeys().Count() > 0;
                result |= properyInfo != null && properyInfo.GetGetMethod().IsStatic;
            }
            return result;            
        }
        public static object GetDefaultValue(this Type type) {
            object result = null;
            if(type.IsValueType) {
                result = Activator.CreateInstance(type);
            }
            return result;
        }
    }
}
