using DevExpress.EntityFramework.SecurityDataStore.Authorization;
using DevExpress.EntityFramework.SecurityDataStore.Security;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EFCoreSecurityODataService.DataModel {
    public class PermissionsProviderContext : DbContext {
        public DbSet<SecurityRole> Roles { get; set; }
        public DbSet<SecurityUser> Users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseInMemoryDatabase();
        }
        public ISecurityUser GetUserByCredentials(string userName, string password) {
            return Users.
                Include(p => p.UserRoleCollection).ThenInclude(p => p.Role).ThenInclude(p => p.MemberPermissions).
                Include(p => p.UserRoleCollection).ThenInclude(p => p.Role).ThenInclude(p => p.OperationPermissions).
                Include(p => p.UserRoleCollection).ThenInclude(p => p.Role).ThenInclude(p => p.ObjectPermissions).
                Include(p => p.UserRoleCollection).ThenInclude(p => p.Role).ThenInclude(p => p.TypePermissions).
                FirstOrDefault(p => p.Name == userName && p.Password == password);
        }
        public IEnumerable<SecurityUser> GetUsers() {
            return Users.
                Include(p => p.UserRoleCollection).ThenInclude(p => p.Role).ThenInclude(p => p.MemberPermissions).
                Include(p => p.UserRoleCollection).ThenInclude(p => p.Role).ThenInclude(p => p.OperationPermissions).
                Include(p => p.UserRoleCollection).ThenInclude(p => p.Role).ThenInclude(p => p.ObjectPermissions).
                Include(p => p.UserRoleCollection).ThenInclude(p => p.Role).ThenInclude(p => p.TypePermissions);
        }
        public static IPermissionsProvider GetPermissionsProvider() {
            using(PermissionsProviderContext context = new PermissionsProviderContext()) {
                string userName = HttpContext.Current.User.Identity.Name;
                IPermissionsProvider permissionsProvider = context.GetUsers().FirstOrDefault(p => p.Name == userName);
                if(permissionsProvider == null) {
                    BasicAuthModule.CreateNotAuthorizedResponse(HttpContext.Current.ApplicationInstance, 401, 3, "Logon failed.");
                }
                return permissionsProvider;
            }
        }
    }
}