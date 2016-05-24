using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Security {
    public class PermissionsContainer : IPermissionsContainer {
        protected virtual List<IPermission> permissions { get; private set; }
        public virtual TypePermission FindFirstTypePermission<T>() where T : class {
            return FindFirstTypePermission(typeof(T));
        }
        public virtual TypePermission FindFirstTypePermission(Type type) {
            return permissions.OfType<TypePermission>().FirstOrDefault(p => p.Type == type);
        }
        public virtual ITypePermission SetTypePermission(Type type, SecurityOperation operation, OperationState state) {
            TypePermission typePermission = FindFirstTypePermission(type);
            if(typePermission == null) {
                typePermission = new TypePermission(type);
                permissions.Add(typePermission);
            }
            typePermission.Operations = operation;
            typePermission.OperationState = state;
            return typePermission;
        }
        public virtual IObjectPermission AddObjectPermission<TSource, TargetType>(SecurityOperation operation, OperationState state, Expression<Func<TSource, TargetType, bool>> criteria) where TSource : SecurityDbContext {
            var objectPermission = new ObjectPermission<TSource, TargetType>(criteria);
            objectPermission.Type = typeof(TargetType);
            objectPermission.Operations = operation;
            objectPermission.OperationState = state;
            permissions.Add(objectPermission);
            return objectPermission;
        }
        public virtual IMemberPermission AddMemberPermission<TSource, TargetType>(SecurityOperation operation, OperationState state, string memberName, Expression<Func<TSource, TargetType, bool>> criteria) where TSource : SecurityDbContext {
            if(operation.HasFlag(SecurityOperation.Create))
                throw new ArgumentException("The create value of the 'operations' parameter is incorrect in this context. Only the Read and Write operations can be granted by a member permission.");

            if(operation.HasFlag(SecurityOperation.Delete))
                throw new ArgumentException("The delete value of the 'operations' parameter is incorrect in this context. Only the Read and Write operations can be granted by a member permission.");

            Type targetType = typeof(TargetType);
            PropertyInfo targetMember = targetType.GetProperty(memberName);

            if(targetMember == null)
                throw new ArgumentException(string.Format("{0} type doesn't contain {1} property.", targetType.Name, memberName));

            var memberPermission = new MemberPermission<TSource, TargetType>(memberName, criteria);
            memberPermission.Type = typeof(TargetType);
            memberPermission.Operations = operation;
            memberPermission.OperationState = state;
            permissions.Add(memberPermission);
            return memberPermission;
        }
        public virtual bool RemovePermission(IPermission permission) {
            return permissions.Remove(permission);
        }
        public virtual IEnumerable<IPermission> GetPermissions() {
            return permissions.ToArray();
        }
        public virtual void AddPermission(IPermission permission) {
            permissions.Add(permission);
        }
        public void ClearPermissions() {
            permissions.Clear();
        }
        public virtual IPermission SetPermissionPolicy(PermissionPolicy policy) {
            PolicyPermission operationPermission = new PolicyPermission(SecurityOperation.NoAccess);
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
            permissions.Add(operationPermission);
            return operationPermission;
        }
        public PermissionsContainer() {
            permissions = new List<IPermission>();
        }
    }
}
