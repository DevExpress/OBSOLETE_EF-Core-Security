using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using DevExpress.EntityFramework.SecurityDataStore.Utility;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using DevExpress.EntityFramework.SecurityDataStore.Security;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public enum ResultProcessOperation { NotContainTargetPermissions, Allow, Deny }
    public class PermissionProcessor : IPermissionProcessor {
        private IEnumerable<IPermission> permissions;
        private SecurityDbContext securityDbContext;
        public static bool AllowPermissionsPriority { get; set; } = false;
        public static SecurityOperation DefaultOperationsAllow { get; set; } = SecurityOperation.FullAccess;
        public bool IsGranted(Type type, SecurityOperation operation) {
            return IsGranted(type, operation, null);
        }
        public bool IsGranted(Type type, SecurityOperation operation, object targetObject) {
            return IsGranted(type, operation, targetObject, "");
        }
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
        public Expression SetExpressionReadCriteriaFromSecurity(Expression sourceExpression, Type type) {
            Expression loadExpression = null;
            if(permissions.Count() > 0) {
                ParameterExpression parameterExpression = Expression.Parameter(type, "p");

                bool allowReadLevelType = IsGranted(type, SecurityOperation.Read);

                if(allowReadLevelType) {
                    IEnumerable<IObjectPermission> objectsDenyExpression = GetObjectPermissions(type).Where(p => p.OperationState == OperationState.Deny && p.Operations.HasFlag(SecurityOperation.Read));
                    if(objectsDenyExpression.Count() > 0) {
                        IEnumerable<Expression> nativeExpression = GetNativeExpressions(objectsDenyExpression.Select(p => p.Criteria));
                        IEnumerable<Expression> inversionExpression = InversionExpressions(nativeExpression);
                        loadExpression = MergeExpressionsAsAnd(inversionExpression);
                    }
                }
                else {
                    IEnumerable<IObjectPermission> objectsAllowExpression = GetObjectPermissions(type).Where(p => p.OperationState == OperationState.Allow && p.Operations.HasFlag(SecurityOperation.Read));
                    if(objectsAllowExpression.Count() > 0) {
                        IEnumerable<Expression> nativeExpression = GetNativeExpressions(objectsAllowExpression.Select(p => p.Criteria));
                        loadExpression = MergeExpressionsAsOr(nativeExpression);
                        IEnumerable<IObjectPermission> objectsDenyExpression = GetObjectPermissions(type).Where(p => p.OperationState == OperationState.Deny && p.Operations.HasFlag(SecurityOperation.Read));
                        nativeExpression = GetNativeExpressions(objectsDenyExpression.Select(p => p.Criteria));
                        IEnumerable<Expression> inversionExpression = InversionExpressions(nativeExpression);
                        Expression denyObjectExpression = MergeExpressionsAsAnd(inversionExpression);
                        if(denyObjectExpression != null) {
                            loadExpression = Expression.And(loadExpression, denyObjectExpression);
                        }
                    }
                    IEnumerable<IMemberPermission> memberAllowExpression = GetMemberPermissions(type).Where(p => p.OperationState == OperationState.Allow && p.Operations.HasFlag(SecurityOperation.Read));
                    if(memberAllowExpression.Count() > 0) {
                        Expression membersExpression = null;
                        IEnumerable<IMemberPermission> memberDenyExpression = GetMemberPermissions(type).Where(p => p.OperationState == OperationState.Deny && p.Operations.HasFlag(SecurityOperation.Read));
                        IEnumerable<string> memberNames = memberAllowExpression.GroupBy(x => x.MemberName).Select(g => g.First().MemberName);
                        foreach(string memberName in memberNames) {
                            Expression memberExpression;
                            IEnumerable<IMemberPermission> currentMemberAllowExpressions = memberAllowExpression.Where(p => p.MemberName == memberName);
                            IEnumerable<Expression> nativeExpression = GetNativeExpressions(currentMemberAllowExpressions.Select(p => p.Criteria));
                            /*if(loadExpression == null) {*/
                            memberExpression = MergeExpressionsAsOr(nativeExpression);
                            //}
                            //else {
                            //    loadExpression = Expression.Or(loadExpression, MergeExpressionsAsOr(nativeExpression));
                            //}
                            IEnumerable<IMemberPermission> currentMemberDenyExpressions = memberDenyExpression.Where(p => p.MemberName == memberName);
                            if(currentMemberDenyExpressions.Count() > 0) {
                                nativeExpression = GetNativeExpressions(currentMemberDenyExpressions.Select(p => p.Criteria));
                                IEnumerable<Expression> inversionExpression = InversionExpressions(nativeExpression);
                                Expression denyLoadObjectExpression = MergeExpressionsAsAnd(inversionExpression);
                                if(denyLoadObjectExpression != null) {
                                    memberExpression = Expression.And(memberExpression, denyLoadObjectExpression);
                                }
                            }
                            if(membersExpression == null) {
                                membersExpression = memberExpression;
                            }
                            else {
                                membersExpression = Expression.Or(membersExpression, memberExpression);
                            }
                        }
                        if(membersExpression != null) {
                            if(loadExpression == null) {
                                loadExpression = membersExpression;
                            }
                            else {
                                loadExpression = Expression.Or(loadExpression, membersExpression);
                            }
                        }
                    }
                    if(loadExpression == null) {
                        loadExpression = Expression.Constant(false);
                    }

                }
                if(loadExpression != null) {
                    UpdateParameterVisitor updateParametrVisitor = new UpdateParameterVisitor(securityDbContext.realDbContext, parameterExpression);
                    loadExpression = updateParametrVisitor.Visit(loadExpression);
                    MethodInfo miWhere = UtilityHelper.GetMethods("Where", type, 1).First().MakeGenericMethod(type);
                    Expression whereLamda = Expression.Lambda(loadExpression, parameterExpression);
                    loadExpression = Expression.Call(miWhere, new[] { sourceExpression, whereLamda });
                }
                else {
                    loadExpression = sourceExpression;
                }
            }
            else {
                loadExpression = sourceExpression;
            }
            return loadExpression;
        }      
        private bool IsSecuredType(Type type) {
            bool result = false;
            IEntityType entityType = securityDbContext.realDbContext.Model.FindEntityType(type);
            if(entityType != null) {
                result = true;
            }
            return result;
        }
        private bool IsGrantedByOperation(SecurityOperation operation) {
            bool result;
            IEnumerable<IPolicyPermission> operationPermissions = permissions.OfType<IPolicyPermission>();
            if(operationPermissions.Count() != 0) {
                result = operationPermissions.Any(p => p.Operations.HasFlag(operation));
            }
            else {
                result = DefaultOperationsAllow.HasFlag(operation);
            }
            return result;
        }
        private ResultProcessOperation IsGrantedByType(Type type, SecurityOperation operation) {
            ResultProcessOperation result;
            IEnumerable<ITypePermission> typePermissions = permissions.OfType<ITypePermission>()
                .Where(p => p.Type == type && p.Operations.HasFlag(operation));
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
            IEntityType entityType = securityDbContext.realDbContext.Model.FindEntityType(targetObject.GetType());
            IEnumerable<INavigation> navigationPropertys = entityType.GetNavigations();

            foreach(var property in targetObject.GetType().GetTypeInfo().DeclaredProperties) {
                if(property.GetGetMethod().IsStatic || navigationPropertys.Any(p => p.Name == property.Name))
                    continue;
                string propertyName = property.Name;
                IProperty propertyMetadata = securityDbContext.realDbContext.Entry(targetObject).Metadata.GetProperties().FirstOrDefault(p => p.Name == propertyName);
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
        private Expression MergeExpressionsAsOr(IEnumerable<Expression> nativeExpression) {
            Expression mergeOrExpression = null;
            foreach(Expression expression in nativeExpression) {
                if(mergeOrExpression != null) {
                    mergeOrExpression = Expression.Or(mergeOrExpression, expression);
                }
                else {
                    mergeOrExpression = expression;
                }
            }
            return mergeOrExpression;
        }
        private IEnumerable<Expression> GetNativeExpressions(IEnumerable<Expression> expressionWithLamda) {
            List<Expression> nativeExpressions = new List<Expression>();
            foreach(Expression expression in expressionWithLamda) {
                LambdaExpression lambdaExpression = expression as LambdaExpression;
                nativeExpressions.Add(lambdaExpression.Body);
            }
            return nativeExpressions;
        }
        private Expression MergeExpressionsAsAnd(IEnumerable<Expression> inversionExpression) {
            Expression mergeOrExpression = null;
            foreach(Expression expression in inversionExpression) {
                if(mergeOrExpression != null) {
                    mergeOrExpression = Expression.And(mergeOrExpression, expression);
                }
                else {
                    mergeOrExpression = expression;
                }
            }
            return mergeOrExpression;
        }
        private IEnumerable<Expression> InversionExpressions(IEnumerable<Expression> objectsDenyExpression) {
            List<Expression> inversionList = new List<Expression>();
            foreach(Expression expression in objectsDenyExpression) {
                inversionList.Add(Expression.Not(expression));
            }
            return inversionList;
        }
        private IEnumerable<IObjectPermission> GetObjectPermissions(Type type) {
            return permissions.OfType<IObjectPermission>().Where(p => p.Type == type);
        }
        private ResultProcessOperation IsGrantedByObject(Type type, SecurityOperation operation, object targetObject) {
            ResultProcessOperation result = ResultProcessOperation.NotContainTargetPermissions;
            IEnumerable<IObjectPermission> typeOperations = GetObjectPermissions(type);
            IEnumerable<IObjectPermission> objectPermissions = typeOperations.Where(p => p.Operations.HasFlag(operation));

            List<bool> objectOperationStatePermissions = new List<bool>();

            foreach(IObjectPermission objectPermission in objectPermissions) {
                OperationState operationState = objectPermission.OperationState;
                LambdaExpression criteriaExpression = objectPermission.GetType().GetProperty("Criteria").GetValue(objectPermission, null) as LambdaExpression;
                bool permissionResult = GetPermissionCriteriaResult(operationState, type, criteriaExpression, targetObject);
                if(permissionResult) {
                    if(operationState == OperationState.Allow) {
                        objectOperationStatePermissions.Add(true);
                    }
                    else {
                        objectOperationStatePermissions.Add(false);
                    }
                }

            }

            if(objectOperationStatePermissions.Count() != 0) {
                if(AllowPermissionsPriority) {
                    result = objectOperationStatePermissions.Any(p => p == true)
                        ? ResultProcessOperation.Allow : ResultProcessOperation.Deny;
                }
                else {
                    result = objectOperationStatePermissions.Any(p => p == false)
                        ? ResultProcessOperation.Deny : ResultProcessOperation.Allow;
                }
            }
            else {
                result = ResultProcessOperation.NotContainTargetPermissions;
            }
            return result;
        }
        private IEnumerable<IMemberPermission> GetMemberPermissions(Type type) {
            return permissions.OfType<IMemberPermission>().Where(p => p.Type == type);
        }
        private bool GetPermissionCriteriaResult(OperationState operationState, Type type, LambdaExpression criteria, object targetObject) {
            return (bool)criteria.Compile().DynamicInvoke(new[] { securityDbContext.realDbContext, targetObject });
        }
        private ResultProcessOperation IsGrantedByMember(Type type, SecurityOperation operation, object targetObject, string memberName) {
            ResultProcessOperation result;
            var memberPermissions = GetMemberPermissions(type).Where(p => p.Operations.HasFlag(operation));

            List<bool> memberOperationStatePermissions = new List<bool>();

            foreach(IMemberPermission memberPermission in memberPermissions) {
                string currentMemberName = memberPermission.GetType().GetProperty("MemberName").GetValue(memberPermission, null) as string;
                if(memberName != currentMemberName)
                    continue;
                OperationState operationState = memberPermission.OperationState;
                LambdaExpression criteriaExpression = memberPermission.GetType().GetProperty("Criteria").GetValue(memberPermission, null) as LambdaExpression;
                bool permissionResult = GetPermissionCriteriaResult(operationState, type, criteriaExpression, targetObject);
                if(permissionResult) {
                    if(operationState == OperationState.Allow) {
                        memberOperationStatePermissions.Add(true);
                    }
                    else {
                        memberOperationStatePermissions.Add(false);
                    }
                }
            }

            if(memberOperationStatePermissions.Count() != 0) {
                if(AllowPermissionsPriority) {
                    result = memberOperationStatePermissions.Any(p => p == true)
                        ? ResultProcessOperation.Allow : ResultProcessOperation.Deny;
                }
                else {
                    result = memberOperationStatePermissions.Any(p => p == false)
                        ? ResultProcessOperation.Deny : ResultProcessOperation.Allow;
                }
            }
            else {
                result = ResultProcessOperation.NotContainTargetPermissions;
            }
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
        public PermissionProcessor(IEnumerable<IPermission> permissions, SecurityDbContext securityDbContext) {
            this.permissions = permissions;
            this.securityDbContext = securityDbContext;
        }
    }
}