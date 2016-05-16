using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Utility {
    public static class UtilityHelper {
        public static object GetDefaultValue(Type type) {
            object result = null;
            if(type.IsValueType) {
                result = Activator.CreateInstance(type);
            }
            return result;
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
    }
}
