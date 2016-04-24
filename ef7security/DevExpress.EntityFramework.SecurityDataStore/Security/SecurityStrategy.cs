using DevExpress.EntityFramework.SecurityDataStore.Security;
using DevExpress.EntityFramework.SecurityDataStore.Security.Services;
using DevExpress.EntityFramework.SecurityDataStore.Utility;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public class SecurityStrategy : ISecurityStrategy {
        protected SecurityDbContext securityDbContext;
        public virtual ISecurityServicesProvider SecurityServicesProvider { get; }
        protected virtual IList<IPermission> SecurityPermissions { get; } = new List<IPermission>();
        public virtual TypePermission FindFirstTypePermission<T>() where T : class {
            return FindFirstTypePermission(typeof(T));
        }
        public virtual TypePermission FindFirstTypePermission(Type type) {
            return SecurityPermissions.OfType<TypePermission>().FirstOrDefault(p => p.Type == type);
        }      
        public virtual TypePermission SetTypePermission(Type type, SecurityOperation operation, OperationState state) {
            TypePermission typePermission = FindFirstTypePermission(type);
            if(typePermission == null) {
                typePermission = new TypePermission(type);
                SecurityPermissions.Add(typePermission);
            }
            typePermission.Operations = operation;
            typePermission.OperationState = state;
            return typePermission;
        }
        public virtual ObjectPermission<TSource, TargetType> AddObjectPermission<TSource, TargetType>(SecurityOperation operation, OperationState state, Expression<Func<TSource, TargetType, bool>> criteria) where TSource : SecurityDbContext {
            var objectPermission = new ObjectPermission<TSource, TargetType>(criteria);
            objectPermission.Type = typeof(TargetType);
            objectPermission.Operations = operation;
            objectPermission.OperationState = state;
            SecurityPermissions.Add(objectPermission);
            return objectPermission;
        }
        public virtual MemberPermission<TSource, TargetType> AddMemberPermission<TSource, TargetType>(SecurityOperation operation, OperationState state, string memberName, Expression<Func<TSource, TargetType, bool>> criteria) where TSource : SecurityDbContext {
            if(operation.HasFlag(SecurityOperation.Create))
                throw new ArgumentException("The create value of the 'operations' parameter is incorrect in this context. Only the Read and Write operations can be granted by a member permission.");

            if(operation.HasFlag(SecurityOperation.Delete))
                throw new ArgumentException("The delete value of the 'operations' parameter is incorrect in this context. Only the Read and Write operations can be granted by a member permission.");

            var memberPermission = new MemberPermission<TSource, TargetType>(memberName, criteria);
            memberPermission.Type = typeof(TargetType);
            memberPermission.Operations = operation;
            memberPermission.OperationState = state;
            SecurityPermissions.Add(memberPermission);
            return memberPermission;
        }
        public virtual bool RemovePermission(IPermission permission) {
            return SecurityPermissions.Remove(permission);
        }
        public virtual IEnumerable<IPermission> GetAllPermissions() {
            return SecurityPermissions.ToArray();
        }
        public virtual void AddPermission(IPermission permission) {
            SecurityPermissions.Add(permission);
        }
        public void ClearPermissions() {
            SecurityPermissions.Clear();
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
            SecurityPermissions.Add(operationPermission);
            return operationPermission;
        } 
        public virtual bool IsGranted(Type type, SecurityOperation operation, object targetObject, string memberName) {
            return SecurityServicesProvider.PermissionProcessor.IsGranted(type, operation, targetObject, memberName);
        }
        public SecurityStrategy(DbContext dbContext) {
            securityDbContext = ((SecurityDbContext)dbContext);
            SecurityServicesProvider = new SecurityServicesProvider(securityDbContext, SecurityPermissions);
        }
    }
}
