using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.TransparentWrapper {
    [TestFixture]
    public class DbSetQueryableNavigationPropertyTests {
        [SetUp]
        public void SetUp() {
            DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass().MakeRealDbContext();
            dbContextConnectionClass.Database.EnsureDeleted();
            dbContextConnectionClass.Database.EnsureCreated();
        }
        [Test]
        public void CreateNative() {
            createMethod(() => new DbContextConnectionClass().MakeRealDbContext());
        }
        [Test]
        public void CreateDXProvider() {
            createMethod(() => new DbContextConnectionClass());
        }
        private void createMethod(Func<DbContextConnectionClass> createDbContext) {
            using(var context = createDbContext()) {
                var director = context.Add(new Person() { PersonName = "1" });
                context.Add(new Company() { CompanyName = "2", Person = director.Entity });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                var company = context.Company.Include(p => p.Person).First();
                Assert.AreEqual(company.Person.PersonName, "1");
            }
        }
        [Test]
        public void UpdateNative() {
            UpdateMethod(() => new DbContextConnectionClass().MakeRealDbContext());
        }
        [Test]
        public void UpdateDXProvider() {
            UpdateMethod(() => new DbContextConnectionClass());
        }
        private void UpdateMethod(Func<DbContextConnectionClass> createDbContext) {
            using(var context = createDbContext()) {
                var director = context.Add(new Person() { PersonName = "1" });
                context.Add(new Company() { CompanyName = "2", Person = director.Entity });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                var company = context.Company.Include(p => p.Person).First();
                company.Person.PersonName = "2";
                var entri = context.ChangeTracker.Entries();
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                var company = context.Company.Include(p => p.Person).First();
                Assert.AreEqual(company.Person.PersonName, "2");
            }
        }
        [Test]
        public void DeleteNative() {
            DeleteMethod(() => new DbContextConnectionClass().MakeRealDbContext());
        }
        [Test]
        public void DeleteDXProvider() {
            DeleteMethod(() => new DbContextConnectionClass());
        }
        private void DeleteMethod(Func<DbContextConnectionClass> createDbContext) {
            using(var context = createDbContext()) {
                var director = context.Add(new Person() { PersonName = "1" });
                context.Add(new Company() { CompanyName = "2", Person = director.Entity });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                var newdirector = context.Company.Single();
                context.Remove(newdirector);
                context.SaveChanges();

            }
            using(var context = createDbContext()) {
                Assert.AreEqual(context.Company.Count(), 0);
            }
        }
        [Test]
        public void DeleteReferencedObjectNative() {
            DeleteReferencedObject(() => new DbContextConnectionClass().MakeRealDbContext());
        }
        [Test]
        public void Delete_ReferencedObjectDXProvider() {
            DeleteReferencedObject(() => new DbContextConnectionClass());
        }
        public void DeleteReferencedObject(Func<DbContextConnectionClass> createDbContext) {
            using(var context = createDbContext()) {
                var director = context.Add(new Person() { PersonName = "1" });
                context.Add(new Company() { CompanyName = "2", Person = director.Entity });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                var newdirector = context.Company.Include(p => p.Person).First();
                context.Remove(newdirector);
                try {
                    context.SaveChanges();
                    Assert.Fail();
                }
                catch {
                    //The DELETE statement conflicted with the REFERENCE constraint "FK_Company_Persons_Personid". The conflict occurred in database "EntityFramework_DbContextDataStore_Tests", table "dbo.Company", column 'Personid'.
                }
            }
        }
    }
    [TestFixture]
    public class ManyToMany {
        [SetUp]
        public void SetUp() {
            SoccerContext soccerContext = new SoccerContext().MakeRealDbContext();
            soccerContext.Database.EnsureDeleted();
            soccerContext.Database.EnsureCreated();
        }
        [Test]
        public void CreateManyToOneNative() {
            CreateManyToOne(() => new SoccerContext().MakeRealDbContext());
        }
        [Test]
        public void CreateManyToOneDXProvider() {
            CreateManyToOne(() => new SoccerContext());
        }
        void CreateManyToOne(Func<SoccerContext> createDbContext) {
            using(var context = createDbContext()) {
                Team team1 = new Team() { Name = "Team 1" };
                Player player1 = new Player() { Name = "Player 1", Age = 18, Team = team1 };
                Player player2 = new Player() { Name = "Player 2", Age = 12, Team = team1 };
                context.Teams.Add(team1);
                context.Add(player1);
                context.Add(player2);
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                var team = context.Teams.Include(p => p.TeamPlayers).First();
                var player = context.Players.Include(p => p.Team).First();
                var ct = context.ChangeTracker.Entries();
            }
        }
    }
}
