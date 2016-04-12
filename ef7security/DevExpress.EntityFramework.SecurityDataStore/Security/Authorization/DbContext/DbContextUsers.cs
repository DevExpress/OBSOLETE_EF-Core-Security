using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;

namespace DevExpress.EntityFramework.SecurityDataStore.Security {
    public abstract class DbContextUsersBase : SecurityDbContext, ISupportUsers {
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<UserRole>().HasOne(p => p.Role).WithMany(p => p.UserRoleCollection).HasForeignKey(p => p.RoleID);
            modelBuilder.Entity<UserRole>().HasOne(p => p.User).WithMany(p => p.UserRoleCollection).HasForeignKey(p => p.UserID);
            modelBuilder.Entity<SecurityPolicyPermission>().HasOne(p => p.SecurityRole).WithMany(p => p.OperationPermissionCollection).HasForeignKey(p=>p.SecurityRoleID);
            modelBuilder.Entity<SecurityTypePermission>().HasOne(p => p.SecurityRole).WithMany(p => p.TypePermissionCollection).HasForeignKey(p => p.SecurityRoleID);
            modelBuilder.Entity<SecurityObjectPermission>().HasOne(p => p.SecurityRole).WithMany(p => p.ObjectPermissionCollection).HasForeignKey(p => p.SecurityRoleID);
            modelBuilder.Entity<SecurityMemberPermission>().HasOne(p => p.SecurityRole).WithMany(p => p.MemberPermissionCollection).HasForeignKey(p => p.SecurityRoleID);
        }
        public ISecurityUser CurrentUser { get; set; }
        public virtual void Logon(string user, string password) {
            var currentUser = this.Users.
                Include(p => p.UserRoleCollection).ThenInclude(p => p.Role).ThenInclude(p => p.MemberPermissionCollection).
                 Include(p => p.UserRoleCollection).ThenInclude(p => p.Role).ThenInclude(p => p.OperationPermissionCollection).
                 Include(p => p.UserRoleCollection).ThenInclude(p => p.Role).ThenInclude(p => p.ObjectPermissionCollection).
                 Include(p => p.UserRoleCollection).ThenInclude(p => p.Role).ThenInclude(p => p.TypePermissionCollection).
                First(p => p.Name == user && p.Password == password);
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
