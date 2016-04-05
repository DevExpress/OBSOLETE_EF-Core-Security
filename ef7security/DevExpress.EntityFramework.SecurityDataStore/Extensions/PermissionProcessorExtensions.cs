using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public static class PermissionProcessorExtensions {
        public static bool IsGranted(this IPermissionProcessor processor, Type type, SecurityOperation operation, object targetObject) {
            return processor.IsGranted(type, operation, targetObject, "");
        }
        public static bool IsGranted(this IPermissionProcessor processor, Type type, SecurityOperation operation) {
            return processor.IsGranted(type, operation, null, "");
        }
    }
}
