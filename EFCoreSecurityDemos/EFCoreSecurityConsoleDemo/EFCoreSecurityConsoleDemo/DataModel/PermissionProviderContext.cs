using DevExpress.EntityFramework.SecurityDataStore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreSecurityConsoleDemo.DataModel {
    public class PermissionProviderContext : DbContext {
        public DbSet<SecurityRole> Roles { get; set; }
        public DbSet<SecurityUser> Users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseInMemoryDatabase();
        }
        public ISecurityUser GetUserByCredentials(string userName, string password) {
            return this.Users.
                            Include(p => p.UserRoleCollection).ThenInclude(p => p.Role).ThenInclude(p => p.MemberPermissions).
                            Include(p => p.UserRoleCollection).ThenInclude(p => p.Role).ThenInclude(p => p.OperationPermissions).
                            Include(p => p.UserRoleCollection).ThenInclude(p => p.Role).ThenInclude(p => p.ObjectPermissions).
                            Include(p => p.UserRoleCollection).ThenInclude(p => p.Role).ThenInclude(p => p.TypePermissions).
                            FirstOrDefault(p => p.Name == userName && p.Password == password);
        }
    }
}
