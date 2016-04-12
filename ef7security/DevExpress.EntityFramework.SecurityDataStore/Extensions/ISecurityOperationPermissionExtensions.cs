using DevExpress.EntityFramework.SecurityDataStore.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore {
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
