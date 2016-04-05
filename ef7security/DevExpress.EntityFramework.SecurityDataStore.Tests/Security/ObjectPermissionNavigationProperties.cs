using DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.Security {
    [TestFixture]
    public class ObjectPermissionNavigationProperties {
        [SetUp]
        public void SetUp() {
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                dbContextConnectionClass.Database.EnsureCreated();
            }
        }
        [TearDown]
        public void TearDown() {
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                dbContextConnectionClass.Database.EnsureDeleted();
            }
        }
        [Test]
        public void Read_PolicyAllow_TestDependencyPrincipalProperty() {
            SecurityTestHelper.InitializeContextWithNavigationProperties();
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                Company company = dbContextConnectionClass.Company.Include(p => p.Person).Include(p => p.Collection).First(p => p.CompanyName == "1");
                Assert.IsNotNull(company.Person);
                Assert.AreEqual(3, company.Collection.Count);
                Person persons = dbContextConnectionClass.Persons.Include(p => p.Company).First(p => p.PersonName == "1");
                Assert.IsNotNull(persons.Company);
                Assert.IsNotNull(persons.One);
            }
        }

        [Test]
        public void Read_PolicyDeny_OneObjectAllow() {
            SecurityTestHelper.InitializeContextWithNavigationProperties();
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                dbContextConnectionClass.Security.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);
                dbContextConnectionClass.Security.AddObjectPermission(SecurityOperation.Read, OperationState.Allow, SecurityTestHelper.CompanyNameEqualsOne);
                Assert.AreEqual(1, dbContextConnectionClass.Company.Count());
            }
        }

        [Test]
        public void Read_PolicyAllow_OneObjectDeny() {
            SecurityTestHelper.InitializeContextWithNavigationProperties();
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                dbContextConnectionClass.Security.SetPermissionPolicy(PermissionPolicy.AllowAllByDefault);
                dbContextConnectionClass.Security.AddObjectPermission(SecurityOperation.Read, OperationState.Deny, SecurityTestHelper.CompanyNameEqualsOne);                
                Assert.AreEqual(2, dbContextConnectionClass.Company.Count());
            }
        }

        [Test]
        public void Read_PolicyDeny_OneObjectAllow_IncludeNavigateObject() {
            SecurityTestHelper.InitializeContextWithNavigationProperties();
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                dbContextConnectionClass.Security.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);
                dbContextConnectionClass.Security.AddObjectPermission(SecurityOperation.Read, OperationState.Allow, SecurityTestHelper.CompanyNameEqualsOne);
                Assert.AreEqual(1, dbContextConnectionClass.Company.Include(p => p.Person).Count());
                Assert.AreEqual(0, dbContextConnectionClass.Persons.Include(p => p.Company).Count());
                Company company = dbContextConnectionClass.Company.Include(p => p.Person).First();

                Assert.IsNull(company.Person);
            }
        }
        [Test]
        public void Read_PolicyAllow_OneObjectDeny_IncludeNavigateObject() {
            SecurityTestHelper.InitializeContextWithNavigationProperties();
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                dbContextConnectionClass.Security.SetPermissionPolicy(PermissionPolicy.AllowAllByDefault);
                dbContextConnectionClass.Security.AddObjectPermission(SecurityOperation.Read, OperationState.Deny, SecurityTestHelper.PersonNameEqualsOne);
                Assert.AreEqual(3, dbContextConnectionClass.Company.Include(p => p.Person).Count());
                Assert.AreEqual(2, dbContextConnectionClass.Persons.Include(p => p.Company).Count());

                Company company1 = dbContextConnectionClass.Company.Include(p => p.Person).First(p => p.CompanyName == "1");
                Assert.IsNull(company1.Person);
                Company company2 = dbContextConnectionClass.Company.Include(p => p.Person).First(p => p.CompanyName == "2");
                Assert.IsNotNull(company2.Person);
            }
        }
        [Test]
        public void Read_PolicyDeny_OneObjectAllow_IncludeNavigateObject_CheckCollection() {
            SecurityTestHelper.InitializeContextWithNavigationProperties();
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                dbContextConnectionClass.Security.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);
                dbContextConnectionClass.Security.AddObjectPermission(SecurityOperation.Read, OperationState.Allow, SecurityTestHelper.CompanyNameEqualsOne);

                Company company1 = dbContextConnectionClass.Company.Include(p => p.Person).Single();
                Assert.IsNull(company1.Person);
                Assert.AreEqual(0, company1.Collection.Count);

                dbContextConnectionClass.Security.AddObjectPermission(SecurityOperation.Read, OperationState.Allow, SecurityTestHelper.PersonNameEqualsOne);

                company1 = dbContextConnectionClass.Company.Include(p => p.Person).Single();

                Assert.AreEqual(1, company1.Collection.Count);
                Assert.IsNotNull(company1.Person);

                Person persons = dbContextConnectionClass.Persons.Include(p => p.Company).Single();

                Assert.IsNotNull(persons.Company);
                Assert.IsNotNull(persons.One);
            }
        }
        [Test]
        public void Read_PolicyDeny_OneObjectAllow_IncludeNavigateObject_SaveChanges() {
            SecurityTestHelper.InitializeContextWithNavigationProperties();
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                dbContextConnectionClass.Security.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);
                dbContextConnectionClass.Security.AddObjectPermission(SecurityOperation.ReadWrite, OperationState.Allow, SecurityTestHelper.CompanyNameEqualsOne);

                Company company1 = dbContextConnectionClass.Company.Include(p => p.Person).Single(d => d.CompanyName == "1");
                Assert.IsNull(company1.Person);

                company1.Description = "5";
                dbContextConnectionClass.SaveChanges();
            }

            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                var company1 = dbContextConnectionClass.Company.Include(p => p.Person).First(p => p.Description == "5");
                Assert.IsNotNull(company1.Person);
            }
        }
        [Test]
        public void Read_PolicyAllow_OneObjectDeny_IncludeNavigateObject_SaveChanges() {
            SecurityTestHelper.InitializeContextWithNavigationProperties();
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                dbContextConnectionClass.Security.SetPermissionPolicy(PermissionPolicy.AllowAllByDefault);
                dbContextConnectionClass.Security.AddObjectPermission(SecurityOperation.ReadWrite, OperationState.Deny, SecurityTestHelper.PersonNameEqualsOne);

                Company company1 = dbContextConnectionClass.Company.Include(p => p.Person).Single(d => d.CompanyName == "1");
                Assert.IsNull(company1.Person);

                company1.Description = "5";
                dbContextConnectionClass.SaveChanges();
            }

            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                var company1 = dbContextConnectionClass.Company.Include(p => p.Person).First(p => p.Description == "5");
                Assert.IsNotNull(company1.Person);
            }
        }
        [Test]
        public void Read_OneObjectDeny_ObjectCount() {
            SecurityTestHelper.InitializeContextWithNavigationProperties();
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                dbContextConnectionClass.Security.AddObjectPermission(SecurityOperation.ReadWrite, OperationState.Deny, SecurityTestHelper.PersonNameEqualsOne);
                Company company1 = dbContextConnectionClass.Company.Include(p => p.Collection).Single(d => d.CompanyName == "1");
                Assert.AreEqual(2, company1.Collection.Count);
            }
        }
        [Test]
        public void Read_Collection_ObjectCount() {
            SecurityTestHelper.InitializeContextWithNavigationProperties();
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                Company company = dbContextConnectionClass.Company.Include(p => p.Person).First(p => p.CompanyName == "1");
                Assert.AreEqual(company.Collection.Count, 1);
                company = dbContextConnectionClass.Company.Include(p => p.Collection).First(p => p.CompanyName == "1");
                Assert.AreEqual(company.Collection.Count, 3);
            }
        }
        [Test]
        public void Modify_FakeCollectionObject() {
            SecurityTestHelper.InitializeContextWithNavigationProperties();
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                dbContextConnectionClass.Security.AddMemberPermission(SecurityOperation.Read, OperationState.Deny, "PersonName", SecurityTestHelper.PersonTrue);
                Company company = dbContextConnectionClass.Company.Include(p => p.Collection).First(p => p.CompanyName == "1");
                Person person = dbContextConnectionClass.Persons.First();
                Assert.IsNull(person.PersonName);
                Person personCollection = company.Collection.First();
                Assert.IsNull(personCollection.PersonName);
            }
        }
    }
}
