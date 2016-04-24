using DevExpress.EntityFramework.SecurityDataStore.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Authorization {
    public static class SecurityOperationPermissionExtensions {
        public static void SetValue(this SecurityPolicyPermission operationPermission, IPolicyPermission permission) {
            operationPermission.Operations = permission.Operations;
        }
        public static void SetValue(this SecurityPolicyPermission operationPermission, ISecurityPolicyPermission permission) {
            operationPermission.Operations = permission.Operations;
        }
        public static SecurityPolicyPermission CreateRolePermission(this IPolicyPermission permission) {
            SecurityPolicyPermission securityOperationPermission = new SecurityPolicyPermission();
            securityOperationPermission.SetValue(permission);
            return securityOperationPermission;
        }
        public static SecurityPolicyPermission CreateRolePermission(this ISecurityPolicyPermission permission) {
            SecurityPolicyPermission securityOperationPermission = new SecurityPolicyPermission();
            securityOperationPermission.SetValue(permission);
            return securityOperationPermission;
        }
    }
}
