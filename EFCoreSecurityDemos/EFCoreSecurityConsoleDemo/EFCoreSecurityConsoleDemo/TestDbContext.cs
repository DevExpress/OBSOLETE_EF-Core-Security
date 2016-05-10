using System.Collections.Generic;
using DevExpress.EntityFramework.SecurityDataStore;
using Microsoft.EntityFrameworkCore;

namespace EFCoreSecurityConsoleDemo {
    public class EFSimpleDB : SecurityDbContext {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        protected override void OnSecuredConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseInMemoryDatabase();
            // TODO: support for multiple options
            optionsBuilder.EnableSensitiveDataLogging();
        }
        // public EFSimpleDB() : base(new DbContextOptions<EFSimpleDB>()) { }       
    }
    public class User {
        public int ID { get; set; }
        public int Age { get; set; }
        public string UserName { get; set; }
        public List<Role> Roles { get; set; }
    }
    public class Role {
        public int ID { get; set; }
        public string RoleName { get; set; }
        public User User { get; set; }
    }
}
