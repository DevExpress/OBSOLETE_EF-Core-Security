using DevExpress.EntityFramework.SecurityDataStore.Security;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public interface ISecurityTypePermission : IPermission{
        OperationState OperationState { get; set; }
        string StringType { get; set; }     
        SecurityRole SecurityRole { get; set; }
    }
}