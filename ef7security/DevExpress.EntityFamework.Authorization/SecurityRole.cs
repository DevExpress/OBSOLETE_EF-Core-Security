using DevExpress.EntityFramework.SecurityDataStore.Security.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.EntityFramework.SecurityDataStore.Security;
using System.Linq.Expressions;

namespace DevExpress.EntityFramework.SecurityDataStore.Authorization {
    public class SecurityRole : BaseSecurityObject, ISecurityRole {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public ICollection<UserRole> Roles { get; set; }
        = new Collection<UserRole>();
        public ICollection<SecurityTypePermission> TypePermissions { get; set; }
            = new Collection<SecurityTypePermission>();
        public ICollection<SecurityPolicyPermission> OperationPermissions { get; set; }
           = new Collection<SecurityPolicyPermission>();
        public ICollection<SecurityMemberPermission> MemberPermissions { get; set; }
           = new Collection<SecurityMemberPermission>();
        public ICollection<SecurityObjectPermission> ObjectPermissions { get; set; }
           = new Collection<SecurityObjectPermission>();

        IEnumerable<IUserRole> ISecurityRole.Roles {
            get {
                return Roles.OfType<IUserRole>();
            }
        }
        IEnumerable<ISecurityTypePermission> ISecurityRole.TypePermissions {
            get {
                return TypePermissions.OfType<ISecurityTypePermission>();
            }
        }
        IEnumerable<ISecurityPolicyPermission> ISecurityRole.OperationPermissions {
            get {
                return OperationPermissions.OfType<ISecurityPolicyPermission>();
            }
        }
        IEnumerable<ISecurityMemberPermission> ISecurityRole.MemberPermissions {
            get {
                return MemberPermissions.OfType<ISecurityMemberPermission>();
            }
        }
        IEnumerable<ISecurityObjectPermission> ISecurityRole.ObjectPermissions {
            get {
                return ObjectPermissions.OfType<ISecurityObjectPermission>();
            }
        }
        private SecurityTypePermission FindFirstTypePermission(ISecurityRole role, Type type) {
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            return role.TypePermissions.OfType<SecurityTypePermission>().FirstOrDefault(p => ((ParameterExpression)criteriaSerializer.Deserialize(p.StringType)).Type == type);
        }
        public ITypePermission SetTypePermission(Type type, SecurityOperation operation, OperationState state) {
            SecurityTypePermission typePermission = FindFirstTypePermission(this, type);
            if(typePermission == null) {
                typePermission = new SecurityTypePermission() { Type = type };
                this.TypePermissions.Add(typePermission);
            }
            typePermission.Operations = operation;
            typePermission.OperationState = state;
            return typePermission;
        }

        public IObjectPermission AddObjectPermission<TSource, TargetType>(SecurityOperation operation, OperationState state, Expression<Func<TSource, TargetType, bool>> criteria) where TSource : SecurityDbContext {
            SecurityObjectPermission objectPermission = new SecurityObjectPermission();
            objectPermission.Type = typeof(TargetType);
            objectPermission.Criteria = criteria;
            objectPermission.Operations = operation;
            objectPermission.OperationState = state;
            this.ObjectPermissions.Add(objectPermission);
            return objectPermission;
        }

        public IMemberPermission AddMemberPermission<TSource, TargetType>(SecurityOperation operation, OperationState state, string memberName, Expression<Func<TSource, TargetType, bool>> criteria) where TSource : SecurityDbContext {
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
            this.MemberPermissions.Add(memberPermission);
            return memberPermission;
        }

        public bool RemovePermission(IPermission permission) {
            throw new NotImplementedException();
        }

        public void AddPermission(IPermission permission) {
            throw new NotImplementedException();
        }

        public void ClearPermissions() {
            throw new NotImplementedException();
        }

        public IEnumerable<IPermission> GetPermissions() {
            List<IPermission> permissions = new List<IPermission>();
            permissions.AddRange(OperationPermissions);
            permissions.AddRange(TypePermissions);
            permissions.AddRange(ObjectPermissions);
            permissions.AddRange(MemberPermissions);
            return permissions;
        }

        public IPermission SetPermissionPolicy(PermissionPolicy policy) {
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
            OperationPermissions.Add(operationPermission);
            return operationPermission;
        }
    }
}
