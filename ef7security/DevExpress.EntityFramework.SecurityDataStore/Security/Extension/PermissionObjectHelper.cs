using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public enum PermissionPolicy { AllowAllByDefault, ReadOnlyAllByDefault, DenyAllByDefault }
    public static class PermissionObjectHelper {
        public static TypePermission FindFirstTypePermission<T>(this ISecurityStrategy securityStrategy) where T : class{
            return FindFirstTypePermission(securityStrategy, typeof(T));
        }
        public static TypePermission FindFirstTypePermission(this ISecurityStrategy securityStrategy, Type type) {
            return securityStrategy.SecurityPermissions.OfType<TypePermission>().FirstOrDefault(p => p.Type == type);
        }
        public static TypePermission SetTypePermission<T>(this ISecurityStrategy securityStrategy, SecurityOperation operation, OperationState state) where T : class {
            return SetTypePermission(securityStrategy, typeof(T), operation, state);
        }
        public static TypePermission SetTypePermission(this ISecurityStrategy securityStrategy, Type type, SecurityOperation operation, OperationState state) {
            TypePermission typePermission = FindFirstTypePermission(securityStrategy, type);
            if(typePermission == null) {
                typePermission = new TypePermission(type);
                securityStrategy.SecurityPermissions.Add(typePermission);    
            }
            typePermission.Operations = operation;
            typePermission.OperationState = state;
            return typePermission;
        }

        //public static ObjectPermission<TSource, TargetType> AddObjectPermission<TSource, TargetType>(this ISecurityStrategy securityStrategy, string operation, OperationState state, Expression<Func<TSource, TargetType, bool>> criteria) where TSource : SecurityDbContext {
        //    ObjectPermission<TSource, TargetType> objectPermission = new ObjectPermission<TSource, TargetType>(criteria);
        //    securityStrategy.SecurityPermissions.Add(objectPermission);
        //    return objectPermission;
        //}

        public static ObjectPermission<TSource, TargetType> AddObjectPermission<TSource, TargetType>(this ISecurityStrategy securityStrategy, SecurityOperation operation, OperationState state, Expression<Func<TSource, TargetType, bool>> criteria) where TSource : SecurityDbContext {
            var objectPermission = new ObjectPermission<TSource, TargetType>(criteria);
            objectPermission.Operations = operation;
            objectPermission.OperationState = state;
            securityStrategy.SecurityPermissions.Add(objectPermission);
            return objectPermission;
        }
        public static MemberPermission<TSource, TargetType> AddMemberPermission<TSource, TargetType>(this ISecurityStrategy securityStrategy, SecurityOperation operation, OperationState state, string memberName, Expression<Func<TSource, TargetType, bool>> criteria) where TSource : SecurityDbContext {
            if(operation.HasFlag(SecurityOperation.Create))
                throw new ArgumentException("The create value of the 'operations' parameter is incorrect in this context. Only the Read and Write operations can be granted by a member permission.");

            if(operation.HasFlag(SecurityOperation.Delete))
                throw new ArgumentException("The delete value of the 'operations' parameter is incorrect in this context. Only the Read and Write operations can be granted by a member permission.");

            var memberPermission = new MemberPermission<TSource, TargetType>(memberName, criteria);
            memberPermission.Operations = operation;
            memberPermission.OperationState = state;
            securityStrategy.SecurityPermissions.Add(memberPermission);
            return memberPermission;
        }

        public static void SetPermissionPolicy(this ISecurityStrategy securityStrategy, PermissionPolicy policy) {
            OperationPermission operationPermission = new OperationPermission(SecurityOperation.NoAccess);
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
            securityStrategy.SecurityPermissions.Add(operationPermission);
        }
        public static object Set(this DbContext dbContext, Type type) {
            return MethodInfoSet.MakeGenericMethod(type).Invoke(null, new object[] { dbContext });

        }
        private static MethodInfo methodInfoSet;
        private static MethodInfo MethodInfoSet {
            get {
                if(methodInfoSet == null) {
                    methodInfoSet = typeof(PermissionObjectHelper).GetRuntimeMethods().First(p => p.Name == "SetGeneric");
                }
                return methodInfoSet;
            }
        }
        private static DbSet<TEntity> SetGeneric<TEntity>(DbContext dbContext) where TEntity : class {
            return dbContext.Set<TEntity>();
        }
    }

}
