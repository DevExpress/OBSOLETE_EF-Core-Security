using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DevExpress.EntityFramework.SecurityDataStore;
using DevExpress.EntityFramework.SecurityDataStore.Security.BusinessEntities;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using DevExpress.EntityFramework.SecurityDataStore.Tests.Security;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts {
    public class DbContextSecurityObject : SecurityDbContext  {
        protected override void OnSecuredConfiguring(DbContextOptionsBuilder optionsBuilder) {
            SecurityTestHelper.ConfigureOptionsBuilder(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<SecurityPerson>().HasOne(p => p.SecurityCompany).WithMany(p => p.CollectionSecurityPerson);
        }
        public DbSet<SecurityCompany> SecurityCompany { get; set; }
        public DbSet<SecurityPerson> SecurityPerson { get; set; }
    }
    public class SecurityCompany : BaseSecurityObject {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<SecurityPerson> CollectionSecurityPerson { get; set; }
        public SecurityCompany() {
            CollectionSecurityPerson = new List<SecurityPerson>();
        }
    }
    public class SecurityPerson : BaseSecurityObject {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int SecurityCompanyId { get; set; }
        public SecurityCompany SecurityCompany { get; set; }
    }
}
