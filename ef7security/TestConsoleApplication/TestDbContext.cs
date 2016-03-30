using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using System.ComponentModel.DataAnnotations.Schema;
using DevExpress.EntityFramework.SecurityDataStore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace TestSimpleDBContext {
    public class EFSimpleDB : SecurityDbContext {       
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }       
        protected override void OnSecuredConfiguring(DbContextOptionsBuilder optionsBuilder) {           
            optionsBuilder.UseInMemoryDatabase();
            // TODO: support for multiple options
            // optionsBuilder.EnableSensitiveDataLogging();
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
