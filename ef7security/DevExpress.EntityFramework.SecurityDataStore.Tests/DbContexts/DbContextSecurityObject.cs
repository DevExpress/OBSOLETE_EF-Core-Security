using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DevExpress.EntityFramework.SecurityDataStore;
using DevExpress.EntityFramework.SecurityDataStore.Security.BusinessEntities;
using System.Collections.ObjectModel;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts {
    public class DbContextSecurityObject : SecurityDbContext {
        protected override void OnSecuredConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseInMemoryDatabase();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<SecurityPerson>().HasOne(p => p.SecurityCompany).WithMany(p => p.CollectionSecurityPerson);
        }
        public DbSet<SecurityCompany> SecurityCompany { get; set; }
        public DbSet<SecurityPerson> SecurityPerson { get; set; }
    }
    public class SecurityCompany : BaseSecurityObject {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<SecurityPerson> CollectionSecurityPerson { get; set; }
        public SecurityCompany() {
            CollectionSecurityPerson = new Collection<SecurityPerson>();
        }
    }
    public class SecurityPerson : BaseSecurityObject {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public SecurityCompany SecurityCompany { get; set; }
    }
}
