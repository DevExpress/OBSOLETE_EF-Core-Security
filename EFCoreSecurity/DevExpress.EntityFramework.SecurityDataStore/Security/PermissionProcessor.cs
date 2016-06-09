using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using DevExpress.EntityFramework.SecurityDataStore.Utility;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using DevExpress.EntityFramework.SecurityDataStore.Security;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public enum ResultProcessOperation { NotContainTargetPermissions, Allow, Deny }
    public class PermissionProcessor : IPermissionProcessor {
        private IPermissionsProvider permissionsProvider;
        private BaseSecurityDbContext securityDbContext;
        public static bool AllowPermissionsPriority { get; set; } = false;
        public static SecurityOperation DefaultOperationsAllow { get; set; } = SecurityOperation.FullAccess;
        public bool IsGranted(Type type, SecurityOperation operation, object targetObject, string memberName) {
            ResultProcessOperation result = ResultProcessOperation.NotContainTargetPermissions;
            if(!IsSecuredType(type)) {
                result = ResultProcessOperation.Allow;
            }
            if(targetObject != null && !string.IsNullOrEmpty(memberName)) {
                result = IsGrantedByMember(type, operation, targetObject, memberName);
            }
            if(result == ResultProcessOperation.NotContainTargetPermissions && targetObject != null) {
                result = IsGrantedByObject(type, operation, targetObject);
            }
            if(result == ResultProcessOperation.NotContainTargetPermissions) {
                result = IsGrantedByType(type, operation);
            }
            if(result == ResultProcessOperation.NotContainTargetPermissions) {
                result = IsGrantedByOperation(operation) ? ResultProcessOperation.Allow : ResultProcessOperation.Deny;
                if(result == ResultProcessOperation.Deny && targetObject != null && string.IsNullOrEmpty(memberName)) {
                    result = IsAnyMemberGranted(type, operation, targetObject);
                }
            }
            if(result == ResultProcessOperation.NotContainTargetPermissions) {
                throw new ArgumentOutOfRangeException();
            }
            return (result == ResultProcessOperation.Allow) ? true : false;
        }
        private bool IsSecuredType(Type type) {
            bool result = false;
            IEntityType entityType = securityDbContext.RealDbContext.Model.FindEntityType(type);
            if(entityType != null) {
                result = true;
            }
            return result;
        }
        private bool IsGrantedByOperation(SecurityOperation operation) {
            bool result;
            IEnumerable<IPolicyPermission> operationPermissions = permissionsProvider.GetPermissions().OfType<IPolicyPermission>();
            if(operationPermissions.Count() != 0) {
                result = operationPermissions.Any(p => p.Operations.HasFlag(operation));
            }
            else {
                result = DefaultOperationsAllow.HasFlag(operation);
            }
            return result;
        }
        private IEnumerable<ITypePermission> GetTypePermissions(Type type) {
            return permissionsProvider.GetPermissions().OfType<ITypePermission>().Where(p => p.Type == type);
        }
        private ResultProcessOperation IsGrantedByType(Type type, SecurityOperation operation) {
            ResultProcessOperation result;
            IEnumerable<ITypePermission> typePermissions = GetTypePermissions(type).Where(p => p.Operations.HasFlag(operation));
            if(typePermissions.Count() != 0) {
                if(AllowPermissionsPriority) {
                    result = typePermissions.Any(p => p.OperationState == OperationState.Allow)
                        ? ResultProcessOperation.Allow : ResultProcessOperation.Deny;
                }
                else {
                    result = typePermissions.Any(p => p.OperationState == OperationState.Deny)
                        ? ResultProcessOperation.Deny : ResultProcessOperation.Allow;
                }
            }
            else {
                result = ResultProcessOperation.NotContainTargetPermissions;
            }
            return result;
        }
        private ResultProcessOperation IsAnyMemberGranted(Type type, SecurityOperation operation, object targetObject) {
            ResultProcessOperation result = ResultProcessOperation.Deny;
            IEntityType entityType = securityDbContext.RealDbContext.Model.FindEntityType(targetObject.GetType());
            IEnumerable<INavigation> navigationPropertys = entityType.GetNavigations();

            foreach(var property in targetObject.GetType().GetTypeInfo().DeclaredProperties) {
                if(property.GetGetMethod().IsStatic || navigationPropertys.Any(p => p.Name == property.Name))
                    continue;
                string propertyName = property.Name;
                IProperty propertyMetadata = securityDbContext.RealDbContext.Entry(targetObject).Metadata.GetProperties().FirstOrDefault(p => p.Name == propertyName);
                if(propertyMetadata == null || propertyMetadata.IsKey()) {
                    continue;
                }

                bool isGranted = IsGranted(targetObject.GetType(), operation, targetObject, propertyName);

                if(isGranted) {
                    result = ResultProcessOperation.Allow;
                    break;
                }
            }
            if(result == ResultProcessOperation.Deny) {
                foreach(INavigation navigationProperty in navigationPropertys) {
                    bool isGranted = IsGranted(targetObject.GetType(), operation, targetObject, navigationProperty.Name);
                    if(isGranted) {
                        result = ResultProcessOperation.Allow;
                        break;
                    }
                }
            }
            return result;
        }
        private IEnumerable<IObjectPermission> GetObjectPermissions(Type type) {
            return permissionsProvider.GetPermissions().OfType<IObjectPermission>().Where(p => p.Type == type);
        }
        private ResultProcessOperation IsGrantedByObject(Type type, SecurityOperation operation, object targetObject) {
            ResultProcessOperation result = ResultProcessOperation.NotContainTargetPermissions;
            IEnumerable<IObjectPermission> objectPermissions = GetObjectPermissions(type).Where(p => p.Operations.HasFlag(operation));
            List<bool> objectPermissionsStates = new List<bool>();
            foreach(IObjectPermission objectPermission in objectPermissions) {
                OperationState operationState = objectPermission.OperationState;
                LambdaExpression criteriaExpression = objectPermission.GetType().GetProperty("Criteria").GetValue(objectPermission, null) as LambdaExpression;
                bool permissionResult = (bool)criteriaExpression.Compile().DynamicInvoke(new[] { securityDbContext.RealDbContext, targetObject });
                if(permissionResult) {
                    if(operationState == OperationState.Allow) {
                        objectPermissionsStates.Add(true);
                    }
                    else {
                        objectPermissionsStates.Add(false);
                    }
                }
            }
            result = MergePermissionsStates(objectPermissionsStates);
            return result;
        }
        private ResultProcessOperation MergePermissionsStates(List<bool> permissionsStates) {
            ResultProcessOperation result;
            if(permissionsStates.Count() != 0) {
                if(AllowPermissionsPriority) {
                    result = permissionsStates.Any(p => p == true)
                        ? ResultProcessOperation.Allow : ResultProcessOperation.Deny;
                }
                else {
                    result = permissionsStates.Any(p => p == false)
                        ? ResultProcessOperation.Deny : ResultProcessOperation.Allow;
                }
            }
            else {
                result = ResultProcessOperation.NotContainTargetPermissions;
            }

            return result;
        }
        private IEnumerable<IMemberPermission> GetMemberPermissions(Type type) {
            return permissionsProvider.GetPermissions().OfType<IMemberPermission>().Where(p => p.Type == type);
        }
        private ResultProcessOperation IsGrantedByMember(Type type, SecurityOperation operation, object targetObject, string memberName) {
            ResultProcessOperation result;
            var memberPermissions = GetMemberPermissions(type).Where(p => p.Operations.HasFlag(operation));
            List<bool> memberPermissionsStates = new List<bool>();
            foreach(IMemberPermission memberPermission in memberPermissions) {
                string currentMemberName = memberPermission.GetType().GetProperty("MemberName").GetValue(memberPermission, null) as string;
                if(memberName != currentMemberName)
                    continue;
                OperationState operationState = memberPermission.OperationState;
                LambdaExpression criteriaExpression = memberPermission.GetType().GetProperty("Criteria").GetValue(memberPermission, null) as LambdaExpression;
                bool permissionResult = (bool)criteriaExpression.Compile().DynamicInvoke(new[] { securityDbContext.RealDbContext, targetObject });
                if(permissionResult) {
                    if(operationState == OperationState.Allow) {
                        memberPermissionsStates.Add(true);
                    }
                    else {
                        memberPermissionsStates.Add(false);
                    }
                }
            }
            result = MergePermissionsStates(memberPermissionsStates);
            return result;
        }
        public IEnumerable<string> GetReadOnlyMembers(Type type) {
            var memberPermissions = GetMemberPermissions(type);
            var constantExpressions = memberPermissions.Where(p =>
            p.Operations == SecurityOperation.Write &&
            p.OperationState == OperationState.Deny &&
            p.Criteria.Body is ConstantExpression &&
            ((ConstantExpression)p.Criteria.Body).Value.Equals(true));
            return constantExpressions.SelectMany(p => p.MemberName.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
        }
        public PermissionProcessor(IPermissionsProvider permissionsProvider, DbContext securityDbContext) {
            this.permissionsProvider = permissionsProvider;
            this.securityDbContext = (BaseSecurityDbContext)securityDbContext;
        }
    }
}