using DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.Security.BusinessObjects {
    [TestFixture]
    public class BaseSecurityObjectsTest {
        [SetUp]
        public void SetUp() {
            using(DbContextSecurityObject context = new DbContextSecurityObject()) {
                context.Database.EnsureCreated();
            }
        }
        [TearDown]
        public void TearDown() {
            using(DbContextSecurityObject context = new DbContextSecurityObject()) {
                context.Database.EnsureDeleted();
            }
        }
        [Test]
        public void GetBlockedMembersRead() {
            CreateThreeObject();
            using(DbContextSecurityObject dbContextConnectionClass = new DbContextSecurityObject()) {
                var Company = dbContextConnectionClass.SecurityCompany.First();
                Assert.IsEmpty(Company.BlockedMembers);
                Assert.IsEmpty(Company.ReadOnlyMembers);
                Assert.IsEmpty(Company.ReadOnlyMembersOnLoad);
            }
            using(DbContextSecurityObject context = new DbContextSecurityObject()) {
                context.Security.AddMemberPermission<DbContextSecurityObject, SecurityCompany>(
                    SecurityOperation.Read, OperationState.Deny, "Name", (s,t) => true);
                context.Security.AddMemberPermission<DbContextSecurityObject, SecurityCompany>(
                    SecurityOperation.Read, OperationState.Deny, "Description", (s, t) => t.Description == "1");
                var Company = context.SecurityCompany.First();
                Assert.AreEqual("Description;Name",  Company.BlockedMembers);
                Assert.IsEmpty(Company.ReadOnlyMembers);
                Assert.IsEmpty(Company.ReadOnlyMembersOnLoad);
            }
        }
        [Test]
        public void GetBlockedMembersWrite() {
            CreateThreeObject();
            using(DbContextSecurityObject dbContextConnectionClass = new DbContextSecurityObject()) {
                var Company = dbContextConnectionClass.SecurityCompany.First();
                Assert.IsEmpty(Company.BlockedMembers);
                Assert.IsEmpty(Company.ReadOnlyMembers);
                Assert.IsEmpty(Company.ReadOnlyMembersOnLoad);
            }
            using(DbContextSecurityObject context = new DbContextSecurityObject()) {
                context.Security.AddMemberPermission<DbContextSecurityObject, SecurityCompany>(
                    SecurityOperation.Write, OperationState.Deny, "Name", (s, t) => true);
                context.Security.AddMemberPermission<DbContextSecurityObject, SecurityCompany>(
                    SecurityOperation.Write, OperationState.Deny, "Description", (s, t) => t.Description == "1");
                var Company = context.SecurityCompany.First();
                Assert.IsEmpty(Company.BlockedMembers);
                Assert.AreEqual("Name", Company.ReadOnlyMembers);
                Assert.AreEqual("Description;Name", Company.ReadOnlyMembersOnLoad);
            }
        }
        [Test]
        public void GetBlockedNavigationCollectionMembersRead() {
            CreateThreeObject();
            using(DbContextSecurityObject dbContextConnectionClass = new DbContextSecurityObject()) {
                var Company = dbContextConnectionClass.SecurityCompany.First();
                Assert.IsEmpty(Company.BlockedMembers);
                Assert.IsEmpty(Company.ReadOnlyMembers);
                Assert.IsEmpty(Company.ReadOnlyMembersOnLoad);
            }
            using(DbContextSecurityObject context = new DbContextSecurityObject()) {
                context.Security.AddMemberPermission<DbContextSecurityObject, SecurityCompany>(
                    SecurityOperation.Read, OperationState.Deny, "CollectionSecurityPerson", (s, t) => true);               
                var Company = context.SecurityCompany.Include(p => p.CollectionSecurityPerson).First();
                Assert.IsNull(Company.CollectionSecurityPerson);
                Assert.AreEqual("CollectionSecurityPerson", Company.BlockedMembers);
                Assert.IsEmpty(Company.ReadOnlyMembers);
                Assert.IsEmpty(Company.ReadOnlyMembersOnLoad);
            }
        }
        [Test]
        public void GetBlockedStaticNavigationCollectionMembersWrite() {
            CreateThreeObject();
            using(DbContextSecurityObject dbContextConnectionClass = new DbContextSecurityObject()) {
                var Company = dbContextConnectionClass.SecurityCompany.First();
                Assert.IsEmpty(Company.BlockedMembers);
                Assert.IsEmpty(Company.ReadOnlyMembers);
                Assert.IsEmpty(Company.ReadOnlyMembersOnLoad);
            }
            using(DbContextSecurityObject context = new DbContextSecurityObject()) {
                context.Security.AddMemberPermission<DbContextSecurityObject, SecurityCompany>(
                    SecurityOperation.Write, OperationState.Deny, "CollectionSecurityPerson", (s, t) => true);
                var Company = context.SecurityCompany.Include(p=>p.CollectionSecurityPerson).First();
                Assert.IsEmpty(Company.BlockedMembers);
                Assert.AreEqual(Company.CollectionSecurityPerson.Count, 3);
                Assert.AreEqual("CollectionSecurityPerson", Company.ReadOnlyMembers);
                Assert.AreEqual("CollectionSecurityPerson", Company.ReadOnlyMembersOnLoad);
            }
        }
        [Test]
        public void GetBlockedNavigationCollectionMembersWrite() {
            CreateThreeObject();
            using(DbContextSecurityObject dbContextConnectionClass = new DbContextSecurityObject()) {
                var Company = dbContextConnectionClass.SecurityCompany.First();
                Assert.IsEmpty(Company.BlockedMembers);
                Assert.IsEmpty(Company.ReadOnlyMembers);
                Assert.IsEmpty(Company.ReadOnlyMembersOnLoad);
            }
            using(DbContextSecurityObject context = new DbContextSecurityObject()) {
                context.Security.AddMemberPermission<DbContextSecurityObject, SecurityCompany>(
                    SecurityOperation.Write, OperationState.Deny, "CollectionSecurityPerson", (s, t) => t.Name == "1");
                var Company = context.SecurityCompany.Include(p => p.CollectionSecurityPerson).First();
                Assert.AreEqual(Company.CollectionSecurityPerson.Count, 3);
                Assert.IsEmpty(Company.BlockedMembers);
                Assert.IsEmpty( Company.ReadOnlyMembers);
                Assert.AreEqual("CollectionSecurityPerson", Company.ReadOnlyMembersOnLoad);
            }
        }
        [Test]
        public void GetBlockedNavigationReferenceMembersRead() {
            CreateThreeObject();
            using(DbContextSecurityObject dbContextConnectionClass = new DbContextSecurityObject()) {
                var Person = dbContextConnectionClass.SecurityPerson.First(p=>p.Name == "1");
                Assert.IsEmpty(Person.BlockedMembers);
                Assert.IsEmpty(Person.ReadOnlyMembers);
                Assert.IsEmpty(Person.ReadOnlyMembersOnLoad);
            }
            using(DbContextSecurityObject context = new DbContextSecurityObject()) {
                context.Security.AddMemberPermission<DbContextSecurityObject, SecurityPerson>(
                    SecurityOperation.Read, OperationState.Deny, "SecurityCompany", (s, t) => true);
                var Person = context.SecurityPerson.First(p => p.Name == "1");
                Assert.AreEqual("SecurityCompany", Person.BlockedMembers);
                Assert.IsEmpty(Person.ReadOnlyMembers);
                Assert.IsEmpty(Person.ReadOnlyMembersOnLoad);
            }
        }
        [Test]
        public void GetBlockedNavigationStaticReferenceMembersWrite() {
            CreateThreeObject();
            using(DbContextSecurityObject dbContextConnectionClass = new DbContextSecurityObject()) {
                var Person = dbContextConnectionClass.SecurityPerson.First(p => p.Name == "1");
                Assert.IsEmpty(Person.BlockedMembers);
                Assert.IsEmpty(Person.ReadOnlyMembers);
                Assert.IsEmpty(Person.ReadOnlyMembersOnLoad);
            }
            using(DbContextSecurityObject context = new DbContextSecurityObject()) {
                context.Security.AddMemberPermission<DbContextSecurityObject, SecurityPerson>(
                    SecurityOperation.Write, OperationState.Deny, "SecurityCompany", (s, t) => true);
                var Person = context.SecurityPerson.First(p => p.Name == "1");
                Assert.IsEmpty(Person.BlockedMembers);
                Assert.AreEqual("SecurityCompany", Person.ReadOnlyMembers);
                Assert.AreEqual("SecurityCompany", Person.ReadOnlyMembersOnLoad);
            }
        }
        [Test]
        public void GetBlockedNavigationReferenceMembersWrite() {
            CreateThreeObject();
            using(DbContextSecurityObject dbContextConnectionClass = new DbContextSecurityObject()) {
                var Person = dbContextConnectionClass.SecurityPerson.First(p => p.Name == "1");
                Assert.IsEmpty(Person.BlockedMembers);
                Assert.IsEmpty(Person.ReadOnlyMembers);
                Assert.IsEmpty(Person.ReadOnlyMembersOnLoad);
            }
            using(DbContextSecurityObject context = new DbContextSecurityObject()) {
                context.Security.AddMemberPermission<DbContextSecurityObject, SecurityPerson>(
                    SecurityOperation.Write, OperationState.Deny, "SecurityCompany", (s, t) => t.Name == "1");
                var Person = context.SecurityPerson.First(p => p.Name == "1");
                Assert.IsEmpty(Person.BlockedMembers);
                Assert.IsEmpty(Person.ReadOnlyMembers);
                Assert.AreEqual("SecurityCompany", Person.ReadOnlyMembersOnLoad);
            }
        }
        private void CreateThreeObject() {
            using(DbContextSecurityObject dbContextConnectionClass = new DbContextSecurityObject()) {
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
}
