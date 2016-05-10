using DevExpress.EntityFramework.SecurityDataStore;
using DevExpress.EntityFramework.SecurityDataStore.Authorization;
using DevExpress.EntityFramework.SecurityDataStore.Security;
using Microsoft.EntityFrameworkCore;

namespace EFCoreSecurityODataService.DataModel {
    public class EFCoreDemoDbContext : SecurityDbContextWithUsers  {
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<DemoTask> Tasks { get; set; }
        public DbSet<ContactTask> ContactTasks { get; set; }
        protected override void OnSecuredConfiguring(DbContextOptionsBuilder optionsBuilder) {
            base.OnSecuredConfiguring(optionsBuilder);
            optionsBuilder.UseInMemoryDatabase();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Contact>().HasOne(p => p.Department).WithMany(p => p.Contacts).HasForeignKey(p => p.DepartmentId);
            modelBuilder.Entity<Contact>().HasMany(p => p.ContactTasks).WithOne(p => p.Contact).HasForeignKey(p => p.ContactId);
            modelBuilder.Entity<DemoTask>().HasMany(p => p.ContactTasks).WithOne(p => p.Task).HasForeignKey(p => p.TaskId);
        }
    }
}
