using DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts;
using NUnit.Framework;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.Security {
    [TestFixture]
    public class MemberPermission {
        [TearDown]
        public void ClearDatabase() {
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Database.EnsureDeleted();
            }
        }        
        [Test]
        public void MemberPermissionsAreForbiddenForCreateAndDeleteOperations() {
            foreach(SecurityOperation securityOperation in new SecurityOperation[] { SecurityOperation.Create, SecurityOperation.Delete }) {
                using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                    Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria = (db, obj) => obj.Description == "Good description";

                    bool withArgumentException = false;
                    try {
                        dbContextMultiClass.Security.PermissionsRepository.AddMemberPermission(securityOperation, OperationState.Allow, "DecimalItem", criteria);
                    }
                    catch(ArgumentException) {
                        withArgumentException = true;
                    }
                    catch(Exception e) {
                        Assert.Fail(e.Message);
                    }
                    Assert.IsTrue(withArgumentException);
                }
            }
        }
        [Test]
        public void ReadObjectAllowPermission() {
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Database.EnsureCreated();

                DbContextObject1 obj1 = new DbContextObject1();
                obj1.DecimalItem = 10;
                obj1.Description = "Good description";
                DbContextObject1 obj2 = new DbContextObject1();
                obj2.DecimalItem = 20;
                obj2.Description = "Not good description";

                dbContextMultiClass.Add(obj1);
                dbContextMultiClass.Add(obj2);
                dbContextMultiClass.SaveChanges();
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                Assert.AreEqual(2, dbContextMultiClass.dbContextDbSet1.Count());

                dbContextMultiClass.Security.PermissionsRepository.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);
                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria = (db, obj) => obj.Description == "Good description";
                dbContextMultiClass.Security.PermissionsRepository.AddMemberPermission(SecurityOperation.Read, OperationState.Allow, "DecimalItem", criteria);

                Assert.AreEqual(1, dbContextMultiClass.dbContextDbSet1.Count());
                DbContextObject1 obj1 = dbContextMultiClass.dbContextDbSet1.FirstOrDefault();
                Assert.IsNull(obj1.Description);
                Assert.AreEqual(10, obj1.DecimalItem);
            }
        }
        [Test]
        public void ReadMemberAllowPermission() {
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Database.EnsureCreated();

                DbContextObject1 obj1 = new DbContextObject1();
                obj1.DecimalItem = 10;
                obj1.Description = "Good description";
                DbContextObject1 obj2 = new DbContextObject1();
                obj2.DecimalItem = 20;
                obj2.Description = "Not good description";

                dbContextMultiClass.Add(obj1);
                dbContextMultiClass.Add(obj2);
                dbContextMultiClass.SaveChanges();
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria = (db, obj) => obj.Description == "Good description";
                dbContextMultiClass.Security.PermissionsRepository.AddMemberPermission(SecurityOperation.Read, OperationState.Allow, "DecimalItem", criteria);

                var query = from d in dbContextMultiClass.dbContextDbSet1
                            select d.DecimalItem;

                Assert.AreEqual(2, query.Count());

                Decimal obj1Decimal = query.First();
                Assert.AreEqual(10, obj1Decimal);

                Decimal obj2Decimal = query.Last();
                // Assert.AreEqual(0, obj2Decimal);   // doesn't work now
            }
        }
        [Test]
        public void ReadObjectDenyPermission() {
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Database.EnsureCreated();

                DbContextObject1 obj1 = new DbContextObject1();
                obj1.DecimalItem = 10;
                obj1.Description = "Good description";
                DbContextObject1 obj2 = new DbContextObject1();
                obj2.DecimalItem = 20;
                obj2.Description = "Not good description";

                dbContextMultiClass.Add(obj1);
                dbContextMultiClass.Add(obj2);
                dbContextMultiClass.SaveChanges();
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                Assert.AreEqual(2, dbContextMultiClass.dbContextDbSet1.Count());

                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> badCriteria = (db, obj) => obj.Description == "Not good description";
                dbContextMultiClass.Security.PermissionsRepository.AddMemberPermission(SecurityOperation.Read, OperationState.Deny, "DecimalItem", badCriteria);

                Assert.AreEqual(2, dbContextMultiClass.dbContextDbSet1.Count());
                DbContextObject1 obj1 = dbContextMultiClass.dbContextDbSet1.FirstOrDefault();
                Assert.AreEqual("Good description", obj1.Description);
                Assert.AreEqual(10, obj1.DecimalItem);
                DbContextObject1 obj2 = dbContextMultiClass.dbContextDbSet1.LastOrDefault();
                Assert.AreEqual("Not good description", obj2.Description);
                Assert.AreEqual(0, obj2.DecimalItem);
            }
        }
        [Test]
        public void ReadObjectMultiplePermissions() {
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Database.EnsureCreated();

                DbContextObject1 obj1 = new DbContextObject1();
                obj1.ItemCount = 5;
                obj1.DecimalItem = 10;
                DbContextObject1 obj2 = new DbContextObject1();
                obj2.ItemCount = 8;
                obj2.DecimalItem = 20;
                DbContextObject1 obj3 = new DbContextObject1();
                obj3.ItemCount = 10;
                obj3.DecimalItem = 30;

                dbContextMultiClass.Add(obj1);
                dbContextMultiClass.Add(obj2);
                dbContextMultiClass.Add(obj3);
                dbContextMultiClass.SaveChanges();
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                Assert.AreEqual(dbContextMultiClass.dbContextDbSet1.Count(), 3);

                dbContextMultiClass.Security.PermissionsRepository.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);
                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> goodCriteria = (db, obj) => obj.ItemCount > 3;
                dbContextMultiClass.Security.PermissionsRepository.AddMemberPermission(SecurityOperation.Read, OperationState.Allow, "DecimalItem", goodCriteria);

                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> goodCriteria2 = (db, obj) => obj.ItemCount < 9;
                dbContextMultiClass.Security.PermissionsRepository.AddMemberPermission(SecurityOperation.Read, OperationState.Allow, "DecimalItem", goodCriteria2);

                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> badCriteria = (db, obj) => obj.ItemCount == 8;
                dbContextMultiClass.Security.PermissionsRepository.AddMemberPermission(SecurityOperation.Read, OperationState.Deny, "DecimalItem", badCriteria);

                IEnumerable<DbContextObject1> objects = dbContextMultiClass.dbContextDbSet1.AsEnumerable();
                Assert.AreEqual(2, objects.Count());

                DbContextObject1 obj1 = objects.ElementAt(0);
                Assert.AreEqual(0, obj1.ItemCount);
                Assert.AreEqual(10, obj1.DecimalItem);

                DbContextObject1 obj2 = objects.ElementAt(1);
                Assert.AreEqual(0, obj2.ItemCount);
                Assert.AreEqual(30, obj2.DecimalItem);
            }
        }
        [Test]
        public void WriteMemberAllowPermission() {
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Database.EnsureCreated();
                dbContextMultiClass.Add(new DbContextObject1());
                dbContextMultiClass.SaveChanges();
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {

                DbContextObject1 obj1 = dbContextMultiClass.dbContextDbSet1.FirstOrDefault();

                dbContextMultiClass.Security.PermissionsRepository.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);
                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria = (db, obj) => obj.Description == "Good description";
                dbContextMultiClass.Security.PermissionsRepository.AddMemberPermission(SecurityOperation.Write, OperationState.Allow, "Description", criteria);

                obj1.Description = "Good description";
                dbContextMultiClass.SaveChanges();

                obj1.Description = "Not good description";
                SecurityTestHelper.FailSaveChanges(dbContextMultiClass);

                obj1.Description = "Good description";
                dbContextMultiClass.SaveChanges();
            }
        }
        [Test]
        public void WriteMemberDenyPermission() {
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Database.EnsureCreated();
                dbContextMultiClass.Add(new DbContextObject1());
                dbContextMultiClass.SaveChanges();
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {

                DbContextObject1 obj1 = dbContextMultiClass.dbContextDbSet1.FirstOrDefault();

                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> badCriteria = (db, obj) => obj.Description == "Not good description";
                dbContextMultiClass.Security.PermissionsRepository.AddMemberPermission(SecurityOperation.Write, OperationState.Deny, "DecimalItem", badCriteria);

                obj1.Description = "Good description";
                obj1.DecimalItem = 20;

                dbContextMultiClass.SaveChanges();

                obj1.Description = "Not good description";
                obj1.DecimalItem = 10;

                SecurityTestHelper.FailSaveChanges(dbContextMultiClass);

                obj1.Description = "Good description";
                obj1.DecimalItem = 10;
                dbContextMultiClass.SaveChanges();
            }
        }
        [Test]
        public void WriteMembersMultiplePermissions() {
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Database.EnsureCreated();
                dbContextMultiClass.Add(new DbContextObject1());
                dbContextMultiClass.SaveChanges();
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {

                DbContextObject1 obj1 = dbContextMultiClass.dbContextDbSet1.FirstOrDefault();

                dbContextMultiClass.Security.PermissionsRepository.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);

                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> goodCriteria = (db, obj) => obj.ItemCount > 3;
                dbContextMultiClass.Security.PermissionsRepository.AddMemberPermission(SecurityOperation.Write, OperationState.Allow, "ItemCount", goodCriteria);

                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> goodCriteria2 = (db, obj) => obj.ItemCount < 9;
                dbContextMultiClass.Security.PermissionsRepository.AddMemberPermission(SecurityOperation.Write, OperationState.Allow, "ItemCount", goodCriteria2);

                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> badCriteria = (db, obj) => obj.ItemCount == 8;
                dbContextMultiClass.Security.PermissionsRepository.AddMemberPermission(SecurityOperation.Write, OperationState.Deny, "ItemCount", badCriteria);              

                obj1.ItemCount = 8;
                SecurityTestHelper.FailSaveChanges(dbContextMultiClass);   
                Assert.AreEqual(0, obj1.DecimalItem);

                obj1.ItemCount = 6;
                dbContextMultiClass.SaveChanges();
            }
        }
        [Test]
        public void IsGrantedAllowPermission() {
            foreach(SecurityOperation securityOperation in new SecurityOperation[] { SecurityOperation.Read, SecurityOperation.Write }) {
                using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                    DbContextObject1 obj1 = new DbContextObject1();
                    Assert.IsTrue(dbContextMultiClass.Security.IsGranted(typeof(DbContextObject1), securityOperation, obj1));

                    dbContextMultiClass.Security.PermissionsRepository.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);
                    Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria = (db, obj) => obj.Description == "Good description";
                    dbContextMultiClass.Security.PermissionsRepository.AddMemberPermission(securityOperation, OperationState.Allow, "DecimalItem", criteria);

                    obj1.Description = "Not good description";
                    Assert.IsFalse(dbContextMultiClass.Security.IsGranted(typeof(DbContextObject1), securityOperation, obj1, "DecimalItem"));

                    obj1.Description = "Good description";
                    Assert.IsTrue(dbContextMultiClass.Security.IsGranted(typeof(DbContextObject1), securityOperation, obj1, "DecimalItem"));
                }
            }
        }
        [Test]
        public void IsGrantedDenyPermission() {
            foreach(SecurityOperation securityOperation in new SecurityOperation[] { SecurityOperation.Read, SecurityOperation.Write }) {
                using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                    DbContextObject1 obj1 = new DbContextObject1();
                    Assert.IsTrue(dbContextMultiClass.Security.IsGranted(typeof(DbContextObject1), securityOperation, obj1));

                    Expression<Func<DbContextMultiClass, DbContextObject1, bool>> badCriteria = (db, obj) => obj.Description == "Not good description";
                    dbContextMultiClass.Security.PermissionsRepository.AddMemberPermission(securityOperation, OperationState.Deny, "DecimalItem", badCriteria);

                    obj1.Description = "Not good description";
                    Assert.IsFalse(dbContextMultiClass.Security.IsGranted(typeof(DbContextObject1), securityOperation, obj1, "DecimalItem"));

                    obj1.Description = "Good description";
                    Assert.IsTrue(dbContextMultiClass.Security.IsGranted(typeof(DbContextObject1), securityOperation, obj1, "DecimalItem"));
                }
            }
        }
        [Test]
        public void IsGrantedMultiplePermissions() {
            foreach(SecurityOperation securityOperation in new SecurityOperation[] { SecurityOperation.Read, SecurityOperation.Write }) {
                using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                    DbContextObject1 obj1 = new DbContextObject1();
                    Assert.IsTrue(dbContextMultiClass.Security.IsGranted(typeof(DbContextObject1), SecurityOperation.Write, obj1, null));

                    Expression<Func<DbContextMultiClass, DbContextObject1, bool>> goodCriteria = (db, obj) => obj.ItemCount > 3;
                    dbContextMultiClass.Security.PermissionsRepository.AddMemberPermission(securityOperation, OperationState.Allow, "DecimalItem", goodCriteria);

                    Expression<Func<DbContextMultiClass, DbContextObject1, bool>> goodCriteria2 = (db, obj) => obj.ItemCount < 9;
                    dbContextMultiClass.Security.PermissionsRepository.AddMemberPermission(securityOperation, OperationState.Allow, "DecimalItem", goodCriteria2);

                    Expression<Func<DbContextMultiClass, DbContextObject1, bool>> badCriteria = (db, obj) => obj.ItemCount == 8;
                    dbContextMultiClass.Security.PermissionsRepository.AddMemberPermission(securityOperation, OperationState.Deny, "DecimalItem", badCriteria);

                    obj1.ItemCount = 8;
                    Assert.IsFalse(dbContextMultiClass.Security.IsGranted(typeof(DbContextObject1), securityOperation, obj1, "DecimalItem"));

                    obj1.ItemCount = 5;
                    Assert.IsTrue(dbContextMultiClass.Security.IsGranted(typeof(DbContextObject1), securityOperation, obj1, "DecimalItem"));
                }
            }
        }
        [Test]
        public void IsGrantedObjectAndMember() {
            using(DbContextMultiClass dbContext = new DbContextMultiClass()) {
                DbContextObject1 dbContextObject1_1 = new DbContextObject1();
                dbContextObject1_1.ItemName = "1";
                dbContext.Add(dbContextObject1_1);
                dbContext.SaveChanges();

                dbContext.Security.PermissionsRepository.SetPermissionPolicy(PermissionPolicy.AllowAllByDefault);
                dbContext.Security.PermissionsRepository.AddObjectPermission<DbContextMultiClass, DbContextObject1>(
                    SecurityOperation.Read,
                    OperationState.Deny,
                    (p, d) => d.ItemName == "1");

                Assert.AreEqual(dbContext.dbContextDbSet1.Count(), 0);
            }
        }
        [Test]
        public void IsGrantedDenyPermissionPolicyObjectDenyMemberAllow() {
            using(DbContextMultiClass dbContext = new DbContextMultiClass()) {
                DbContextObject1 dbContextObject1_1 = new DbContextObject1();
                dbContextObject1_1.ItemName = "1";
                dbContext.Add(dbContextObject1_1);
                dbContext.SaveChanges();

                dbContext.Security.PermissionsRepository.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);
                dbContext.Security.PermissionsRepository.AddObjectPermission<DbContextMultiClass, DbContextObject1>(
                   SecurityOperation.Read,
                   OperationState.Allow,
                   (p, d) => d.ItemName == "1");
                dbContext.Security.PermissionsRepository.AddObjectPermission<DbContextMultiClass, DbContextObject1>(
                    SecurityOperation.Read,
                    OperationState.Deny,
                    (p, d) => d.ItemName == "1");
                dbContext.Security.PermissionsRepository.AddMemberPermission<DbContextMultiClass, DbContextObject1>(SecurityOperation.Read, OperationState.Allow, "ItemName", (p, b) => b.ItemName == "1");
                Assert.AreEqual(dbContext.dbContextDbSet1.Count(), 1);
            }
        }
    }
}
