using DevExpress.EntityFramework.SecurityDataStore.Security;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public interface IUserRole  {
        SecurityUser User { get; set; }       
        SecurityRole Role { get; set; }
    }
}