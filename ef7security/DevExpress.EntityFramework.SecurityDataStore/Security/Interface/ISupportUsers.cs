using Microsoft.EntityFrameworkCore;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public interface ISupportUsers {
        DbSet<ISecurityUser> Users { get; set; }
        DbSet<ISecurityRole> Roles { get; set; }
    }
}