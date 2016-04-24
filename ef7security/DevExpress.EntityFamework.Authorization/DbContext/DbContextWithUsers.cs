using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;

namespace DevExpress.EntityFramework.SecurityDataStore.Authorization {
    public abstract class DbContextUsersBase : SecurityDbContext, ISupportUsers {
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<UserRole>().HasOne(p => p.Role).WithMany(p => p.Roles).HasForeignKey(p => p.RoleID);
            modelBuilder.Entity<UserRole>().HasOne(p => p.User).WithMany(p => p.UserRoleCollection).HasForeignKey(p => p.UserID);
            modelBuilder.Entity<SecurityPolicyPermission>().HasOne(p => p.SecurityRole).WithMany(p => p.OperationPermissions).HasForeignKey(p=>p.SecurityRoleID);
            modelBuilder.Entity<SecurityTypePermission>().HasOne(p => p.SecurityRole).WithMany(p => p.TypePermissions).HasForeignKey(p => p.SecurityRoleID);
            modelBuilder.Entity<SecurityObjectPermission>().HasOne(p => p.SecurityRole).WithMany(p => p.ObjectPermissions).HasForeignKey(p => p.SecurityRoleID);
            modelBuilder.Entity<SecurityMemberPermission>().HasOne(p => p.SecurityRole).WithMany(p => p.MemberPermissions).HasForeignKey(p => p.SecurityRoleID);
        }
        public ISecurityUser CurrentUser { get; set; }
        public ISecurityUser GetUserByCredentials(string user, string password) {
            return this.Users.
                            Include(p => p.UserRoleCollection).ThenInclude(p => p.Role).ThenInclude(p => p.MemberPermissions).
                             Include(p => p.UserRoleCollection).ThenInclude(p => p.Role).ThenInclude(p => p.OperationPermissions).
                             Include(p => p.UserRoleCollection).ThenInclude(p => p.Role).ThenInclude(p => p.ObjectPermissions).
                             Include(p => p.UserRoleCollection).ThenInclude(p => p.Role).ThenInclude(p => p.TypePermissions).
                            First(p => p.Name == user && p.Password == password);
        }
        public virtual void Logon(string user, string password) {
            ISecurityUser currentUser = GetUserByCredentials(user, password);
            Logon(currentUser);
        }

        public virtual void Logon(ISecurityUser user) {
            CurrentUser = user;
            InitSecurity(user);
        }
        protected virtual void InitSecurity(ISecurityUser user) {          
            var allPermissions = user.GetAllPermissions();
            foreach(var permission in allPermissions) {
                Security.AddPermission(permission);
            }
        }
        public virtual DbSet<SecurityUser> Users { get; set; }
        public virtual DbSet<SecurityRole> Roles { get; set; }
    }

}
