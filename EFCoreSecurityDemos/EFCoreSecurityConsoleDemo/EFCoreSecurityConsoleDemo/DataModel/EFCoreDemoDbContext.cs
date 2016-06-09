using DevExpress.EntityFramework.SecurityDataStore;
using DevExpress.EntityFramework.SecurityDataStore.Security;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EFCoreSecurityConsoleDemo.DataModel {
    public class EFCoreDemoDbContext : SecurityDbContext  {
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
