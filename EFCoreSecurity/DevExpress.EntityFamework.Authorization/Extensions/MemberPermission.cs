using DevExpress.EntityFramework.SecurityDataStore.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Authorization {
    public static class SecurityMemberPermissionExtensions {
        public static void SetValue(this SecurityMemberPermission operationPermission, IMemberPermission permission) {
            operationPermission.Operations = permission.Operations;
            operationPermission.OperationState = permission.OperationState;
            operationPermission.Type = permission.Type;
            operationPermission.Criteria = permission.Criteria;
            operationPermission.MemberName = permission.MemberName;
        }
        public static void SetValue(this SecurityMemberPermission operationPermission, ISecurityMemberPermission permission) {
            operationPermission.Operations = permission.Operations;
            operationPermission.OperationState = permission.OperationState;
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();

            // TODO: more checks are needed...
            operationPermission.Type = ((ParameterExpression)criteriaSerializer.Deserialize(permission.StringType)).Type;
            operationPermission.Criteria = (LambdaExpression)criteriaSerializer.Deserialize(permission.StringCriteria);
            operationPermission.MemberName = permission.MemberName;
        }
        public static SecurityMemberPermission CreateRolePermission(this IMemberPermission permission) {
            SecurityMemberPermission securityOperationPermission = new SecurityMemberPermission();
            securityOperationPermission.SetValue(permission);
            return securityOperationPermission;
        }
        public static SecurityMemberPermission CreateRolePermission(this ISecurityMemberPermission permission) {
            SecurityMemberPermission securityOperationPermission = new SecurityMemberPermission();
            securityOperationPermission.SetValue(permission);
            return securityOperationPermission;
        }
    }
}
