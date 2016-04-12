using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public static class ISecurityUsereExtensions {
        public static IEnumerable<IPermission> GetAllPermissions(this ISecurityUser securityUser) {
            List<IPermission> permissions = new List<IPermission>();
            foreach(var userRole in securityUser.UserRoleCollection) {
                permissions.AddRange(userRole.Role.OperationPermissionCollection);
                permissions.AddRange(userRole.Role.TypePermissionCollection);
                permissions.AddRange(userRole.Role.ObjectPermissionCollection);
                permissions.AddRange(userRole.Role.MemberPermissionCollection);
            }
            return permissions;
        }
    }
}
