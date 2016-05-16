using DevExpress.EntityFramework.SecurityDataStore.Security;

namespace DevExpress.EntityFramework.SecurityDataStore.Authorization {
    public interface ISecurityTypePermission : IPermission{
        OperationState OperationState { get; set; }
        string StringType { get; set; }     
        ISecurityRole SecurityRole { get; }
    }
}