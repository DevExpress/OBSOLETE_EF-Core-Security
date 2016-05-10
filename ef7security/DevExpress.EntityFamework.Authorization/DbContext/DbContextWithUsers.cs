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
    public abstract class SecurityDbContextWithUsers : SecurityDbContext, IAuthorizationData {
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<UserRole>().HasOne(p => p.Role).WithMany(p => p.Roles).HasForeignKey(p => p.RoleID);
            modelBuilder.Entity<UserRole>().HasOne(p => p.User).WithMany(p => p.UserRoleCollection).HasForeignKey(p => p.UserID);
            modelBuilder.Entity<SecurityPolicyPermission>().HasOne(p => p.SecurityRole).WithMany(p => p.OperationPermissions).HasForeignKey(p=>p.SecurityRoleID);
            modelBuilder.Entity<SecurityTypePermission>().HasOne(p => p.SecurityRole).WithMany(p => p.TypePermissions).HasForeignKey(p => p.SecurityRoleID);
            modelBuilder.Entity<SecurityObjectPermission>().HasOne(p => p.SecurityRole).WithMany(p => p.ObjectPermissions).HasForeignKey(p => p.SecurityRoleID);
            modelBuilder.Entity<SecurityMemberPermission>().HasOne(p => p.SecurityRole).WithMany(p => p.MemberPermissions).HasForeignKey(p => p.SecurityRoleID);
        }
        protected virtual void InitSecurity(ISecurityUser user) {
            var allPermissions = user.GetAllPermissions();
            foreach (var permission in allPermissions) {
                Security.PermissionsRepository.AddPermission(permission);
            }
        }

        public virtual DbSet<SecurityUser> Users { get; set; }
        public virtual DbSet<SecurityRole> Roles { get; set; }
        public ISecurityUser CurrentUser { get; set; }

        public ISecurityUser GetUserByCredentials(string userName, string password) {
            return this.Users.
                            Include(p => p.UserRoleCollection).ThenInclude(p => p.Role).ThenInclude(p => p.MemberPermissions).
                            Include(p => p.UserRoleCollection).ThenInclude(p => p.Role).ThenInclude(p => p.OperationPermissions).
                            Include(p => p.UserRoleCollection).ThenInclude(p => p.Role).ThenInclude(p => p.ObjectPermissions).
                            Include(p => p.UserRoleCollection).ThenInclude(p => p.Role).ThenInclude(p => p.TypePermissions).
                            FirstOrDefault(p => p.Name == userName && p.Password == password);
        }
        public virtual void Logon(string userName, string password) {
            ISecurityUser currentUser = GetUserByCredentials(userName, password);
            if(currentUser == null) {
                throw new InvalidOperationException("Logon is failed. Try enter right credentials.");
            }
            Logon(currentUser);
        }
        public virtual void Logon(ISecurityUser user) {
            CurrentUser = user;
            InitSecurity(user);
        }
        public virtual void Logoff() {
            if (CurrentUser != null) {
                Security.PermissionsRepository.ClearPermissions();
                CurrentUser = null;
            }
        }
    }
}
