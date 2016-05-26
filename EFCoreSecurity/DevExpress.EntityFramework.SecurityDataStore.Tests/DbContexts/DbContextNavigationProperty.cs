using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DevExpress.EntityFramework.SecurityDataStore.Security.BusinessEntities;
using System.ComponentModel.DataAnnotations;
using DevExpress.EntityFramework.SecurityDataStore.Tests.Security;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts {
    public class DbContextConnectionClass : DbContextDbSetBasePerson {
        protected override void OnSecuredConfiguring(DbContextOptionsBuilder optionsBuilder) {
            SecurityTestHelper.ConfigureOptionsBuilder(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Company>().HasOne(p => p.Person).WithOne(p => p.Company).HasForeignKey<Person>(p => p.CompanyId).IsRequired(false);
            modelBuilder.Entity<Person>().HasOne(p => p.Company).WithOne(p => p.Person).HasForeignKey<Company>(p => p.PersonsId).IsRequired(false);
            modelBuilder.Entity<Office>().HasOne(p => p.Company).WithMany(p => p.Offices).HasForeignKey(p => p.CompanyId).IsRequired(false);
        }
    }
    public class DbContextDbSetBasePerson : SecurityDbContext {
        public DbSet<Person> Persons { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Office> Offices { get; set; }
    }
    public class SoccerContextBase : SecurityDbContext {
        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }
    }
    public class SoccerContext : SoccerContextBase {
        protected override void OnSecuredConfiguring(DbContextOptionsBuilder optionsBuilder) {
            SecurityTestHelper.ConfigureOptionsBuilder(optionsBuilder);
        }
    }
    public class Office : BaseSecurityObject {
        [Key]
        public int Id { get; private set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Company Company { get; set; }
        public int? CompanyId { get; set; }
    }
    public class Person : BaseSecurityObject {
        [Key]
        public int Id { get; private set; }
        public string PersonName { get; set; }
        public string Description { get; set; }
        public Company Company { get; set; }
        // public Company One { get; set; }

        // public int? OneId { get; set; }
        public int? CompanyId { get; set; }
    }
    public class Company : BaseSecurityObject {
        [Key]
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public Person Person { get; set; }
        public List<Office> Offices { get; set; } = new List<Office>();

        // public int? CollectionFK { get; set; }
        public int? PersonsId { get; set; }
    }
    public class Team {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Coach { get; set; }

        List<Player> _Players = new List<Player>();
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
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public Team Team { get; set; }
    }
}
