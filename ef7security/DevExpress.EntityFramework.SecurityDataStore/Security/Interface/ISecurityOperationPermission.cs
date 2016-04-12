using DevExpress.EntityFramework.SecurityDataStore.Security;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public interface ISecurityPolicyPermission : IPolicyPermission {
        SecurityRole SecurityRole { get; set; }
    }
}