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
            modelBuilder.Entity<IUserRole>().HasOne(p => p.Role).WithMany(p => p.UserRoleCollection);
            modelBuilder.Entity<IUserRole>().HasOne(p => p.User).WithMany(p => p.UserRoleCollection);
            modelBuilder.Entity<ISecurityPermission>().HasOne(p => p.SecurityRole).WithMany(p => p.OperationPermissionCollection);
            modelBuilder.Entity<ISecurityTypePermission>().HasOne(p => p.SecurityRole).WithMany(p => p.TypePermissionCollection);
            modelBuilder.Entity<ISecurityObjectPermission>().HasOne(p => p.SecurityRole).WithMany(p => p.ObjectPermissionCollection);
            modelBuilder.Entity<ISecurityMemberPermission>().HasOne(p => p.SecurityRole).WithMany(p => p.MemberPermissionCollection);           
        }
        public override void SecurityRegistrationServices(IServiceCollection service) {
            service.AddScoped<ISecurityStrategy, UserSecurityStrategy>();
        }
        public virtual DbSet<ISecurityUser> Users { get; set; }
        public virtual DbSet<ISecurityRole> Roles { get; set; }
        
    }
}
