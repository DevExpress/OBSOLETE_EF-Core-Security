using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Authorization {
    public static class SecurityUserExtensions {
        public static IEnumerable<IPermission> GetAllPermissions(this ISecurityUser securityUser) {
            List<IPermission> permissions = new List<IPermission>();
            foreach(var userRole in securityUser.UserRoleCollection) {
                permissions.AddRange(userRole.Role.OperationPermissions);
                permissions.AddRange(userRole.Role.TypePermissions);
                permissions.AddRange(userRole.Role.ObjectPermissions);
                permissions.AddRange(userRole.Role.MemberPermissions);
            }
            return permissions;
        }
    }
}
