using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts {
    public class DbContextThenInclude : SecurityDbContext {
        protected override void OnSecuredConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseInMemoryDatabase();
            // optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=efcoresecuritytests;Trusted_Connection=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Parent>().HasMany(p => p.ChildCollection).WithOne(p=>p.Parent);
        }
        public DbSet<Parent> Parent { get; set; }
    }
    public class Parent {
        public Guid ID { get; set; }
        public string Name { get; set; }      
        public Collection<Child1> ChildCollection { get; set; } 
            = new Collection<Child1>();
    }
    public class Child1 {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public Child2 Child { get; set; }
        public Parent Parent { get; set; }
    }
    public class Child2 {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public Child3 Child { get; set; }
    }
    public class Child3 {
        public Guid ID { get; set; }
        public string Name { get; set; }
    }
}
