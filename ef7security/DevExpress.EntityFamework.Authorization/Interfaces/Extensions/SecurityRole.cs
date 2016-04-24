using DevExpress.EntityFramework.SecurityDataStore.Security;
using DevExpress.EntityFramework.SecurityDataStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace DevExpress.EntityFramework.SecurityDataStore.Authorization {
    public static class SecurityRoleExtensions {
        public static SecurityPolicyPermission SetPermissionPolicy(this ISecurityRole role, PermissionPolicy policy) {
            SecurityPolicyPermission operationPermission = new SecurityPolicyPermission();
            switch(policy) {
                case PermissionPolicy.AllowAllByDefault:
                    operationPermission.Operations = SecurityOperation.FullAccess;
                    break;
                case PermissionPolicy.ReadOnlyAllByDefault:
                    operationPermission.Operations = SecurityOperation.Read;
                    break;
                case PermissionPolicy.DenyAllByDefault:
                    break;
                default:
                    break;
            }
            role.OperationPermissions.Add(operationPermission);
            return operationPermission;
        }
        public static SecurityTypePermission FindFirstTypePermission<T>(this ISecurityRole role) where T : class {
            return FindFirstTypePermission(role, typeof(T));
        }
        public static SecurityTypePermission FindFirstTypePermission(this ISecurityRole role, Type type) {
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            return role.TypePermissions.OfType<SecurityTypePermission>().FirstOrDefault(p => ((ParameterExpression)criteriaSerializer.Deserialize(p.StringType)).Type == type);
        }
        public static ISecurityTypePermission SetTypePermission(this ISecurityRole role, Type type, SecurityOperation operation, OperationState state) {
            SecurityTypePermission typePermission = FindFirstTypePermission(role, type);
            if(typePermission == null) {
                typePermission = new SecurityTypePermission() { Type = type };
                role.TypePermissions.Add(typePermission);
            }
            typePermission.Operations = operation;
            typePermission.OperationState = state;
            return typePermission;
        }
        public static SecurityObjectPermission AddObjectPermission<TSource, TargetType>(this ISecurityRole role, SecurityOperation operation, OperationState state, Expression<Func<TSource, TargetType, bool>> criteria) where TSource : SecurityDbContext {
            SecurityObjectPermission objectPermission = new SecurityObjectPermission();
            objectPermission.Type = typeof(TargetType);
            objectPermission.Criteria = criteria;
            objectPermission.Operations = operation;
            objectPermission.OperationState = state;
            role.ObjectPermissions.Add(objectPermission);
            return objectPermission;
        }
        public static SecurityMemberPermission AddMemberPermission<TSource, TargetType>(this ISecurityRole role, SecurityOperation operation, OperationState state, string memberName, Expression<Func<TSource, TargetType, bool>> criteria) where TSource : SecurityDbContext {
            if(operation.HasFlag(SecurityOperation.Create))
                throw new ArgumentException("The create value of the 'operations' parameter is incorrect in this context. Only the Read and Write operations can be granted by a member permission.");
            if(operation.HasFlag(SecurityOperation.Delete))
                throw new ArgumentException("The delete value of the 'operations' parameter is incorrect in this context. Only the Read and Write operations can be granted by a member permission.");
            SecurityMemberPermission memberPermission = new SecurityMemberPermission();
            memberPermission.Type = typeof(TargetType);
            memberPermission.Criteria = criteria;
            memberPermission.Operations = operation;
            memberPermission.OperationState = state;
            memberPermission.MemberName = memberName;
            role.MemberPermissions.Add(memberPermission);
            return memberPermission;
        }
    }
}
