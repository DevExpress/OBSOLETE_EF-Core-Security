using DevExpress.EntityFramework.SecurityDataStore.Utility;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Security {
    public class SecurityExpressionBuilder : ISecurityExpressionBuilder {
        private IPermissionProcessor permissionProcessor;
        private IPermissionsProvider permissionsProvider;
        private DbContext realDbContext;
        public SecurityExpressionBuilder(IPermissionProcessor permissionProcessor, IPermissionsProvider permissionsProvider, DbContext securityDbContext) {
            this.permissionProcessor = permissionProcessor;
            this.permissionsProvider = permissionsProvider;
            realDbContext = ((BaseSecurityDbContext)securityDbContext).RealDbContext;
        }
        public Expression GetDatabaseReadExpressionFromSecurity(Expression sourceExpression, Type type) {
            Expression loadExpression = null;

            if(permissionsProvider.GetPermissions().Count() > 0) {
                ParameterExpression parameterExpression = Expression.Parameter(type, "p");
                bool allowReadLevelType = permissionProcessor.IsGranted(type, SecurityOperation.Read);
                if(allowReadLevelType) {
                    IEnumerable<IObjectPermission> objectsDenyExpression = GetObjectPermissions(type).Where(p => p.OperationState == OperationState.Deny && p.Operations.HasFlag(SecurityOperation.Read));
                    if(objectsDenyExpression.Count() > 0) {
                        IEnumerable<Expression> originalExpressions = GetBodiesOfLambdaExpressions(objectsDenyExpression.Select(p => p.Criteria));
                        IEnumerable<Expression> invertedExpressions = GetInvertedExpressions(originalExpressions);
                        loadExpression = GetAndMergedExpression(invertedExpressions);
                    }
                }
                else {
                    IEnumerable<IObjectPermission> objectsAllowExpression = GetObjectPermissions(type).Where(p => p.OperationState == OperationState.Allow && p.Operations.HasFlag(SecurityOperation.Read));
                    if(objectsAllowExpression.Count() > 0) {
                        IEnumerable<Expression> nativeExpression = GetBodiesOfLambdaExpressions(objectsAllowExpression.Select(p => p.Criteria));
                        loadExpression = GetOrMergedExpression(nativeExpression);
                        IEnumerable<IObjectPermission> objectsDenyExpression = GetObjectPermissions(type).Where(p => p.OperationState == OperationState.Deny && p.Operations.HasFlag(SecurityOperation.Read));
                        nativeExpression = GetBodiesOfLambdaExpressions(objectsDenyExpression.Select(p => p.Criteria));
                        IEnumerable<Expression> invertedExpressions = GetInvertedExpressions(nativeExpression);
                        Expression denyObjectExpression = GetAndMergedExpression(invertedExpressions);
                        if(denyObjectExpression != null) {
                            loadExpression = Expression.AndAlso(loadExpression, denyObjectExpression);
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
                            IEnumerable<Expression> nativeExpression = GetBodiesOfLambdaExpressions(currentMemberAllowExpressions.Select(p => p.Criteria));
                            memberExpression = GetOrMergedExpression(nativeExpression);
                            IEnumerable<IMemberPermission> currentMemberDenyExpressions = memberDenyExpression.Where(p => p.MemberName == memberName);
                            if(currentMemberDenyExpressions.Count() > 0) {
                                nativeExpression = GetBodiesOfLambdaExpressions(currentMemberDenyExpressions.Select(p => p.Criteria));
                                IEnumerable<Expression> inversionExpression = GetInvertedExpressions(nativeExpression);
                                Expression denyLoadObjectExpression = GetAndMergedExpression(inversionExpression);
                                if(denyLoadObjectExpression != null) {
                                    memberExpression = Expression.AndAlso(memberExpression, denyLoadObjectExpression);
                                }
                            }
                            if(membersExpression == null) {
                                membersExpression = memberExpression;
                            }
                            else {
                                membersExpression = Expression.OrElse(membersExpression, memberExpression);
                            }
                        }
                        if(membersExpression != null) {
                            if(loadExpression == null) {
                                loadExpression = membersExpression;
                            }
                            else {
                                loadExpression = Expression.OrElse(loadExpression, membersExpression);
                            }
                        }
                    }
                    if(loadExpression == null) {
                        loadExpression = Expression.Constant(false);
                    }

                }
                if(loadExpression != null) {
                    UpdateParameterVisitor updateParameterVisitor = new UpdateParameterVisitor(realDbContext, parameterExpression);
                    loadExpression = updateParameterVisitor.Visit(loadExpression);

                    //loadExpression = Expression.Condition(
                    //    loadExpression,
                    //    Expression.Constant(true, typeof(bool)),
                    //    Expression.Constant(false, typeof(bool)));

                    MethodInfo whereMethodInfo = UtilityHelper.GetMethods("Where", type, 1).First().MakeGenericMethod(type);
                    Expression whereLambda = Expression.Lambda(loadExpression, parameterExpression);
                    loadExpression = Expression.Call(whereMethodInfo, new[] { sourceExpression, whereLambda });
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
            return permissionsProvider.GetPermissions().OfType<IObjectPermission>().Where(p => p.Type == type);
        }
        private IEnumerable<Expression> GetBodiesOfLambdaExpressions(IEnumerable<Expression> expressionWithLamda) {
            List<Expression> expressionsBodies = new List<Expression>();
            foreach(Expression expression in expressionWithLamda) {
                LambdaExpression lambdaExpression = expression as LambdaExpression;
                expressionsBodies.Add(lambdaExpression.Body);
            }
            return expressionsBodies;
        }
        private IEnumerable<Expression> GetInvertedExpressions(IEnumerable<Expression> objectsDenyExpression) {
            List<Expression> invertedExpressions = new List<Expression>();
            foreach(Expression expression in objectsDenyExpression) {
                invertedExpressions.Add(Expression.Not(expression));
            }
            return invertedExpressions;
        }
        private Expression GetMergedExpression(IEnumerable<Expression> expressions, Func<Expression, Expression, BinaryExpression> merger) {
            Expression resultExpression = null;
            foreach (Expression expression in expressions) {
                if (resultExpression != null)
                    resultExpression = merger(resultExpression, expression);
                else
                    resultExpression = expression;
            }
            return resultExpression;
        } 
        private Expression GetAndMergedExpression(IEnumerable<Expression> expressions) {
            return GetMergedExpression(expressions, Expression.AndAlso);
        }
        private Expression GetOrMergedExpression(IEnumerable<Expression> expressions) {
            return GetMergedExpression(expressions, Expression.OrElse);
        }
        private IEnumerable<IMemberPermission> GetMemberPermissions(Type type) {
            return permissionsProvider.GetPermissions().OfType<IMemberPermission>().Where(p => p.Type == type);
        }
    }
}
