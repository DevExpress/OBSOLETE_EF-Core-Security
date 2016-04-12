using DevExpress.EntityFramework.SecurityDataStore.Security;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public interface ISecurityPermission : IPermission {
        SecurityRole SecurityRole { get; set; }
    }
}