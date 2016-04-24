using DevExpress.EntityFramework.SecurityDataStore.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Authorization {
    public static class SecurityTypePermissionExtensions {
        public static void SetValue(this SecurityTypePermission operationPermission, ITypePermission permission) {
            operationPermission.Operations = permission.Operations;
            operationPermission.OperationState = permission.OperationState;
            operationPermission.Type = permission.Type;
        }
        public static void SetValue(this SecurityTypePermission operationPermission, ISecurityTypePermission permission) {
            operationPermission.Operations = permission.Operations;
            operationPermission.OperationState = permission.OperationState;
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            // TODO: more checks are needed...
            operationPermission.Type = ((ParameterExpression)criteriaSerializer.Deserialize(permission.StringType)).Type;
        }
        public static SecurityTypePermission CreateRolePermission(this ITypePermission permission) {
            SecurityTypePermission securityOperationPermission = new SecurityTypePermission();
            securityOperationPermission.SetValue(permission);
            return securityOperationPermission;
        }
        public static SecurityTypePermission CreateRolePermission(this ISecurityTypePermission permission) {
            SecurityTypePermission securityOperationPermission = new SecurityTypePermission();
            securityOperationPermission.SetValue(permission);
            return securityOperationPermission;
        }
    }
}
