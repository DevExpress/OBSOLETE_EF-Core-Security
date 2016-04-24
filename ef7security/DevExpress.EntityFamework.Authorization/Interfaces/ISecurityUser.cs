using DevExpress.EntityFramework.SecurityDataStore;
using DevExpress.EntityFramework.SecurityDataStore.Security;
using System.Collections.Generic;

namespace DevExpress.EntityFramework.SecurityDataStore.Authorization {
    public interface ISecurityUser {
        string Name { get; set; }
        string Password { get; set; }
        ICollection<UserRole> UserRoleCollection { get; set; }
    }
}