using System;
using System.Linq.Expressions;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public interface IModificationСriterionService {
        Expression GetDatabaseReadExpressionFromSecurity(Expression sourceExpression, Type type);
    }
}