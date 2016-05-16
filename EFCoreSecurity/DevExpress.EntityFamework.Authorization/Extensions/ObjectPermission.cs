using DevExpress.EntityFramework.SecurityDataStore.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Authorization {
    public static class SecurityObjectPermissionExtensions {
        public static void SetValue(this SecurityObjectPermission operationPermission, IObjectPermission permission) {
            operationPermission.Operations = permission.Operations;
            operationPermission.OperationState = permission.OperationState;
            operationPermission.Type = permission.Type;
            operationPermission.Criteria = permission.Criteria;
        }
        public static void SetValue(this SecurityObjectPermission operationPermission, ISecurityObjectPermission permission) {
            operationPermission.Operations = permission.Operations;
            operationPermission.OperationState = permission.OperationState;
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();

            // TODO: more checks are needed...
            operationPermission.Type = ((ParameterExpression)criteriaSerializer.Deserialize(permission.StringType)).Type;
            operationPermission.Criteria = (LambdaExpression)criteriaSerializer.Deserialize(permission.StringCriteria);
        }
        public static SecurityObjectPermission CreateRolePermission(this IObjectPermission permission) {
            SecurityObjectPermission securityOperationPermission = new SecurityObjectPermission();
            securityOperationPermission.SetValue(permission);
            return securityOperationPermission;
        }
        public static SecurityObjectPermission CreateRolePermission(this ISecurityObjectPermission permission) {
            SecurityObjectPermission securityOperationPermission = new SecurityObjectPermission();
            securityOperationPermission.SetValue(permission);
            return securityOperationPermission;
        }
    }
}
