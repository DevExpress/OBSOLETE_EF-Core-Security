using DevExpress.EntityFramework.SecurityDataStore.Authorization;
using DevExpress.EntityFramework.SecurityDataStore.Security;
using DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.Authorization {
    [TestFixture]
    public class UserTests {
        [SetUp]
        public void SetUp() {
            using(TestDbContextWithUsers context = new TestDbContextWithUsers()) {
                context.Database.EnsureCreated();
            }
        }
        [TearDown]
        public void TearDown() {
            using(TestDbContextWithUsers context = new TestDbContextWithUsers()) {
                // context.Database.EnsureCreated();
                context.Database.EnsureDeleted();
            }
        }
        // TODO: too complicated test
        [Test]
        public void CreateUserAndRole() {
            using(TestDbContextWithUsers context = new TestDbContextWithUsers()) {
                SecurityUser user = new SecurityUser();
                SecurityRole role = new SecurityRole();
                UserRole userRole = new UserRole { Role = role, User = user };
                context.Add(userRole);
                user.Name = "Admin";
                user.Password = "1";
                role.Name = "AdminRole";
                role.AddMemberPermission<TestDbContextWithUsers, Company>(SecurityOperation.Read, OperationState.Deny, "Description", (s, t) => t.Description == "1");
                Company cmopany = new Company() { CompanyName = "1", Description = "1" };
                context.Add(cmopany);
                context.SaveChanges();
            }
            using(TestDbContextWithUsers context = new TestDbContextWithUsers()) {
                var company = context.Company.First();
                Assert.AreEqual("1", company.CompanyName);
                Assert.AreEqual("1", company.Description);
            }
            using(TestDbContextWithUsers context = new TestDbContextWithUsers()) {
                context.Logon("Admin", "1");
                var company = context.Company.First();
                Assert.AreEqual("1", company.CompanyName);
                Assert.IsNull(company.Description);
            }
        }
    }
}
