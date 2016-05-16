using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Utility {
    public class UpdateParameterVisitor : ExpressionVisitor {
        private object dbContext;
        private ParameterExpression parameters;      

        public UpdateParameterVisitor(object dbContext, ParameterExpression parameters) {
            this.dbContext = dbContext;
            this.parameters = parameters;
        }
        protected override Expression VisitParameter(ParameterExpression node) {
            Expression resultExpression = node;
            if(parameters.Type == node.Type) {
                resultExpression = parameters;
            }
            else {
                if(node.Type.Equals(dbContext.GetType())) {
                    resultExpression = Expression.Constant(dbContext);
                }
            }
            return resultExpression;
        }     
        public override Expression Visit(Expression node) {
            return base.Visit(node); 
        }
    }
}
