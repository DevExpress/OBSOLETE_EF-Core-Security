using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.Security {
    [TestFixture]
    public class SecurityInfoTests {
        [TearDown]
        public void ClearDatabase() {
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Database.EnsureDeleted();
            }
        }
        [Test]
        public void ReadBlockedMembersFromObjectWithOneBlockedMember() {
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
                dbContextMultiClass.Security.PermissionsContainer.AddMemberPermission(SecurityOperation.Read, OperationState.Deny, "DecimalItem", badCriteria);

                Assert.AreEqual(2, dbContextMultiClass.dbContextDbSet1.Count());
                DbContextObject1 obj1 = dbContextMultiClass.dbContextDbSet1.FirstOrDefault();
                Assert.AreEqual("Good description", obj1.Description);
                Assert.AreEqual(10, obj1.DecimalItem);

                IList<string> obj1BlockedMembers = dbContextMultiClass.Entry(obj1).GetBlockedMembers();
                Assert.IsNotNull(obj1BlockedMembers);
                Assert.AreEqual(0, obj1BlockedMembers.Count());

                DbContextObject1 obj2 = dbContextMultiClass.dbContextDbSet1.LastOrDefault();
                Assert.AreEqual("Not good description", obj2.Description);
                Assert.AreEqual(0, obj2.DecimalItem);

                IList<string> obj2BlockedMembers = dbContextMultiClass.Entry(obj2).GetBlockedMembers();
                Assert.IsNotNull(obj2BlockedMembers);
                Assert.AreEqual(1, obj2BlockedMembers.Count());
                Assert.AreEqual("DecimalItem", obj2BlockedMembers.First());
            }
        }
        [Test]
        public void ReadBlockedMembersFromObjectWithMultipleBlockedMembers() {
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Database.EnsureCreated();

                DbContextObject1 obj1 = new DbContextObject1();
                obj1.ItemName = "Object 1 name";
                obj1.DecimalItem = 10;
                obj1.Description = "Good description";
                DbContextObject1 obj2 = new DbContextObject1();
                obj2.ItemName = "Object 2 name";
                obj2.DecimalItem = 20;
                obj2.Description = "Not good description";

                dbContextMultiClass.Add(obj1);
                dbContextMultiClass.Add(obj2);
                dbContextMultiClass.SaveChanges();
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                Assert.AreEqual(2, dbContextMultiClass.dbContextDbSet1.Count());

                Expression<Func<DbContextMultiClass, DbContextObject1, bool>> badCriteria = (db, obj) => obj.Description == "Not good description";
                dbContextMultiClass.Security.PermissionsContainer.AddMemberPermission(SecurityOperation.Read, OperationState.Deny, "DecimalItem", badCriteria);
                dbContextMultiClass.Security.PermissionsContainer.AddMemberPermission(SecurityOperation.Read, OperationState.Deny, "ItemName", badCriteria);

                Assert.AreEqual(2, dbContextMultiClass.dbContextDbSet1.Count());
                DbContextObject1 obj1 = dbContextMultiClass.dbContextDbSet1.FirstOrDefault();
                Assert.AreEqual("Good description", obj1.Description);
                Assert.AreEqual("Object 1 name", obj1.ItemName);
                Assert.AreEqual(10, obj1.DecimalItem);

                IList<string> obj1BlockedMembers = dbContextMultiClass.Entry(obj1).GetBlockedMembers();
                Assert.IsNotNull(obj1BlockedMembers);
                Assert.AreEqual(0, obj1BlockedMembers.Count());

                DbContextObject1 obj2 = dbContextMultiClass.dbContextDbSet1.LastOrDefault();
                Assert.AreEqual("Not good description", obj2.Description);
                Assert.AreEqual(null, obj2.ItemName);
                Assert.AreEqual(0, obj2.DecimalItem);

                IList<string> obj2BlockedMembers = dbContextMultiClass.Entry(obj2).GetBlockedMembers();
                Assert.IsNotNull(obj2BlockedMembers);
                Assert.AreEqual(2, obj2BlockedMembers.Count());
                Assert.AreEqual("DecimalItem", obj2BlockedMembers.First());
                Assert.AreEqual("ItemName", obj2BlockedMembers.Last());
            }
        }
        [Test]
        public void ReadBlockedMembersFromObjectWithBlockedNavigationProperty() {
            SecurityTestHelper.InitializeContextWithNavigationProperties();
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                dbContextConnectionClass.Security.PermissionsContainer.SetPermissionPolicy(PermissionPolicy.AllowAllByDefault);

                dbContextConnectionClass.Security.PermissionsContainer.AddMemberPermission(SecurityOperation.Read, OperationState.Deny, "Person", SecurityTestHelper.CompanyNameEqualsTwo);
                
                Assert.AreEqual(3, dbContextConnectionClass.Company.Include(p => p.Person).Count());

                Company company1 = dbContextConnectionClass.Company.Include(p => p.Person).First(p => p.CompanyName == "1");
                Assert.IsNotNull(company1.Person);

                IList<string> company1BlockedMembers = dbContextConnectionClass.Entry(company1).GetBlockedMembers();
                Assert.IsNotNull(company1BlockedMembers);
                Assert.AreEqual(0, company1BlockedMembers.Count());

                Company company2 = dbContextConnectionClass.Company.Include(p => p.Person).First(p => p.CompanyName == "2");
                Assert.IsNull(company2.Person);

                IList<string> company2BlockedMembers = dbContextConnectionClass.Entry(company2).GetBlockedMembers();
                Assert.IsNotNull(company2BlockedMembers);
                Assert.AreEqual(1, company2BlockedMembers.Count());
                Assert.AreEqual("Person", company2BlockedMembers.First());
            }
        }
        [Test]
        public void ReadBlockedMembersFromObjectWithBlockedCollectionProperty() {
            SecurityTestHelper.InitializeContextWithNavigationPropertiesAndCollections();
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                dbContextConnectionClass.Security.PermissionsContainer.SetPermissionPolicy(PermissionPolicy.AllowAllByDefault);

                dbContextConnectionClass.Security.PermissionsContainer.AddMemberPermission(SecurityOperation.Read, OperationState.Deny, "Collection", SecurityTestHelper.CompanyNameEqualsTwo);

                Assert.AreEqual(3, dbContextConnectionClass.Company.Include(p => p.Collection).Count());

                Company company1 = dbContextConnectionClass.Company.Include(p => p.Collection).First(p => p.CompanyName == "1");
                Assert.IsNotNull(company1.Collection);
                Assert.AreEqual(1, company1.Collection.Count());

                IList<string> company1BlockedMembers = dbContextConnectionClass.Entry(company1).GetBlockedMembers();
                Assert.IsNotNull(company1BlockedMembers);
                Assert.AreEqual(0, company1BlockedMembers.Count());

                Company company2 = dbContextConnectionClass.Company.Include(p => p.Collection).First(p => p.CompanyName == "2");
                Assert.IsNull(company2.Collection);

                IList<string> company2BlockedMembers = dbContextConnectionClass.Entry(company2).GetBlockedMembers();
                Assert.IsNotNull(company2BlockedMembers);
                Assert.AreEqual(1, company2BlockedMembers.Count());
                Assert.AreEqual("Collection", company2BlockedMembers.First());
            }
        }
        [Test]
        public void ReadBlockedMembersFromBaseSecurityObject() {
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Database.EnsureCreated();

                DbContextBaseSecurityObject obj1 = new DbContextBaseSecurityObject();
                obj1.DecimalItem = 10;
                obj1.Description = "Good description";
                DbContextBaseSecurityObject obj2 = new DbContextBaseSecurityObject();
                obj2.DecimalItem = 20;
                obj2.Description = "Not good description";

                dbContextMultiClass.Add(obj1);
                dbContextMultiClass.Add(obj2);
                dbContextMultiClass.SaveChanges();
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                Assert.AreEqual(2, dbContextMultiClass.dbContextBaseSecurityObjectDbSet.Count());

                Expression<Func<DbContextMultiClass, DbContextBaseSecurityObject, bool>> badCriteria = (db, obj) => obj.Description == "Not good description";
                dbContextMultiClass.Security.PermissionsContainer.AddMemberPermission(SecurityOperation.Read, OperationState.Deny, "DecimalItem", badCriteria);

                Assert.AreEqual(2, dbContextMultiClass.dbContextBaseSecurityObjectDbSet.Count());
                DbContextBaseSecurityObject obj1 = dbContextMultiClass.dbContextBaseSecurityObjectDbSet.FirstOrDefault();
                Assert.AreEqual("Good description", obj1.Description);
                Assert.AreEqual(10, obj1.DecimalItem);

                IEnumerable<string> obj1BlockedMembers = obj1.BlockedMembers;
                Assert.IsNotNull(obj1BlockedMembers);
                Assert.AreEqual(0, obj1BlockedMembers.Count());

                DbContextBaseSecurityObject obj2 = dbContextMultiClass.dbContextBaseSecurityObjectDbSet.LastOrDefault();
                Assert.AreEqual("Not good description", obj2.Description);
                Assert.AreEqual(0, obj2.DecimalItem);

                IEnumerable<string> obj2BlockedMembers = obj2.BlockedMembers;
                Assert.IsNotNull(obj2BlockedMembers);
                Assert.AreEqual(1, obj2BlockedMembers.Count());
                Assert.AreEqual("DecimalItem", obj2BlockedMembers.First());
            }
        }
        [Test]
        public void ReadBlockedMembersFromISecurityEntityObject() {
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                dbContextMultiClass.Database.EnsureCreated();

                DbContextISecurityEntityObject obj1 = new DbContextISecurityEntityObject();
                obj1.DecimalItem = 10;
                obj1.Description = "Good description";
                DbContextISecurityEntityObject obj2 = new DbContextISecurityEntityObject();
                obj2.DecimalItem = 20;
                obj2.Description = "Not good description";

                dbContextMultiClass.Add(obj1);
                dbContextMultiClass.Add(obj2);
                dbContextMultiClass.SaveChanges();
            }
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass()) {
                Assert.AreEqual(2, dbContextMultiClass.dbContextISecurityEntityDbSet.Count());

                Expression<Func<DbContextMultiClass, DbContextISecurityEntityObject, bool>> badCriteria = (db, obj) => obj.Description == "Not good description";
                dbContextMultiClass.Security.PermissionsContainer.AddMemberPermission(SecurityOperation.Read, OperationState.Deny, "DecimalItem", badCriteria);

                Assert.AreEqual(2, dbContextMultiClass.dbContextISecurityEntityDbSet.Count());
                DbContextISecurityEntityObject obj1 = dbContextMultiClass.dbContextISecurityEntityDbSet.FirstOrDefault();
                Assert.AreEqual("Good description", obj1.Description);
                Assert.AreEqual(10, obj1.DecimalItem);

                IEnumerable<string> obj1BlockedMembers = obj1.BlockedMembers;
                Assert.IsNotNull(obj1BlockedMembers);
                Assert.AreEqual(0, obj1BlockedMembers.Count());

                DbContextISecurityEntityObject obj2 = dbContextMultiClass.dbContextISecurityEntityDbSet.LastOrDefault();
                Assert.AreEqual("Not good description", obj2.Description);
                Assert.AreEqual(0, obj2.DecimalItem);

                IEnumerable<string> obj2BlockedMembers = obj2.BlockedMembers;
                Assert.IsNotNull(obj2BlockedMembers);
                Assert.AreEqual(1, obj2BlockedMembers.Count());
                Assert.AreEqual("DecimalItem", obj2BlockedMembers.First());
            }
        }
    }
}
