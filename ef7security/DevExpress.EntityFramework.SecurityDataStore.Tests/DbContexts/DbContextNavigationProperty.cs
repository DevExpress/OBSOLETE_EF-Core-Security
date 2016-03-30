using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts {
    public class DbContextConnectionClass : DbContextDbSetBasePerson {
        protected override void OnSecuredConfiguring(DbContextOptionsBuilder options) {
            options.UseInMemoryDatabase();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Company>().HasOne(p => p.Person).WithOne(p => p.Company).HasForeignKey<Person>(p=>p.CompanyFK);
            modelBuilder.Entity<Person>().HasOne(p => p.Company).WithOne(p => p.Person).HasForeignKey<Company>(p=>p.PersonsFK);
            modelBuilder.Entity<Person>().HasOne(p => p.One).WithMany(p => p.Collection).HasForeignKey(p => p.OneFK);
        }
    }
    public class DbContextDbSetBasePerson : SecurityDbContext {
        public DbSet<Person> Persons { get; set; }
        public DbSet<Company> Company { get; set; }
     

    }
    public class SoccerContextBase : SecurityDbContext {
        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }


    }
    public class SimpleTest {
        public int ID { get; set; }
        public string Name { get; set; }
    }
    public class SoccerContext : SoccerContextBase {
        protected override void OnSecuredConfiguring(DbContextOptionsBuilder options) {
            options.UseInMemoryDatabase();
        }
    }
    public class Person {
        public int ID { get; private set; }

        public SimpleTest SimpleTest { get; set; }
        public string PersonName { get; set; }
        public string Description { get; set; }
        public Company Company { get; set; }
        public Company One { get; set; }

        public int OneFK { get; set; }
        public int CompanyFK { get; set; }
    }
    public class Company {
        public int ID { get; set; }

        public string CompanyName { get; set; }
        public string Description { get; set; }
        public Person Person { get; set; }
        public List<Person> Collection { get; set; } = new List<Person>();

        public int CollectionFK { get; set; }
        public int PersonsFK { get; set; }
    }
    public class Team {
        List<Player> _Players = new List<Player>();
        public int Id { get; set; }
        public string Name { get; set; }
        public string Coach { get; set; }
        public List<Player> TeamPlayers {
            get {
                return _Players;
            }
            set {
                _Players = value;
            }
        }
    }
    public class Player {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public Team Team { get; set; }
    }
}
