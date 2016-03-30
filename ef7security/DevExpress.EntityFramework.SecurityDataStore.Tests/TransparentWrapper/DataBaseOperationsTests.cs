using DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests {
    [TestFixture]
    public class ScenarioTest_DeleteAndCreateDataBaseInDxProvider {
        [SetUp]
        public void SetUp() {
            var realDbcontext = new DbContextMultiClass().MakeRealDbContext(); ;
            realDbcontext.Database.EnsureDeleted();
            realDbcontext.Database.EnsureCreated();
            var securityDbcontext = new DbContextMultiClass();
            realDbcontext.Database.EnsureDeleted();
            realDbcontext.Database.EnsureCreated();
        }
        [Test]
        public void NativeDeleteDatabase() {
            DeleteDatabase(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void DxProviderDeleteDatabase() {
            DeleteDatabase(() => new DbContextMultiClass());
        }
        private void DeleteDatabase(Func<DbContextMultiClass> createDbContext) {

            using(var context = createDbContext()) {
                context.dbContextDbSet1.Add(new DbContextObject1());
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                Assert.IsNotNull(context.dbContextDbSet1.Single());
                context.Database.EnsureDeleted();
            }
            using(var context = createDbContext()) {
                try {
                    context.dbContextDbSet1.Single();
                    Assert.Fail();
                }
                catch {
                }
            }
        }
        [Test]
        public void TestGen() {
            var v = TestGenClass(typeof(Guid));
        }
        private object TestGenClass(Type type) {
            if(type.IsValueType) {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        [Test]
        public void NativeCreateDatabase() {
            CreateDatabase(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void DxProviderCreateDatabase() {
            CreateDatabase(() => new DbContextMultiClass());
        }
        private void CreateDatabase(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Database.EnsureDeleted();
            }
            using(var context = createDbContext()) {
                context.Database.EnsureCreated();
            }
            using(var context = createDbContext()) {
                context.dbContextDbSet1.Add(new DbContextObject1());
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                Assert.IsNotNull(context.dbContextDbSet1.Single());
            }
        }
        [Test]
        public void ChangeTracker_Native() {
            ChangeTrackerTest(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void ChangeTracker_Native_DXProvider() {
            ChangeTrackerTest(() => new DbContextMultiClass());
        }
        public void ChangeTrackerTest(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.SaveChanges();
                Assert.AreEqual(context.ChangeTracker.Entries().Count(), 1);
            }
            using(var context = createDbContext()) {
                Assert.AreEqual(context.ChangeTracker.Entries().Count(), 0);
                DbContextObject1.Count = 0;
                Assert.IsNotNull(context.dbContextDbSet1.Single());
                Assert.IsNotNull(context.ChangeTracker.Entries().Single());
            }
        }
    }
}
