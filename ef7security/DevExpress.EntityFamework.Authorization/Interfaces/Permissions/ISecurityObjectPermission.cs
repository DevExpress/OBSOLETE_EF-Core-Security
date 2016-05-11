using DevExpress.EntityFramework.SecurityDataStore.Security;

namespace DevExpress.EntityFramework.SecurityDataStore.Authorization {
    public interface ISecurityObjectPermission : IPermission {
        string StringCriteria { get; set; }
        OperationState OperationState { get; set; }
        string StringType { get; set; }    
        ISecurityRole SecurityRole { get; }
    }
}