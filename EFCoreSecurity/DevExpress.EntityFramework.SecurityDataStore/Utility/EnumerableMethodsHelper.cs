using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Utility {
   public static class EnumerableMethodsHelper {
        private static MethodInfo select;
        public static MethodInfo Select {
            get {
                if(select == null) {
                    IEnumerable<MethodInfo> methodsInfo = typeof(Enumerable).GetMethods().Where(p => p.Name == "Select");
                    foreach(MethodInfo methodInfo in methodsInfo) {
                        Type[] genericArguments = methodInfo.GetParameters().Last().ParameterType.GetGenericArguments();
                        if(genericArguments.Count() == 2) {
                            select = methodInfo;
                            break;
                        }
                    }
                }
                return select;
            }
        }
        private static MethodInfo selectMany;
        public static MethodInfo SelectMany {
            get {
                if(selectMany == null) {
                    IEnumerable<MethodInfo> methodsInfo = typeof(Enumerable).GetMethods().Where(p => p.Name == "SelectMany");
                    foreach(MethodInfo methodInfo in methodsInfo) {
                        Type[] genericArguments = methodInfo.GetParameters().Last().ParameterType.GetGenericArguments();
                        if(genericArguments.Count() == 2) {
                            selectMany = methodInfo;
                            break;
                        }
                    }
                }
                return selectMany;
            }
        }
    }
}
