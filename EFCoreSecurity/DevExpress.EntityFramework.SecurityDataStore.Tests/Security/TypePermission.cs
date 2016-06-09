using NUnit.Framework;
using System;
using System.Linq;
using DevExpress.EntityFramework.SecurityDataStore.Tests.Helpers;
using DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts;
using DevExpress.EntityFramework.SecurityDataStore.Security;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.Security {
    [TestFixture]
    public abstract class TypePermissionTests {
        [Test]
        public void ReadType() {
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.ResetDatabase();
                dbContextMultiClass.Add(new DbContextObject1());
                dbContextMultiClass.SaveChanges();
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                Assert.IsNotNull(dbContextMultiClass.dbContextDbSet1.FirstOrDefault());
                dbContextMultiClass.PermissionsContainer.SetTypePermission<DbContextObject1>(SecurityOperation.Read, OperationState.Deny);
                Assert.IsNull(dbContextMultiClass.dbContextDbSet1.FirstOrDefault());
            }
        }
        [Test]
        public void WriteType() {
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.ResetDatabase();
                dbContextMultiClass.Add(new DbContextObject1());
                dbContextMultiClass.SaveChanges();
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.PermissionsContainer.SetTypePermission<DbContextObject1>(SecurityOperation.Write, OperationState.Deny);
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
                dbContextMultiClass.ResetDatabase();
                dbContextMultiClass.PermissionsContainer.SetTypePermission<DbContextObject1>(SecurityOperation.Create, OperationState.Deny);
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
                dbContextMultiClass.ResetDatabase();
                dbContextMultiClass.Add(new DbContextObject1());
                dbContextMultiClass.SaveChanges();
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.PermissionsContainer.SetTypePermission<DbContextObject1>(SecurityOperation.Delete, OperationState.Deny);
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
                    dbContextMultiClass.PermissionsContainer.SetTypePermission<DbContextObject1>(securityOperation, OperationState.Deny);
                    Assert.IsFalse(dbContextMultiClass.Security.IsGranted(typeof(DbContextObject1), securityOperation));
                }
            }
        }
    }

    [TestFixture]
    public class InMemoryTypePermissionTests : TypePermissionTests {
        [SetUp]
        public void Setup() {
            SecurityTestHelper.CurrentDatabaseProviderType = SecurityTestHelper.DatabaseProviderType.IN_MEMORY;
        }
    }

    [TestFixture]
    public class LocalDb2012TypePermissionTests : TypePermissionTests {
        [SetUp]
        public void Setup() {
            SecurityTestHelper.CurrentDatabaseProviderType = SecurityTestHelper.DatabaseProviderType.LOCALDB_2012;
        }
    }
}
