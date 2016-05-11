using DevExpress.EntityFramework.SecurityDataStore.Security;

namespace DevExpress.EntityFramework.SecurityDataStore.Authorization {
    public interface ISecurityPolicyPermission : IPolicyPermission {
        ISecurityRole SecurityRole { get; }
    }
}