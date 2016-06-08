using NUnit.Framework;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DevExpress.EntityFramework.SecurityDataStore.Tests.Helpers;
using DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.Security.BusinessObjects {
    [TestFixture]
    public abstract class BaseSecurityObjectsTests {
        [SetUp]
        public void ClearDatabase() {
            using(DbContextSecurityObject context = new DbContextSecurityObject()) {
                context.ResetDatabase();
            }
        }
        /*
        [TearDown]
        public void TearDown() {
            using(DbContextSecurityObject context = new DbContextSecurityObject()) {
                context.Database.EnsureDeleted();
            }
        }
        */
        [Test]
        public void GetBlockedMembersOnRead() {
            CreateThreeObjects();
            using(DbContextSecurityObject dbContextConnectionClass = new DbContextSecurityObject()) {
                var Company = dbContextConnectionClass.SecurityCompany.First();
                Assert.IsEmpty(Company.BlockedMembers);
                Assert.IsEmpty(Company.ReadOnlyMembers);
                Assert.IsEmpty(Company.ReadOnlyMembersOnLoad);
            }
            using(DbContextSecurityObject context = new DbContextSecurityObject()) {
                context.PermissionsContainer.AddMemberPermission<DbContextSecurityObject, SecurityCompany>(
                    SecurityOperation.Read, OperationState.Deny, "Name", (s,t) => true);
                context.PermissionsContainer.AddMemberPermission<DbContextSecurityObject, SecurityCompany>(
                    SecurityOperation.Read, OperationState.Deny, "Description", (s, t) => t.Description == "1");
                var Company = context.SecurityCompany.First();
                Assert.AreEqual("Description;Name",  Company.BlockedMembers);
                Assert.IsEmpty(Company.ReadOnlyMembers);
                Assert.IsEmpty(Company.ReadOnlyMembersOnLoad);
            }
        }
        [Test]
        public void GetBlockedMembersOnWrite() {
            CreateThreeObjects();
            using(DbContextSecurityObject dbContextConnectionClass = new DbContextSecurityObject()) {
                var Company = dbContextConnectionClass.SecurityCompany.First();
                Assert.IsEmpty(Company.BlockedMembers);
                Assert.IsEmpty(Company.ReadOnlyMembers);
                Assert.IsEmpty(Company.ReadOnlyMembersOnLoad);
            }
            using(DbContextSecurityObject context = new DbContextSecurityObject()) {
                context.PermissionsContainer.AddMemberPermission<DbContextSecurityObject, SecurityCompany>(
                    SecurityOperation.Write, OperationState.Deny, "Name", (s, t) => true);
                context.PermissionsContainer.AddMemberPermission<DbContextSecurityObject, SecurityCompany>(
                    SecurityOperation.Write, OperationState.Deny, "Description", (s, t) => t.Description == "1");
                var Company = context.SecurityCompany.First();
                Assert.IsEmpty(Company.BlockedMembers);
                Assert.AreEqual("Name", Company.ReadOnlyMembers);
                Assert.AreEqual("Description;Name", Company.ReadOnlyMembersOnLoad);
            }
        }
        [Test]
        public void GetBlockedNavigationCollectionMembersOnRead() {
            CreateThreeObjects();
            using(DbContextSecurityObject dbContextConnectionClass = new DbContextSecurityObject()) {
                var Company = dbContextConnectionClass.SecurityCompany.First();
                Assert.IsEmpty(Company.BlockedMembers);
                Assert.IsEmpty(Company.ReadOnlyMembers);
                Assert.IsEmpty(Company.ReadOnlyMembersOnLoad);
            }
            using(DbContextSecurityObject context = new DbContextSecurityObject()) {
                context.PermissionsContainer.AddMemberPermission<DbContextSecurityObject, SecurityCompany>(
                    SecurityOperation.Read, OperationState.Deny, "CollectionSecurityPerson", (s, t) => true);               
                var Company = context.SecurityCompany.Include(p => p.CollectionSecurityPerson).First();
                Assert.IsNull(Company.CollectionSecurityPerson);
                Company.BlockedMembers.Contains("CollectionSecurityPerson");
                Assert.IsEmpty(Company.ReadOnlyMembers);
                Assert.IsEmpty(Company.ReadOnlyMembersOnLoad);
            }
        }
        [Test]
        public void GetBlockedStaticNavigationCollectionMembersOnWrite() {
            CreateThreeObjects();
            using(DbContextSecurityObject dbContextConnectionClass = new DbContextSecurityObject()) {
                var Company = dbContextConnectionClass.SecurityCompany.First();
                Assert.IsEmpty(Company.BlockedMembers);
                Assert.IsEmpty(Company.ReadOnlyMembers);
                Assert.IsEmpty(Company.ReadOnlyMembersOnLoad);
            }
            using(DbContextSecurityObject context = new DbContextSecurityObject()) {
                context.PermissionsContainer.AddMemberPermission<DbContextSecurityObject, SecurityCompany>(
                    SecurityOperation.Write, OperationState.Deny, "CollectionSecurityPerson", (s, t) => true);
                var Company = context.SecurityCompany.Include(p=>p.CollectionSecurityPerson).First();
                Assert.IsEmpty(Company.BlockedMembers);
                Assert.AreEqual(Company.CollectionSecurityPerson.Count, 3);
                Assert.AreEqual("CollectionSecurityPerson", Company.ReadOnlyMembers);
                Assert.AreEqual("CollectionSecurityPerson", Company.ReadOnlyMembersOnLoad);
            }
        }
        [Test]
        public void GetBlockedNavigationCollectionMembersOnWrite() {
            CreateThreeObjects();
            using(DbContextSecurityObject dbContextConnectionClass = new DbContextSecurityObject()) {
                var Company = dbContextConnectionClass.SecurityCompany.First();
                Assert.IsEmpty(Company.BlockedMembers);
                Assert.IsEmpty(Company.ReadOnlyMembers);
                Assert.IsEmpty(Company.ReadOnlyMembersOnLoad);
            }
            using(DbContextSecurityObject context = new DbContextSecurityObject()) {
                context.PermissionsContainer.AddMemberPermission<DbContextSecurityObject, SecurityCompany>(
                    SecurityOperation.Write, OperationState.Deny, "CollectionSecurityPerson", (s, t) => t.Name == "1");
                var Company = context.SecurityCompany.Include(p => p.CollectionSecurityPerson).First();
                Assert.AreEqual(Company.CollectionSecurityPerson.Count, 3);
                Assert.IsEmpty(Company.BlockedMembers);
                Assert.IsEmpty( Company.ReadOnlyMembers);
                Assert.AreEqual("CollectionSecurityPerson", Company.ReadOnlyMembersOnLoad);
            }
        }
        [Test]
        public void GetBlockedNavigationReferenceMembersOnRead() {
            CreateThreeObjects();
            using(DbContextSecurityObject dbContextConnectionClass = new DbContextSecurityObject()) {
                var Person = dbContextConnectionClass.SecurityPerson.First(p=>p.Name == "1");
                Assert.IsEmpty(Person.BlockedMembers);
                Assert.IsEmpty(Person.ReadOnlyMembers);
                Assert.IsEmpty(Person.ReadOnlyMembersOnLoad);
            }
            using(DbContextSecurityObject context = new DbContextSecurityObject()) {
                context.PermissionsContainer.AddMemberPermission<DbContextSecurityObject, SecurityPerson>(
                    SecurityOperation.Read, OperationState.Deny, "SecurityCompany", (s, t) => true);
                var Person = context.SecurityPerson.First(p => p.Name == "1");
                Assert.IsTrue(Person.BlockedMembers.Contains("SecurityCompany"));
                Assert.IsEmpty(Person.ReadOnlyMembers);
                Assert.IsEmpty(Person.ReadOnlyMembersOnLoad);
            }
        }
        [Test]
        public void GetBlockedNavigationForeignKeyReferenceMembersOnRead() {
            CreateThreeObjects();
            using(DbContextSecurityObject dbContextConnectionClass = new DbContextSecurityObject()) {
                var Person = dbContextConnectionClass.SecurityPerson.First(p => p.Name == "1");
                Assert.IsEmpty(Person.BlockedMembers);
                Assert.IsEmpty(Person.ReadOnlyMembers);
                Assert.IsEmpty(Person.ReadOnlyMembersOnLoad);
            }
            using(DbContextSecurityObject context = new DbContextSecurityObject()) {
                context.PermissionsContainer.AddMemberPermission<DbContextSecurityObject, SecurityPerson>(
                    SecurityOperation.Read, OperationState.Deny, "SecurityCompany", (s, t) => true);
                var Person = context.SecurityPerson.First(p => p.Name == "1");
                Assert.IsTrue(Person.BlockedMembers.Contains("SecurityCompanyId"));
                Assert.IsEmpty(Person.ReadOnlyMembers);
                Assert.IsEmpty(Person.ReadOnlyMembersOnLoad);
            }
        }
        [Test]
        public void GetBlockedNavigationStaticReferenceMembersOnWrite() {
            CreateThreeObjects();
            using(DbContextSecurityObject dbContextConnectionClass = new DbContextSecurityObject()) {
                var Person = dbContextConnectionClass.SecurityPerson.First(p => p.Name == "1");
                Assert.IsEmpty(Person.BlockedMembers);
                Assert.IsEmpty(Person.ReadOnlyMembers);
                Assert.IsEmpty(Person.ReadOnlyMembersOnLoad);
            }
            using(DbContextSecurityObject context = new DbContextSecurityObject()) {
                context.PermissionsContainer.AddMemberPermission<DbContextSecurityObject, SecurityPerson>(
                    SecurityOperation.Write, OperationState.Deny, "SecurityCompany", (s, t) => true);
                var Person = context.SecurityPerson.First(p => p.Name == "1");
                Assert.IsEmpty(Person.BlockedMembers);
                Assert.AreEqual("SecurityCompany", Person.ReadOnlyMembers);
                Assert.AreEqual("SecurityCompany", Person.ReadOnlyMembersOnLoad);
            }
        }
        [Test]
        public void GetBlockedNavigationReferenceMembersOnWrite() {
            CreateThreeObjects();
            using(DbContextSecurityObject dbContextConnectionClass = new DbContextSecurityObject()) {
                var Person = dbContextConnectionClass.SecurityPerson.First(p => p.Name == "1");
                Assert.IsEmpty(Person.BlockedMembers);
                Assert.IsEmpty(Person.ReadOnlyMembers);
                Assert.IsEmpty(Person.ReadOnlyMembersOnLoad);
            }
            using(DbContextSecurityObject context = new DbContextSecurityObject()) {
                context.PermissionsContainer.AddMemberPermission<DbContextSecurityObject, SecurityPerson>(
                    SecurityOperation.Write, OperationState.Deny, "SecurityCompany", (s, t) => t.Name == "1");
                var Person = context.SecurityPerson.First(p => p.Name == "1");
                Assert.IsEmpty(Person.BlockedMembers);
                Assert.IsEmpty(Person.ReadOnlyMembers);
                Assert.AreEqual("SecurityCompany", Person.ReadOnlyMembersOnLoad);
            }
        }

        private void CreateThreeObjects() {
            using(DbContextSecurityObject dbContextConnectionClass = new DbContextSecurityObject()) {
                dbContextConnectionClass.ResetDatabase();
                SecurityCompany company = new SecurityCompany();
                company.Name = "1";
                company.Description = "1";
                dbContextConnectionClass.Add(company);
                for(int i = 1; i < 4; i++) {
                    string indexString = i.ToString();
                    SecurityPerson person = new SecurityPerson();
                    person.Name = indexString;
                    person.Description = indexString;
                    person.SecurityCompany = company;
                    dbContextConnectionClass.Add(person);
                }
                dbContextConnectionClass.SaveChanges();
            }
        }
    }

    [TestFixture]
    public class InMemoryBaseSecurityObjectsTests : BaseSecurityObjectsTests {
        [SetUp]
        public void Setup() {
            SecurityTestHelper.CurrentDatabaseProviderType = SecurityTestHelper.DatabaseProviderType.IN_MEMORY;
            base.ClearDatabase();
        }
    }

    [TestFixture]
    public class LocalDb2012BaseSecurityObjectsTests : BaseSecurityObjectsTests {
        [SetUp]
        public void Setup() {
            SecurityTestHelper.CurrentDatabaseProviderType = SecurityTestHelper.DatabaseProviderType.LOCALDB_2012;
            base.ClearDatabase();
        }
    }
}
