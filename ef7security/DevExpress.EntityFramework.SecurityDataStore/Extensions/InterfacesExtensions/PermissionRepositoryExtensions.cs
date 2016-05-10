using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Security {
    public static class PermissionsRepositoryExtensions {
        public static TypePermission SetTypePermission<T>(this IPermissionsRepository permissionsRepository, SecurityOperation operation, OperationState state) where T : class {
            return permissionsRepository.SetTypePermission(typeof(T), operation, state);
        }
    }
}
