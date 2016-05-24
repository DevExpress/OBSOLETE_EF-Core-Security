using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

        [Test]
        public void SelectManyNative() {
            SelectMany(() => new DbContextConnectionClass().MakeRealDbContext());
        }
        [Test]
        public void SelectManyDXProvider() {
            SelectMany(() => new DbContextConnectionClass());
        }
        public void SelectMany(Func<DbContextConnectionClass> createDbContext) {
            using(var context = createDbContext()) {
                Company company = new Company();
                company.Offices.Add(new Office());
                company.Offices.Add(new Office());
                company.Offices.Add(new Office());
                context.Add(company);
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                var description = context.Company.SelectMany(p => p.Offices).Select(p=>p.Description);
                Assert.AreEqual(3, description.Count());
                var persons = context.Company.SelectMany(p => p.Offices);
                Assert.AreEqual(3, persons.Count());
            }
        }

        [Test]
        public void SelectManyIndexNative() {
            SelectMany(() => new DbContextConnectionClass().MakeRealDbContext());
        }
        [Test]
        public void SelectManyIndexDXProvider() {
            SelectMany(() => new DbContextConnectionClass());
        }
        public void SelectManyIndex(Func<DbContextConnectionClass> createDbContext) {
            using(var context = createDbContext()) {
                Company company = new Company();
                company.Offices.Add(new Office());
                company.Offices.Add(new Office());
                company.Offices.Add(new Office());
                context.Add(company);
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                var description = context.Company.SelectMany((p,i) => p.Offices ).Select(p => p.Description);
                Assert.AreEqual(3, description.Count());
                var persons = context.Company.SelectMany(p => p.Offices);
                Assert.AreEqual(3, persons.Count());
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
    [TestFixture]
    public class ThenIncludeTest {
        [SetUp]
        public void SetUp() {
            using(DbContextThenInclude contex = new DbContextThenInclude()) {
                contex.Database.EnsureCreated();
            }
        }
        [TearDown]
        public void TearDown() {
            using(DbContextThenInclude contex = new DbContextThenInclude()) {
                contex.Database.EnsureDeleted();
            }
        }
        [Test]
        public void ThenIncludeNativeTest() {
            ThenIncludeBaseTest(() => new DbContextThenInclude().GetRealDbContext());
        }
        [Test]
        public void ThenIncludeDXProviderTest() {
            ThenIncludeBaseTest(() => new DbContextThenInclude());
        }
        public void ThenIncludeBaseTest(Func<DbContextThenInclude> createContext) {
            CreateData(createContext);
            using(var context = createContext()) {
                Parent parent = context.Parent.Include(p => p.ChildCollection).ThenInclude(p => p.Child).ThenInclude(p => p.Child).First();
                Assert.AreEqual(2, parent.ChildCollection.Count);
                Child1 child1 = parent.ChildCollection.First(p => p.Name == "1");
                Assert.IsNotNull(child1.Child);
                Assert.IsNotNull(child1.Child.Child);
            }
        }

        private void CreateData(Func<DbContextThenInclude> createContext) {
            using(var context = createContext()) {
                Parent parent = new Parent();
                parent.Name = "1";
                Child1 child = new Child1() { Name = "1" };
                parent.ChildCollection.Add(child);
                parent.ChildCollection.Add(new Child1() { Name = "2" });
                child.Child = new Child2() { Name = "1" };
                child.Child.Child = new Child3() { Name = "1" };
                context.Add(parent);
                context.SaveChanges();
            }
        }
    }

    [TestFixture]
    public class SelectNestedNavigationTest {
        [SetUp]
        public void SetUp() {
            using(DbContextConnectionClass contex = new DbContextConnectionClass()) {
                contex.Database.EnsureCreated();
            }
        }
        [TearDown]
        public void TearDown() {
            using(DbContextConnectionClass contex = new DbContextConnectionClass()) {
                contex.Database.EnsureDeleted();
            }
        }
        [Test]
        public void IncludeSelectNativeTest() {
            IncludeSelectBaseTest(() => new DbContextConnectionClass().GetRealDbContext());
        }
        [Test]
        public void IncludeSelectDXProviderTest() {
            IncludeSelectBaseTest(() => new DbContextConnectionClass());
        }
        public void IncludeSelectBaseTest(Func<DbContextConnectionClass> createContext) {
            CreateData(createContext);
            using(var context = createContext()) {
                IQueryable<Company> company = context.Offices.Include(p => p.Company).Select(p => p.Company);
                List<Office> offices = company.First().Offices;
                Assert.IsEmpty(offices);
            }
        }
        [Test]
        public void SelectIncludeNativeTest() {
            SelectIncludeBaseTest(() => new DbContextConnectionClass().GetRealDbContext());
        }
        [Test]
        public void SelectIncludeDXProviderTest() {
            SelectIncludeBaseTest(() => new DbContextConnectionClass());
        }
        public void SelectIncludeBaseTest(Func<DbContextConnectionClass> createContext) {
            CreateData(createContext);
            using(var context = createContext()) {
                //IQueryable<Company> company = context.Persons.Select(p => p.One).Include(c => c.Offices);
                //List<Person> persons = company.First().Offices;
                //Assert.IsNotEmpty(persons);

                IQueryable<Company> company = context.Offices.Select(p => p.Company).Include(c => c.Offices);
                List<Office> offices = company.First().Offices;
                Assert.IsNotEmpty(offices);
            }
        }
        [Test]
        public void IncludeNativeTest() {
            IncludeBaseTest(() => new DbContextConnectionClass().GetRealDbContext());
        }
        [Test]
        public void IncludeDXProviderTest() {
            IncludeBaseTest(() => new DbContextConnectionClass());
        }
        public void IncludeBaseTest(Func<DbContextConnectionClass> createContext) {
            CreateData(createContext);
            using(var context = createContext()) {
                Company company = context.Offices.Include(c => c.Company).First().Company;
                List<Office> offices = company.Offices;
                Assert.IsNotEmpty(offices);
                Assert.AreEqual(offices.Count, 1);
                Assert.AreEqual(offices.First().Name, "London");
            }
        }
        [Test]
        public void FromContextAnyNativeTest() {
            FromContextAnyBaseTest(() => new DbContextConnectionClass().GetRealDbContext());
        }
        [Test]
        public void FromContextAnyDXProviderTest() {
            FromContextAnyBaseTest(() => new DbContextConnectionClass());
        }
        public void FromContextAnyBaseTest(Func<DbContextConnectionClass> createContext) {
            CreateData(createContext);
            using(var context = createContext()) {
                IQueryable<Company> company = context.Company.Where(c => c.Offices.Any(p => p.Name == "London"));
                List<Office> offices = company.First().Offices;
                Assert.IsEmpty(offices);
            }
        }
        [Test]
        public void FromContextAnyIncludeNativeTest() {
            FromContextAnyIncludeBaseTest(() => new DbContextConnectionClass().GetRealDbContext());
        }
        [Test]
        public void FromContextAnyIncludeDXProviderTest() {
            FromContextAnyIncludeBaseTest(() => new DbContextConnectionClass());
        }
        public void FromContextAnyIncludeBaseTest(Func<DbContextConnectionClass> createContext) {
            CreateData(createContext);
            using(var context = createContext()) {
                IQueryable<Company> company = context.Company.Include(c => c.Offices).Where(c => c.Offices.Any(p => p.Name == "London"));
                List<Office> offices = company.First().Offices;
                Assert.IsNotEmpty(offices);
                Assert.AreEqual(offices.Count, 1);
                Assert.AreEqual(offices.First().Name, "London");
            }
        }

        [Test]
        public void FromContextNativeTest() {
            FromContextBaseTest(() => new DbContextConnectionClass().GetRealDbContext());
        }
        [Test]
        public void FromContextDXProviderTest() {
            FromContextBaseTest(() => new DbContextConnectionClass());
        }
        public void FromContextBaseTest(Func<DbContextConnectionClass> createContext) {
            CreateData(createContext);
            using(var context = createContext()) {
                IQueryable<Office> offices = context.Offices.Where(o => o.Company.CompanyName == "Microsoft");
                Company company = offices.First().Company;
                Assert.IsNull(company);
            }
        }
        [Test]
        public void FromContextIncludeNativeTest() {
            FromContextIncludeBaseTest(() => new DbContextConnectionClass().GetRealDbContext());
        }
        [Test]
        public void FromContextIncludeDXProviderTest() {
            FromContextIncludeBaseTest(() => new DbContextConnectionClass());
        }
        public void FromContextIncludeBaseTest(Func<DbContextConnectionClass> createContext) {
            CreateData(createContext);
            using(var context = createContext()) {
                IQueryable<Office> offices = context.Offices.Include(c => c.Company).Where(c => c.Company.CompanyName == "Microsoft");
                Company company = offices.First().Company;
                Assert.IsNotNull(company);
                Assert.AreEqual(company.CompanyName, "Microsoft");
            }
        }

        [Test]
        public void FromContextContainsNativeTest() {
            FromContextContainsBaseTest(() => new DbContextConnectionClass().GetRealDbContext());
        }
        [Test]
        public void FromContextContainsDXProviderTest() {
            FromContextContainsBaseTest(() => new DbContextConnectionClass());
        }
        public void FromContextContainsBaseTest(Func<DbContextConnectionClass> createContext) {
            CreateData(createContext);
            using(var context = createContext()) {
                IQueryable<Company> company = context.Company.Where(c => c.Offices.Contains(context.Offices.Where(p => p.Name == "London").First()));
                List<Office> offices = company.First().Offices;
                Assert.IsEmpty(offices);
            }
        }
        [Test]
        public void FromContextContainsIncludeNativeTest() {
            FromContextContainsIncludeBaseTest(() => new DbContextConnectionClass().GetRealDbContext());
        }
        [Test]
        public void FromContextContainsIncludeDXProviderTest() {
            FromContextContainsIncludeBaseTest(() => new DbContextConnectionClass());
        }
        public void FromContextContainsIncludeBaseTest(Func<DbContextConnectionClass> createContext) {
            CreateData(createContext);
            using(var context = createContext()) {
                IQueryable<Company> company = context.Company.Include(c => c.Offices).Where(c => c.Offices.Contains(context.Offices.Where(p => p.Name == "London").First()));
                List<Office> offices = company.First().Offices;
                Assert.IsNotEmpty(offices);
                Assert.AreEqual(offices.Count, 1);
                Assert.AreEqual(offices.First().Name, "London");
            }
        }

        private void CreateData(Func<DbContextConnectionClass> createContext) {
            using(var context = createContext()) {
                // Person person = new Person() { PersonName = "John" };
                Office office = new Office() { Name = "London" };
                Company company = new Company() { CompanyName = "Microsoft" };
                office.Company = company;
                context.Add(office);
                context.SaveChanges();
            }
        }
    }
}
