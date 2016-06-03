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
    public class NavigationOneReference {
        [SetUp]
        public void SetUp() {
            using(DbContextNavigationReferenceObject context = new DbContextNavigationReferenceObject()) {
                context.Database.EnsureCreated();
            }
        }
        [TearDown]
        public void TearDown() {
            using(DbContextNavigationReferenceObject context = new DbContextNavigationReferenceObject()) {
                context.Database.EnsureDeleted();
            }
        }
        [Test]
        public void DenyWriteNavigationPeoperty() {
            CreateTwoObjects();
            using(DbContextNavigationReferenceObject context = new DbContextNavigationReferenceObject()) {

                context.PermissionsContainer.AddMemberPermission<DbContextNavigationReferenceObject, One>(SecurityOperation.Write, OperationState.Deny, "Reference",
                    (s, t) => true);
                One one = context.One.Include(p=>p.Reference).First(p => p.Name == "1");
                one.Reference = null;
                AssertFail(context);
            }

        }
        [Test]
        public void DenyWriteNavigationPeoperty_AddExisting_NavigationCriteria() {
            CreateTwoObjects();
            using(DbContextNavigationReferenceObject context = new DbContextNavigationReferenceObject()) {
                context.PermissionsContainer.AddMemberPermission<DbContextNavigationReferenceObject, One>(SecurityOperation.Write, OperationState.Deny, "Reference",
                    (s, t) => t.Reference != null && t.Reference.Name == "2");
                One one = context.One.Include(p => p.Reference).First(p => p.Name == "1");
                one.Reference = null;
                context.SaveChanges();
                one.Reference = one;
                context.SaveChanges();
                One one2 = context.One.Include(p => p.Reference).First(p => p.Name == "2");
                one.Reference = one2;
                AssertFail(context);
            }
        }

        [Test]
        public void DenyWriteNavigationPeoperty_AddNew_NavigationCriteria() {
            CreateTwoObjects();
            using(DbContextNavigationReferenceObject context = new DbContextNavigationReferenceObject()) {
                context.PermissionsContainer.AddMemberPermission<DbContextNavigationReferenceObject, One>(SecurityOperation.Write, OperationState.Deny, "Reference",
                    (s, t) => t.Reference != null && t.Reference.Name == "2");
                One one = context.One.Include(p => p.Reference).First(p => p.Name == "1");
                one.Reference = null;
                context.SaveChanges();
                one.Reference = one;
                context.SaveChanges();
                One one2 = new One() { Name = "2" };
                one.Reference = one2;
                AssertFail(context);
            }
        }
        private void AssertFail(DbContext context) {
            try {
                context.SaveChanges();
                Assert.Fail("Fail");
            }
            catch(Exception e) {
                Assert.AreNotEqual("Fail", e.Message);
            }
        }
        private void CreateTwoObjects() {
            using(DbContextNavigationReferenceObject context = new DbContextNavigationReferenceObject()) {
                context.ResetDatabase();
                One one1 = new One();
                one1.Name = "1";
                context.Add(one1);
                One one2 = new One();
                one2.Name = "2";
                one1.Reference = one2;
                context.SaveChanges();
            }
        }
    }
}
