using DevExpress.EntityFramework.SecurityDataStore.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public enum OperationState { Allow, Deny }
    public interface IPermission {
        SecurityOperation Operations { get; }
    }
  
    public interface ITypePermission : IPermission {
        Type TargetObjectType { get; }
        OperationState OperationState { get; }
    }
    public interface IObjectPermission : IPermission {
        Type Type { get; }
        OperationState OperationState { get; }
        Expression Criteria { get; }
    }
    public interface IMemberPermission : IPermission {
        Type Type { get; }
        OperationState OperationState { get; }
        Expression Criteria { get; }
        string MemberName { get; }
    }
    public interface ISecurityStrategy {    
        bool IsGranted(Type type, SecurityOperation operation, object targetObject, string memberName);
        Expression SetExpressionCriteriaFromType(Expression sourceExpression, Type type);
        IList<IPermission> SecurityPermissions { get; }
        SecurityObjectsBuilder SecurityObjectsBuilder { get; }// replace interface
#if DebugTest
        SecurityDbContext GetDbContext();
#endif
    }
    public interface ISecurityObjectsBuilder {
        IEnumerable<object> ProcessObjects(IEnumerable<object> objects);
    }
}
