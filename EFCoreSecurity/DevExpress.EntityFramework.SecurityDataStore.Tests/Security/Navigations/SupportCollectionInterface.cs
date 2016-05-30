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
    public class SupportCollectionInterface {
        [SetUp]
        public void SetUp() {
            OneToManyICollection_One.Count = 0;
            using(var context = new DbContextICollectionProperty()) {
                context.Database.EnsureCreated();
            }
        }
        [TearDown]
        public void TearDown() {
            using(var context = new DbContextICollectionProperty()) {
                context.Database.EnsureDeleted();
            }
        }
        [Test]
        public void CreateSecurityCollectionObject_Read() {
            CreateObjects();
            using(DbContextICollectionProperty context = new DbContextICollectionProperty()) {
                OneToManyICollection_One.Count = 0;
                OneToManyICollection_Many.Count = 0;
                OneToManyICollection_One one_1 = context.OneToManyICollection_One.First();
                Assert.AreEqual(one_1.Name, "1");
                Assert.AreEqual(one_1.Collection.Count, 0);
                Assert.AreEqual(OneToManyICollection_One.Count, 2);
                Assert.AreEqual(OneToManyICollection_Many.Count, 0);
                OneToManyICollection_One.Count = 0;
                OneToManyICollection_Many.Count = 0;


                OneToManyICollection_One one_2 = context.OneToManyICollection_One.Include(p => p.Collection).First();
                Assert.AreEqual(one_2.Name, "1");
                Assert.AreEqual(one_2.Collection.Count, 3);
                Assert.AreEqual(OneToManyICollection_One.Count, 1); // recreate securityObjects
                Assert.AreEqual(OneToManyICollection_Many.Count, 6);
            }
        }

        [Test]
        public void CreateSecurityCollectionObject_DenyObjectInCollection_Read() {
            CreateObjects();
            using(DbContextICollectionProperty context = new DbContextICollectionProperty()) {
                OneToManyICollection_One.Count = 0;
                OneToManyICollection_Many.Count = 0;
                context.PermissionsContainer.AddObjectPermission<DbContextICollectionProperty, OneToManyICollection_Many>(SecurityOperation.Read,
                    OperationState.Deny, (p, d) => d.Name == "3");

                OneToManyICollection_One one = context.OneToManyICollection_One.Include(p => p.Collection).First();
                Assert.AreEqual(one.Collection.Count, 2);
                Assert.AreEqual(OneToManyICollection_One.Count, 2);
                Assert.AreEqual(OneToManyICollection_Many.Count, 5);// 3 real object + 2 securityObject
            }
        }

        [Test]
        public void CreateSecurityCollectionObject_DenyObjectMemberInCollection_Read() {
            CreateObjects();
            using(DbContextICollectionProperty context = new DbContextICollectionProperty()) {
                context.PermissionsContainer.AddMemberPermission<DbContextICollectionProperty, OneToManyICollection_Many>(SecurityOperation.Read,
                    OperationState.Deny, "Name", (p, d) => d.Name == "3");
                OneToManyICollection_One.Count = 0;
                OneToManyICollection_Many.Count = 0;

                OneToManyICollection_One one = context.OneToManyICollection_One.Include(p => p.Collection).First();
                Assert.IsTrue(one.Collection.Any(p => string.IsNullOrEmpty(p.Name)));
                Assert.AreEqual(OneToManyICollection_One.Count, 2);
                Assert.AreEqual(OneToManyICollection_Many.Count, 6);
            }
        }
        [Test]
        public void CreateSecurityCollectionObject_DenyMemberCollection_Read() {
            CreateObjects();
            using(DbContextICollectionProperty context = new DbContextICollectionProperty()) {
                context.PermissionsContainer.AddMemberPermission<DbContextICollectionProperty, OneToManyICollection_One>(SecurityOperation.Read,
                    OperationState.Deny, "Collection", (p, d) => true);
                OneToManyICollection_One.Count = 0;
                OneToManyICollection_Many.Count = 0;

                OneToManyICollection_One one = context.OneToManyICollection_One.Include(p => p.Collection).First();

                Assert.IsNull(one.Collection);
                Assert.AreEqual(OneToManyICollection_One.Count, 2);
                Assert.AreEqual(OneToManyICollection_Many.Count, 6);
            }
        }

        [Test]
        public void CreateSecurityCollectionObject_DenyNavigationMember_Read() {
            CreateObjects();
            using(DbContextICollectionProperty context = new DbContextICollectionProperty()) {
                context.PermissionsContainer.AddMemberPermission<DbContextICollectionProperty, OneToManyICollection_Many>(SecurityOperation.Read,
                    OperationState.Deny, "One", (p, d) => true);
                OneToManyICollection_One.Count = 0;
                OneToManyICollection_Many.Count = 0;

                OneToManyICollection_Many one = context.OneToManyICollection_Many.Include(p => p.One).First();

                Assert.IsNull(one.One);
                Assert.AreEqual(OneToManyICollection_One.Count, 2);
                Assert.AreEqual(OneToManyICollection_Many.Count, 2);
            }
        }
        [Test]
        public void AddItemInCollection() {
            CreateObjects();
            using(DbContextICollectionProperty context = new DbContextICollectionProperty()) {
                OneToManyICollection_One.Count = 0;
                OneToManyICollection_Many.Count = 0;
                OneToManyICollection_One one = context.OneToManyICollection_One.Include(p => p.Collection).First();
                OneToManyICollection_Many oneToManyICollection_Many = new OneToManyICollection_Many();
                oneToManyICollection_Many.Name = "4";
                one.Collection.Add(oneToManyICollection_Many);
                context.SaveChanges();
                Assert.AreEqual(2, OneToManyICollection_One.Count);
                Assert.AreEqual(8, OneToManyICollection_Many.Count);
            }
            using(DbContextICollectionProperty context = new DbContextICollectionProperty()) {
                OneToManyICollection_One.Count = 0;
                OneToManyICollection_Many.Count = 0;
                OneToManyICollection_One one = context.OneToManyICollection_One.Include(p => p.Collection).First();
                Assert.AreEqual(4, one.Collection.Count);
                Assert.AreEqual(2, OneToManyICollection_One.Count);
                Assert.AreEqual(8, OneToManyICollection_Many.Count);
            }
        }
        [Test]
        public void AddItemInCollection_DenyCreateObject() {
            CreateObjects();
            using(DbContextICollectionProperty context = new DbContextICollectionProperty()) {
                context.PermissionsContainer.AddObjectPermission<DbContextICollectionProperty, OneToManyICollection_Many>(
                    SecurityOperation.Create, OperationState.Deny, (c, t) => t.Name == "4");
                OneToManyICollection_One.Count = 0;
                OneToManyICollection_Many.Count = 0;
                OneToManyICollection_One one = context.OneToManyICollection_One.Include(p => p.Collection).First();
                OneToManyICollection_Many oneToManyICollection_Many = new OneToManyICollection_Many();
                oneToManyICollection_Many.Name = "4";
                one.Collection.Add(oneToManyICollection_Many);
                AssertFail(context);
                one.Collection.Add(oneToManyICollection_Many);
                oneToManyICollection_Many.Name = "5";
                context.SaveChanges();
                Assert.AreEqual(2, OneToManyICollection_One.Count);
                Assert.AreEqual(9, OneToManyICollection_Many.Count);
            }
            using(DbContextICollectionProperty context = new DbContextICollectionProperty()) {
                OneToManyICollection_One.Count = 0;
                OneToManyICollection_Many.Count = 0;
                OneToManyICollection_One one = context.OneToManyICollection_One.Include(p => p.Collection).First();
                Assert.AreEqual(4, one.Collection.Count);
                Assert.IsTrue(one.Collection.Any(p => p.Name == "5"));
                Assert.AreEqual(2, OneToManyICollection_One.Count);
                Assert.AreEqual(8, OneToManyICollection_Many.Count);
            }
        }

        [Test]
        public void ChengeItemInCollection_DenyChengeObject() {
            CreateObjects();
            using(DbContextICollectionProperty context = new DbContextICollectionProperty()) {
                context.PermissionsContainer.AddObjectPermission<DbContextICollectionProperty, OneToManyICollection_Many>(
                    SecurityOperation.Write, OperationState.Deny, (c, t) => t.Name == "1");
                OneToManyICollection_One.Count = 0;
                OneToManyICollection_Many.Count = 0;
                OneToManyICollection_One one = context.OneToManyICollection_One.Include(p => p.Collection).First();
                one.Collection.First(p => p.Name == "3").Name = "1";
                AssertFail(context);
                one.Collection.First(p => p.Name == "3").Name = "5";
                context.SaveChanges();
                Assert.AreEqual(2, OneToManyICollection_One.Count);
                Assert.AreEqual(6, OneToManyICollection_Many.Count);
            }
        }

        [Test]
        public void ChengeItemInCollection_DenyRemoveObject() {
            CreateObjects();
            using(DbContextICollectionProperty context = new DbContextICollectionProperty()) {
                context.PermissionsContainer.AddObjectPermission<DbContextICollectionProperty, OneToManyICollection_Many>(
                    SecurityOperation.Delete, OperationState.Deny, (c, t) => t.Name == "1");
                OneToManyICollection_One one = context.OneToManyICollection_One.Include(p => p.Collection).First();
                context.Remove(one.Collection.First(p => p.Name == "1"));
                AssertFail(context);
                context.Remove(one.Collection.First(p => p.Name == "2"));
                context.SaveChanges();
            }
            using(DbContextICollectionProperty context = new DbContextICollectionProperty()) {
                OneToManyICollection_One.Count = 0;
                OneToManyICollection_Many.Count = 0;
                OneToManyICollection_One one = context.OneToManyICollection_One.Include(p => p.Collection).First();
                Assert.AreEqual(2, one.Collection.Count);
                Assert.AreEqual(2, OneToManyICollection_One.Count);
                Assert.AreEqual(4, OneToManyICollection_Many.Count);
            }
        }
        [Test]
        public void ChengeItemInCollection_RemoveObjectInCollection() {
            CreateObjects();
            using(DbContextICollectionProperty context = new DbContextICollectionProperty()) {
                context.PermissionsContainer.AddMemberPermission<DbContextICollectionProperty, OneToManyICollection_One>(
                    SecurityOperation.Write, OperationState.Deny, "Collection", (c, t) => t.Name == "1");
                OneToManyICollection_One one = context.OneToManyICollection_One.Include(p => p.Collection).First();
                one.Collection.Remove(one.Collection.First(p => p.Name == "1"));
                AssertFail(context);
            }

            using(DbContextICollectionProperty context = new DbContextICollectionProperty()) {
                OneToManyICollection_One one = context.OneToManyICollection_One.Include(p => p.Collection).First();
                Assert.AreEqual(3, one.Collection.Count);
                Assert.AreEqual(3, context.OneToManyICollection_Many.Count());
            }
        }
        [Test]
        public void ChengeItemInCollection_DenyRemoveObjectInCollection() {
            CreateObjects();
            using(DbContextICollectionProperty context = new DbContextICollectionProperty()) {
                context.PermissionsContainer.AddObjectPermission<DbContextICollectionProperty, OneToManyICollection_Many>(
                    SecurityOperation.Write, OperationState.Deny, (c, t) => t.Name == "1");
                OneToManyICollection_One one = context.OneToManyICollection_One.Include(p => p.Collection).First();
                one.Collection.Remove(one.Collection.First(p => p.Name == "1"));
                AssertFail(context);
            }

            using(DbContextICollectionProperty context = new DbContextICollectionProperty()) {
                OneToManyICollection_One one = context.OneToManyICollection_One.Include(p => p.Collection).First();
                Assert.AreEqual(3, one.Collection.Count);
                Assert.AreEqual(3, context.OneToManyICollection_Many.Count());
            }
        }
        [Test]
        public void ChengeItemInCollection_DenyAddExistingObjectInCollection() {
            CreateObjectsWithOutCollection();
            using(DbContextICollectionProperty context = new DbContextICollectionProperty()) {
                context.PermissionsContainer.AddObjectPermission<DbContextICollectionProperty, OneToManyICollection_Many>(
                    SecurityOperation.Write, OperationState.Deny, (c, t) => t.Name == "1");
                OneToManyICollection_One one = context.OneToManyICollection_One.Include(p => p.Collection).First();
                OneToManyICollection_Many many = context.OneToManyICollection_Many.First(p => p.Name == "1");
                one.Collection.Add(many);
                AssertFail(context);
                OneToManyICollection_Many many_ = context.OneToManyICollection_Many.First(p => p.Name == "2");
                one.Collection.Add(many_);
                context.SaveChanges();
            }

            using(DbContextICollectionProperty context = new DbContextICollectionProperty()) {
                OneToManyICollection_One one = context.OneToManyICollection_One.Include(p => p.Collection).First();
                Assert.AreEqual(1, one.Collection.Count);
                Assert.AreEqual(3, context.OneToManyICollection_Many.Count());
            }
        }
         [Test]
        public void ChengeItemInCollection_DenyWriteCollectionAddExistingObjectInCollection() {
            CreateObjectsWithOutCollection();
            using(DbContextICollectionProperty context = new DbContextICollectionProperty()) {
                context.PermissionsContainer.AddMemberPermission<DbContextICollectionProperty, OneToManyICollection_One>(
                    SecurityOperation.Write, OperationState.Deny,"Collection", (c, t) => true);
                OneToManyICollection_One one = context.OneToManyICollection_One.Include(p => p.Collection).First();
                OneToManyICollection_Many many = context.OneToManyICollection_Many.First(p => p.Name == "1");
                one.Collection.Add(many);
                AssertFail(context);
            }

            using(DbContextICollectionProperty context = new DbContextICollectionProperty()) {
                OneToManyICollection_One one = context.OneToManyICollection_One.Include(p => p.Collection).First();
                Assert.AreEqual(0, one.Collection.Count);
                Assert.AreEqual(3, context.OneToManyICollection_Many.Count());
            }
        }
        [Test]
        public void ChangeItemInCollection_DenyAddingNewObjectInCollection() {
            CreateObjects();
            using(DbContextICollectionProperty context = new DbContextICollectionProperty()) {
                context.PermissionsContainer.AddMemberPermission<DbContextICollectionProperty, OneToManyICollection_One>(
                  SecurityOperation.Write, OperationState.Deny, "Collection", (c, t) => true);
                OneToManyICollection_One one = context.OneToManyICollection_One.Include(p => p.Collection).First();
                OneToManyICollection_Many many = new OneToManyICollection_Many();
                many.Name = "4";
                one.Collection.Add(many);
                AssertFail(context);
            }
            using(DbContextICollectionProperty context = new DbContextICollectionProperty()) {
                OneToManyICollection_One one = context.OneToManyICollection_One.Include(p => p.Collection).First();
                Assert.AreEqual(3, one.Collection.Count);
                Assert.AreEqual(3, context.OneToManyICollection_Many.Count());
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

        public static void CreateObjects() {
            using(DbContextICollectionProperty context = new DbContextICollectionProperty()) {
                OneToManyICollection_One one = new OneToManyICollection_One();
                one.Name = "1";
                context.Add(one);
                for(int i = 1; i < 4; i++) {
                    string indexString = i.ToString();
                    OneToManyICollection_Many company = new OneToManyICollection_Many();
                    company.Name = i.ToString();
                    one.Collection.Add(company);
                }
                context.SaveChanges();
            }
        }
        public static void CreateObjectsWithOutCollection() {
            using(DbContextICollectionProperty context = new DbContextICollectionProperty()) {
                OneToManyICollection_One one = new OneToManyICollection_One();
                one.Name = "1";
                context.Add(one);
                for(int i = 1; i < 4; i++) {
                    string indexString = i.ToString();
                    OneToManyICollection_Many company = new OneToManyICollection_Many();
                    company.Name = i.ToString();
                    context.Add(company);
                }
                context.SaveChanges();
            }
        }
    }
}
