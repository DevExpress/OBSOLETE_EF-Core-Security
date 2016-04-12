using DevExpress.EntityFramework.SecurityDataStore.Security;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public interface ISecurityObjectPermission : IPermission {
        string StringCriteria { get; set; }
        OperationState OperationState { get; set; }
        string StringType { get; set; }    
        SecurityRole SecurityRole { get; set; }
    }
}