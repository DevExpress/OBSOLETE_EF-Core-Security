using System;
using System.Linq.Expressions;

namespace DevExpress.EntityFramework.SecurityDataStore.Security {
    public interface IModificationСriterion {
        Expression GetDatabaseReadExpressionFromSecurity(Expression sourceExpression, Type type);
    }
}