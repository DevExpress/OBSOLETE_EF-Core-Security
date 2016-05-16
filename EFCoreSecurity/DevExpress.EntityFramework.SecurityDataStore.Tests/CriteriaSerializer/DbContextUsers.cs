using DevExpress.EntityFramework.SecurityDataStore.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts;
using DevExpress.EntityFramework.SecurityDataStore.Authorization;
using DevExpress.EntityFramework.Authorization;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests {
    public class TestDbContextWithUsers : SecurityDbContextWithUsers {
        protected override void OnSecuredConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseInMemoryDatabase();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Company>().HasOne(p => p.Person).WithOne(p => p.Company).HasForeignKey<Person>(p => p.CompanyFK);
            modelBuilder.Entity<Person>().HasOne(p => p.Company).WithOne(p => p.Person).HasForeignKey<Company>(p => p.PersonsFK);
            modelBuilder.Entity<Person>().HasOne(p => p.One).WithMany(p => p.Collection).HasForeignKey(p => p.OneFK);
        }
        public DbSet<Person> Person { get; set; }
        public DbSet<Company> Company { get; set; }
    }
}
