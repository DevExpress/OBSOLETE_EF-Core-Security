using DevExpress.EntityFramework.Authorization;
using DevExpress.EntityFramework.SecurityDataStore;
using DevExpress.EntityFramework.SecurityDataStore.Security;
using Microsoft.EntityFrameworkCore;

namespace EFCoreSecurityODataService.DataModel {
    public class EFCoreDemoDbContext : SecurityDbContext {
        public EFCoreDemoDbContext() : base() {}
        public EFCoreDemoDbContext(IPermissionsProvider permissionsProvider) {
            PermissionsContainer.AddPermissions(permissionsProvider.GetPermissions());
        }
        public DbSet<Contact> Contacts { get; set; }
        protected override void OnSecuredConfiguring(DbContextOptionsBuilder optionsBuilder) {
            base.OnSecuredConfiguring(optionsBuilder);
            optionsBuilder.UseInMemoryDatabase();
        }
    }
}
