using DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts;
using DevExpress.EntityFramework.SecurityDataStore.Security;
using NUnit.Framework;
using System;
using System.Linq;


namespace DevExpress.EntityFramework.SecurityDataStore.Tests.TransparentWrapper {
    [TestFixture]
    public class SimpleSecurityTest {
        [Test]
        public void ReadType() {
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Database.EnsureCreated();
                dbContextMultiClass.Add(new DbContextObject1());
                dbContextMultiClass.SaveChanges();
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                Assert.IsNotNull(dbContextMultiClass.dbContextDbSet1.FirstOrDefault());
                dbContextMultiClass.Security.PermissionsContainer.SetTypePermission<DbContextObject1>(SecurityOperation.Read, OperationState.Deny);
                Assert.IsNull(dbContextMultiClass.dbContextDbSet1.FirstOrDefault());
            }
        }
        [Test]
        public void WriteType() {
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Database.EnsureCreated();
                dbContextMultiClass.Add(new DbContextObject1());
                dbContextMultiClass.SaveChanges();
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Security.PermissionsContainer.SetTypePermission<DbContextObject1>(SecurityOperation.Write, OperationState.Deny);
                DbContextObject1 obj = dbContextMultiClass.dbContextDbSet1.FirstOrDefault();
                obj.ItemCount = 100;
                try {
                    dbContextMultiClass.SaveChanges();
                    Assert.Fail("Fail");
                }
                catch(Exception e) {
                    Assert.AreNotEqual("Fail", e.Message);
                }
            }
        }
        [Test]
        public void CreateType() {
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Security.PermissionsContainer.SetTypePermission<DbContextObject1>(SecurityOperation.Create, OperationState.Deny);
                dbContextMultiClass.Add(new DbContextObject1());
                try {
                    dbContextMultiClass.SaveChanges();
                    Assert.Fail("Fail");
                }
                catch(Exception e) {
                    Assert.AreNotEqual("Fail", e.Message);
                }
            }
        }
        [Test]
        public void DeleteType() {
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Database.EnsureCreated();
                dbContextMultiClass.Add(new DbContextObject1());
                dbContextMultiClass.SaveChanges();
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Security.PermissionsContainer.SetTypePermission<DbContextObject1>(SecurityOperation.Delete, OperationState.Deny);
                DbContextObject1 dbContextObject1 = dbContextMultiClass.dbContextDbSet1.First();
                dbContextMultiClass.Remove(dbContextObject1);
                try {
                    dbContextMultiClass.SaveChanges();
                    Assert.Fail("Fail");
                }
                catch(Exception e) {
                    Assert.AreNotEqual("Fail", e.Message);
                }
            }
        }
        [Test]
        public void TypeIsGranted() {
            foreach(SecurityOperation securityOperation in Enum.GetValues(typeof(SecurityOperation))) {
                using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                    Assert.IsTrue(dbContextMultiClass.Security.IsGranted(typeof(DbContextObject1), securityOperation));
                    dbContextMultiClass.Security.PermissionsContainer.SetTypePermission<DbContextObject1>(securityOperation, OperationState.Deny);
                    Assert.IsFalse(dbContextMultiClass.Security.IsGranted(typeof(DbContextObject1), securityOperation));
                }
            }
        }
    }
}
