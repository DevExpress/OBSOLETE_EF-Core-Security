using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using DevExpress.EntityFramework.SecurityDataStore.Tests.Helpers;
using DevExpress.EntityFramework.SecurityDataStore.Security.BaseSecurityEntity;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts {
    public class DbContextManyToManyRelationship : SecurityDbContext {
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<DemoTask> Tasks { get; set; }
        public DbSet<ContactTask> ContactTasks { get; set; }
        protected override void OnSecuredConfiguring(DbContextOptionsBuilder optionsBuilder) {
            base.OnSecuredConfiguring(optionsBuilder);
            SecurityTestHelper.ConfigureOptionsBuilder(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Contact>().HasOne(p => p.Department).WithMany(p => p.Contacts).HasForeignKey(p => p.DepartmentId).IsRequired(false);
            modelBuilder.Entity<Contact>().HasMany(p => p.ContactTasks).WithOne(p => p.Contact).HasForeignKey(p => p.ContactId);
            modelBuilder.Entity<DemoTask>().HasMany(p => p.ContactTasks).WithOne(p => p.Task).HasForeignKey(p => p.TaskId);
        }
    }

    public class ContactTask : BaseSecurityEntity {
        public int Id { get; set; }
        public int ContactId { get; set; }
        public Contact Contact { get; set; }
        public int TaskId { get; set; }
        public DemoTask Task { get; set; }
    }

    public class DemoTask : BaseSecurityEntity {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DateCompleted { get; set; }
        public int PercentCompleted { get; set; }
        public List<ContactTask> ContactTasks { get; set; }
        public DemoTask() {
            ContactTasks = new List<ContactTask>();
        }
    }

    public class Department : BaseSecurityEntity {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Office { get; set; }
        public List<Contact> Contacts { get; set; }
    }

    public class Contact : BaseSecurityEntity {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int? DepartmentId { get; set; }
        public Department Department { get; set; }
        public List<ContactTask> ContactTasks { get; set; }
    }
}
