using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Remotion.Linq.Clauses.Expressions;
using DevExpress.EntityFramework.DbContextDataStore.Utility;
using Remotion.Linq;
using DevExpress.EntityFramework.SecurityDataStore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace DevExpress.EntityFramework.DbContextDataStore.Utility {
    class UpdateExpressionVisitor : ExpressionVisitor {
        private DbContext dbContext;
        private QueryContext queryContext;
        private ParameterExpression[] parameterExpressionArray;
        public UpdateExpressionVisitor(ParameterExpression[] parameterExpressionArray) {
            this.parameterExpressionArray = parameterExpressionArray;
        }
        public UpdateExpressionVisitor(ParameterExpression[] parameterExpressionArray, DbContext dbContext, QueryContext queryContext) {
            this.parameterExpressionArray = parameterExpressionArray;
            this.dbContext = dbContext;
            this.queryContext = queryContext;
        }
        protected override Expression VisitMember(MemberExpression node) {
            if(node.Expression is SubQueryExpression) {
                SubQueryExpression subQueryExpression = (SubQueryExpression)node.Expression;
                Expression updateSubQuery = this.Visit(subQueryExpression);
                MemberExpression updateMemberExpression = Expression.Property(updateSubQuery, node.Member.Name);
                return updateMemberExpression;
            }

            Type typeParamExpression = GetTypeParameter(node);
            foreach(var param in parameterExpressionArray) {
                if(param.Type == typeParamExpression) {
                    return recursionUpdateMemberExpression(node, param);
                }
            }
            throw new ArgumentException("param.Type != typeParamExpression");
        }
        private Type GetTypeParameter(Expression outerExpression) {
            if(outerExpression is MemberExpression) {
                MemberExpression memberExpression = (MemberExpression)outerExpression;
                return GetTypeParameter(memberExpression.Expression);
            }
            return outerExpression.Type;
        }
        private static MemberExpression recursionUpdateMemberExpression(MemberExpression memberExpression, ParameterExpression paramExpression) {
            Expression updateExpression = paramExpression;
            if(memberExpression.Expression is MemberExpression) {
                updateExpression = recursionUpdateMemberExpression((MemberExpression)memberExpression.Expression, paramExpression);
            }
            return memberExpression.Update(updateExpression);
        }
        protected override Expression VisitBinary(BinaryExpression node) {
            ExpressionType expressionType = node.NodeType;
            Expression leftExpression = this.Visit(node.Left);
            Expression rightExpression = this.Visit(node.Right);
            return Expression.MakeBinary(expressionType, leftExpression, rightExpression);
        }
        protected Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression node) {
            return parameterExpressionArray[0];
        }
        protected override Expression VisitParameter(ParameterExpression node) {
            var valueExpression = queryContext.ParameterValues[node.Name];
            return Expression.Constant(valueExpression);
        }
        protected Expression VisitSubQueryExpression(SubQueryExpression node) {
            var paramGeneric = parameterExpressionArray.Where(p => p.Type.GetGenericArguments().Count() == 1);
            Expression param = node.QueryModel.MainFromClause.FromExpression;
            if(param is QuerySourceReferenceExpression) {
                node.QueryModel.MainFromClause.FromExpression = paramGeneric.First();
            }
            QueryModelVisitor queryModelVisitor = new QueryModelVisitor(dbContext, queryContext);
            queryModelVisitor.VisitQueryModel(node.QueryModel);
            return queryModelVisitor.expression;
        }
        protected override Expression VisitNew(NewExpression node) {
            List<Expression> argListUpdate = new List<Expression>();
            foreach(var argExpression in node.Arguments) {
                Expression UpdateArgument = this.Visit(argExpression);
                argListUpdate.Add(UpdateArgument);
            }
            return node.Update(argListUpdate);
        }
        protected override Expression VisitMethodCall(MethodCallExpression node) {
            if(node.Arguments.Count == 0) {
                return node;
            }
            List<Expression> updateArguments = new List<Expression>();
            foreach(var arg in node.Arguments) {
                updateArguments.Add(this.Visit(arg));
            }
            
            return node.Update(null, updateArguments);
        }

        public override Expression Visit(Expression node) {
            if(node is QuerySourceReferenceExpression) {
                return VisitQuerySourceReferenceExpression((QuerySourceReferenceExpression)node);
            }

            if(node is SubQueryExpression) {
                return VisitSubQueryExpression((SubQueryExpression)node);
            }

            return base.Visit(node);
        }
        public static Expression Update(Expression outerExpression, ParameterExpression[] parameterExpressionArray, DbContext dbContext, QueryContext queryContext) {
            return new UpdateExpressionVisitor(parameterExpressionArray, dbContext, queryContext).Visit(outerExpression);
        }
    }
}
