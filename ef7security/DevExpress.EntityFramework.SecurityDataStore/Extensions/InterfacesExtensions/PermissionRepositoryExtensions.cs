using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Security {
    public static class PermissionsContainerExtensions {
        public static ITypePermission SetTypePermission<T>(this IPermissionsContainer PermissionsContainer, SecurityOperation operation, OperationState state) where T : class {
            return PermissionsContainer.SetTypePermission(typeof(T), operation, state);
        }
    }
}
