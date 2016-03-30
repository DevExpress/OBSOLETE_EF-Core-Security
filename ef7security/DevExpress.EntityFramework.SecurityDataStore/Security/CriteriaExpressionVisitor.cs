using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore {
    class CriteriaExpressionVisitor<TSource, TargetType> : ExpressionVisitor {
        private TSource dbContext;
        private ParameterExpression parameter;
        public Expression Modify(Expression expression, ParameterExpression parameter, TSource dbContext) {
            this.dbContext = dbContext;
            this.parameter = parameter;
            var newExpression = Visit(expression);
            var result = Expression.Lambda<Func<TargetType, bool>>(newExpression, parameter);
            return result;
        }
        protected override Expression VisitLambda<TIn>(Expression<TIn> node) {
            return Expression.Lambda<Func<TargetType, bool>>(Visit(node.Body), parameter);
        }
        protected override Expression VisitParameter(ParameterExpression node) {
            if(node.Type == typeof(TSource)) {
                return Expression.Constant(dbContext, typeof(TSource));
            }
            else if(node.Type == typeof(TargetType)) {
                return node;
            }
            throw new InvalidOperationException();
        }
    }
}
