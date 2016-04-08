using DevExpress.EntityFramework.SecurityDataStore.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public enum OperationState { Allow, Deny }
    public enum PermissionPolicy { AllowAllByDefault, ReadOnlyAllByDefault, DenyAllByDefault }
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

    public interface ISecurityObjectsBuilder {
        IEnumerable<object> ProcessObjects(IEnumerable<object> objects);
    }
}
