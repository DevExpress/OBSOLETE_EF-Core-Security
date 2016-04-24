using DevExpress.EntityFramework.SecurityDataStore.Security;
using Microsoft.EntityFrameworkCore;

namespace DevExpress.EntityFramework.SecurityDataStore.Authorization {
    public interface IAuthorizationData {
        DbSet<SecurityUser> Users { get; set; }
        DbSet<SecurityRole> Roles { get; set; }
    }
}