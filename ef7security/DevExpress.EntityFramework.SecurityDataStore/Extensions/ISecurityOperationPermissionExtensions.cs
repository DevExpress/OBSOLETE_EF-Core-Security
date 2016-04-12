using DevExpress.EntityFramework.SecurityDataStore.Extensions;
using DevExpress.EntityFramework.SecurityDataStore.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public static class SecurityOperationPermissionExtensions {
        public static void SetValue(this SecurityOperationPermission operationPermission, IPermission permission) {
            operationPermission.Operations = permission.Operations;
        }
        public static SecurityOperationPermission CreateRolePermission(this IPermission permission) {
            SecurityOperationPermission securityOperationPermission = new SecurityOperationPermission();
            securityOperationPermission.SetValue(permission);
            return securityOperationPermission;
        }
    }

    public static class SecurityTypePermissionExtensions {
        public static void SetValue(this SecurityTypePermission operationPermission, ITypePermission permission) {
            operationPermission.Operations = permission.Operations;
            operationPermission.OperationState = permission.OperationState;
            operationPermission.Type = permission.Type;
        }
        public static SecurityOperationPermission CreateRolePermission(this ITypePermission permission) {
            SecurityOperationPermission securityOperationPermission = new SecurityOperationPermission();
            securityOperationPermission.SetValue(permission);
            return securityOperationPermission;
        }
    }
    public static class SecurityObjectPermissionExtensions {
        public static void SetValue(this SecurityObjectPermission operationPermission, IObjectPermission permission) {
            operationPermission.Operations = permission.Operations;
            operationPermission.OperationState = permission.OperationState;
            operationPermission.Type = permission.Type;
            operationPermission.Criteria = permission.Criteria;
        }
        public static SecurityOperationPermission CreateRolePermission(this IObjectPermission permission) {
            SecurityOperationPermission securityOperationPermission = new SecurityOperationPermission();
            securityOperationPermission.SetValue(permission);
            return securityOperationPermission;
        }
    }
    public static class SecurityMemberPermissionExtensions {
        public static void SetValue(this SecurityMemberPermission operationPermission, IMemberPermission permission) {
            operationPermission.Operations = permission.Operations;
            operationPermission.OperationState = permission.OperationState;
            operationPermission.Type = permission.Type;
            operationPermission.Criteria = permission.Criteria;
            operationPermission.MemberName = permission.MemberName;
        }
        public static SecurityOperationPermission CreateRolePermission(this IMemberPermission permission) {
            SecurityOperationPermission securityOperationPermission = new SecurityOperationPermission();
            securityOperationPermission.SetValue(permission);
            return securityOperationPermission;
        }
    }
}
