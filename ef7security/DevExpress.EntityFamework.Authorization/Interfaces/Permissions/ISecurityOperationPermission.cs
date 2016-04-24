using DevExpress.EntityFramework.SecurityDataStore.Security;

namespace DevExpress.EntityFramework.SecurityDataStore.Authorization {
    public interface ISecurityPolicyPermission : IPolicyPermission {
        SecurityRole SecurityRole { get; set; }
    }
}