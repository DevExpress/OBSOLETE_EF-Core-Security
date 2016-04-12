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
        SecurityOperation Operations { get; set; }
    }
    public interface IPolicyPermission : IPermission {
    }
    public interface ITypePermission : IPermission {
        Type Type { get; set; }
        OperationState OperationState { get; set; }
    }
    public interface IObjectPermission : IPermission {
        Type Type { get; set; }
        OperationState OperationState { get; set; }
        LambdaExpression Criteria { get; set; }
    }
    public interface IMemberPermission : IPermission {
        Type Type { get; set; }
        OperationState OperationState { get; set; }
        LambdaExpression Criteria { get; set; }
        string MemberName { get; set; }
    }

    public interface ISecurityObjectsBuilder {
        IEnumerable<object> ProcessObjects(IEnumerable<object> objects);
    }
}
