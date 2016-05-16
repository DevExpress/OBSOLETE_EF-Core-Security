using DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.TransparentWrapper {
    [TestFixture]
    public class DbSetTestExtensionParamInLinqQuery {
        private int GetInt() => 2;
        [SetUp]
        public void SetUp() {
            DbContextObject1.Count = 0;
            using(DbContextMultiClass dbContextMultiClass = new DbContextMultiClass().MakeRealDbContext()) {
                dbContextMultiClass.Database.EnsureDeleted();
                dbContextMultiClass.Database.EnsureCreated();
                for(int i = 1; i <= 10; i++) {
                    dbContextMultiClass.dbContextDbSet1.Add(new DbContextObject1() { ItemCount = i });
                }
                dbContextMultiClass.SaveChanges();
            }
        }
        [Test]
        public void AllNative() {
            All(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void AllDXProvider() {
            All(() => new DbContextMultiClass());
        }
        private void All(Func<DbContextMultiClass> createDbContext) {
            using(var dbContext = createDbContext()) {
                var firstItem = dbContext.dbContextDbSet1.First();
                Assert.IsTrue(dbContext.dbContextDbSet1.All(p => true));
                Assert.IsFalse(dbContext.dbContextDbSet1.All(p => p == dbContext.dbContextDbSet1.First()));
                Assert.IsFalse(dbContext.dbContextDbSet1.All(p => p == firstItem));
                Assert.IsFalse(dbContext.dbContextDbSet1.All(p => p.ItemCount == dbContext.dbContextDbSet1.First().ItemCount));
                Assert.IsFalse(dbContext.dbContextDbSet1.All(p => GetInt() == 4));
            }
        }
        [Test]
        public void AnyNative() {
            Any(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void AnyDXProvider() {
            Any(() => new DbContextMultiClass());
        }
        private void Any(Func<DbContextMultiClass> createDbContext) {
            using(var dbContext = createDbContext()) {
                var firstItem = dbContext.dbContextDbSet1.First();
                Assert.IsTrue(dbContext.dbContextDbSet1.Any(p => true));
                Assert.IsTrue(dbContext.dbContextDbSet1.Any(p => p == dbContext.dbContextDbSet1.First()));
                //Assert.IsTrue(dbContext.dbContextDbSet1.Any(p => p == firstItem));
                Assert.IsTrue(dbContext.dbContextDbSet1.Any(p => p.ItemCount == dbContext.dbContextDbSet1.First().ItemCount));
                Assert.IsFalse(dbContext.dbContextDbSet1.Any(p => GetInt() == 4));
            }
        }
        [Test]
        public void CountNative() {
            Count(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void CountDXProvider() {
            Count(() => new DbContextMultiClass());
        }
        private void Count(Func<DbContextMultiClass> createDbContext) {
            using(var dbContext = createDbContext()) {
                var firstItem = dbContext.dbContextDbSet1.First();
                Assert.AreEqual(dbContext.dbContextDbSet1.Count(p => true), 10);
                Assert.AreEqual(dbContext.dbContextDbSet1.Count(p => p == dbContext.dbContextDbSet1.First()), 1);
           //     Assert.AreEqual(1, dbContext.dbContextDbSet1.Count(p => p == firstItem));
                Assert.AreEqual(dbContext.dbContextDbSet1.Count(p => p.ItemCount == dbContext.dbContextDbSet1.First().ItemCount), 1);
                Assert.AreEqual(dbContext.dbContextDbSet1.Count(p => GetInt() == 4), 0);
            }
        }
        [Test]
        public void FirstNative() {
            First(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void FirstDXProvider() {
            First(() => new DbContextMultiClass());
        }
        private void First(Func<DbContextMultiClass> createDbContext) {
            using(var dbContext = createDbContext()) {
                var firstItem = dbContext.dbContextDbSet1.First();
                Assert.IsNotNull(dbContext.dbContextDbSet1.First(p => true));
                Assert.IsNotNull(dbContext.dbContextDbSet1.First(p => p == dbContext.dbContextDbSet1.First()));
               // Assert.IsNotNull(dbContext.dbContextDbSet1.First(p => p == firstItem));
                Assert.IsNotNull(dbContext.dbContextDbSet1.First(p => p.ItemCount == dbContext.dbContextDbSet1.First().ItemCount));
                Assert.IsNotNull(dbContext.dbContextDbSet1.First(p => GetInt() == 2));
            }
        }
        [Test]
        public void FirstOrDefaultNative() {
            FirstOrDefault(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void FirstOrDefaultDXProvider() {
            FirstOrDefault(() => new DbContextMultiClass());
        }
        private void FirstOrDefault(Func<DbContextMultiClass> createDbContext) {
            using(var dbContext = createDbContext()) {
                var firstItem = dbContext.dbContextDbSet1.FirstOrDefault();
                Assert.IsNotNull(dbContext.dbContextDbSet1.FirstOrDefault(p => true));
                Assert.IsNotNull(dbContext.dbContextDbSet1.FirstOrDefault(p => p == dbContext.dbContextDbSet1.First()));
              //  Assert.IsNotNull(dbContext.dbContextDbSet1.FirstOrDefault(p => p == firstItem));
                Assert.IsNotNull(dbContext.dbContextDbSet1.FirstOrDefault(p => p.ItemCount == dbContext.dbContextDbSet1.First().ItemCount));
                Assert.IsNull(dbContext.dbContextDbSet1.FirstOrDefault(p => GetInt() == 4));
            }
        }
        [Test]
        public void MaxNative() {
            Max(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void MaxDXProvider() {
            Max(() => new DbContextMultiClass());
        }
        private void Max(Func<DbContextMultiClass> createDbContext) {
            using(var dbContext = createDbContext()) {
                var firstItem = dbContext.dbContextDbSet1.First();
                Assert.AreEqual(dbContext.dbContextDbSet1.Max(p => p.ItemCount), 10);
                Assert.AreEqual(dbContext.dbContextDbSet1.Max(p => dbContext.dbContextDbSet1.First().ItemCount), 1);
                Assert.AreEqual(dbContext.dbContextDbSet1.Max(p => firstItem.ItemCount), 1);
                Assert.AreEqual(dbContext.dbContextDbSet1.Max(p => dbContext.dbContextDbSet1.First().ItemCount), 1);
                Assert.AreEqual(dbContext.dbContextDbSet1.Max(p => GetInt()), 2);
            }
        }
        [Test]
        public void MinNative() {
            Min(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void MinDXProvider() {
            Min(() => new DbContextMultiClass());
        }
        private void Min(Func<DbContextMultiClass> createDbContext) {
            using(var dbContext = createDbContext()) {
                var firstItem = dbContext.dbContextDbSet1.First();
                Assert.AreEqual(dbContext.dbContextDbSet1.Min(p => p.ItemCount), 1);
                Assert.AreEqual(dbContext.dbContextDbSet1.Min(p => dbContext.dbContextDbSet1.First().ItemCount), 1);
                Assert.AreEqual(dbContext.dbContextDbSet1.Min(p => firstItem.ItemCount), 1);
                Assert.AreEqual(dbContext.dbContextDbSet1.Min(p => dbContext.dbContextDbSet1.First().ItemCount), 1);
                Assert.AreEqual(dbContext.dbContextDbSet1.Min(p => GetInt()), 2);
            }
        }
        [Test]
        public void SelectNative() {
            Select(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void SelectDXProvider() {
            Select(() => new DbContextMultiClass());
        }
        private void Select(Func<DbContextMultiClass> createDbContext) {
            using(var dbContext = createDbContext()) {
                var firstItem = dbContext.dbContextDbSet1.First();
                dbContext.dbContextDbSet1.Select(p => p.ItemCount);
                dbContext.dbContextDbSet1.Select(p => dbContext.dbContextDbSet1.First().ItemCount);
                dbContext.dbContextDbSet1.Select(p => firstItem.ItemCount);
                dbContext.dbContextDbSet1.Select(p => dbContext.dbContextDbSet1.First().ItemCount);
                dbContext.dbContextDbSet1.Select(p => GetInt());
                dbContext.dbContextDbSet1.First(p => true);
                dbContext.dbContextDbSet1.First(p => p == dbContext.dbContextDbSet1.First());
                //dbContext.dbContextDbSet1.First(p => p == firstItem); TODO ??
                dbContext.dbContextDbSet1.First(p => p.ItemCount == dbContext.dbContextDbSet1.First().ItemCount);
                dbContext.dbContextDbSet1.First(p => GetInt() == 2);
            }
        }
    }
}
