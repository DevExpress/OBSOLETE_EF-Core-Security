using System;
using System.Linq.Expressions;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public interface IModificationСriterionService {
        Expression SetExpressionReadCriteriaFromSecurity(Expression sourceExpression, Type type);
    }
}