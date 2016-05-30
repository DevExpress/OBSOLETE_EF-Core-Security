using DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.Security {
    [TestFixture]
    public class MemberPermissionNavigationProperties {
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
        public void PolicyAllow_NavigationMemberDeny() {
            SecurityTestHelper.InitializeContextWithNavigationProperties();
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                dbContextConnectionClass.PermissionsContainer.SetPermissionPolicy(PermissionPolicy.AllowAllByDefault);

                dbContextConnectionClass.PermissionsContainer.AddMemberPermission(SecurityOperation.Read, OperationState.Deny, "Person", SecurityTestHelper.CompanyNameEqualsOne);

                
                Assert.AreEqual(3, dbContextConnectionClass.Company.Include(p => p.Person).Count());
                Assert.AreEqual(3, dbContextConnectionClass.Persons.Include(p => p.Company).Count());

                Company company1 = dbContextConnectionClass.Company.Include(p => p.Person).First(p => p.CompanyName == "1");
                Assert.IsNull(company1.Person);
                
                dbContextConnectionClass.PermissionsContainer.AddObjectPermission(SecurityOperation.Read, OperationState.Deny, SecurityTestHelper.PersonNameEqualsOne);
                Company company2 = dbContextConnectionClass.Company.Include(p => p.Person).First(p => p.CompanyName == "1");
                Assert.IsNull(company2.Person);
            }
        }
        [Test]
        public void PolicyDeny_NavigationMemberAllow() {
            SecurityTestHelper.InitializeContextWithNavigationProperties();
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                dbContextConnectionClass.PermissionsContainer.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);
                dbContextConnectionClass.PermissionsContainer.AddMemberPermission(SecurityOperation.Read, OperationState.Allow, "Person", SecurityTestHelper.CompanyNameEqualsOne);


                Assert.AreEqual(1, dbContextConnectionClass.Company.Include(p => p.Person).Count());
                Assert.AreEqual(0, dbContextConnectionClass.Persons.Include(p => p.Company).Count());

                Company company1 = dbContextConnectionClass.Company.Include(p => p.Person).First();
                Assert.IsNull(company1.Person);

                dbContextConnectionClass.PermissionsContainer.AddObjectPermission(SecurityOperation.Read, OperationState.Allow, SecurityTestHelper.PersonNameEqualsOne);
                Assert.AreEqual(1, dbContextConnectionClass.Company.Include(p => p.Person).Count());
                Assert.AreEqual(1, dbContextConnectionClass.Persons.Include(p => p.Company).Count());
                company1 = dbContextConnectionClass.Company.Include(p => p.Person).First();
                Assert.IsNotNull(company1.Person);
            }
        }
        [Test]
        public void Read_PolicyDeny_OneMemberAllow_IncludeNavigateObject_SaveChanges() {
            SecurityTestHelper.InitializeContextWithNavigationProperties();
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                dbContextConnectionClass.PermissionsContainer.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);

                dbContextConnectionClass.PermissionsContainer.AddMemberPermission(SecurityOperation.ReadWrite,OperationState.Allow, "Person", SecurityTestHelper.CompanyNameEqualsOne);
                dbContextConnectionClass.PermissionsContainer.AddMemberPermission(SecurityOperation.ReadWrite, OperationState.Allow, "Description", SecurityTestHelper.CompanyTrue);

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
                dbContextConnectionClass.PermissionsContainer.SetPermissionPolicy(PermissionPolicy.AllowAllByDefault);
                dbContextConnectionClass.PermissionsContainer.AddObjectPermission(SecurityOperation.ReadWrite, OperationState.Deny, SecurityTestHelper.PersonNameEqualsOne);

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
    }
}
