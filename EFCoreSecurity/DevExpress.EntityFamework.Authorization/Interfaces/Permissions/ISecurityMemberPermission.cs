using DevExpress.EntityFramework.SecurityDataStore.Security;

namespace DevExpress.EntityFramework.SecurityDataStore.Authorization {
    public interface ISecurityMemberPermission : IPermission {
        string StringCriteria { get; set; }
        string MemberName { get; set; }        
        OperationState OperationState { get; set; }
        string StringType { get; set; }
        ISecurityRole SecurityRole { get; }
    }
}