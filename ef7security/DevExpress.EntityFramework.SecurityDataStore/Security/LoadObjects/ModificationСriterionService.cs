using DevExpress.EntityFramework.SecurityDataStore.Utility;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Security.Services {
    public class ModificationСriterionService : IModificationСriterionService {
        private IPermissionProcessor permissionProcessor;
        private IEnumerable<IPermission> securityPermissions;
        private DbContext realDbContext;
        public ModificationСriterionService(IPermissionProcessor permissionProcessor, IEnumerable<IPermission> securityPermissions, DbContext realDbContext) {
            this.permissionProcessor = permissionProcessor;
            this.securityPermissions = securityPermissions;
            this.realDbContext = realDbContext;
        }
        public Expression SetExpressionReadCriteriaFromSecurity(Expression sourceExpression, Type type) {
            Expression loadExpression = null;
            if(securityPermissions.Count() > 0) {
                ParameterExpression parameterExpression = Expression.Parameter(type, "p");
                bool allowReadLevelType = permissionProcessor.IsGranted(type, SecurityOperation.Read, null, "");
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
                    UpdateParametrVisitor updateParametrVisitor = new UpdateParametrVisitor(realDbContext, parameterExpression);
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
        private IEnumerable<IObjectPermission> GetObjectPermissions(Type type) {
            return securityPermissions.OfType<IObjectPermission>().Where(p => p.Type == type);
        }
        private IEnumerable<Expression> GetNativeExpressions(IEnumerable<Expression> expressionWithLamda) {
            List<Expression> nativeExpressions = new List<Expression>();
            foreach(Expression expression in expressionWithLamda) {
                LambdaExpression lambdaExpression = expression as LambdaExpression;
                nativeExpressions.Add(lambdaExpression.Body);
            }
            return nativeExpressions;
        }
        private IEnumerable<Expression> InversionExpressions(IEnumerable<Expression> objectsDenyExpression) {
            List<Expression> inversionList = new List<Expression>();
            foreach(Expression expression in objectsDenyExpression) {
                inversionList.Add(Expression.Not(expression));
            }
            return inversionList;
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
        private IEnumerable<IMemberPermission> GetMemberPermissions(Type type) {
            return securityPermissions.OfType<IMemberPermission>().Where(p => p.Type == type);
        }
    }
}
