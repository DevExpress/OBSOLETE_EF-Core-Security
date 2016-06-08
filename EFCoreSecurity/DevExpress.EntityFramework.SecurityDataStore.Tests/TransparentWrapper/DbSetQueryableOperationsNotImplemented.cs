using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.EntityFramework.SecurityDataStore.Tests.Helpers;
using DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.TransparentWrapper {
    [TestFixture]
    public abstract class DbSetQueryableOperationsNotImplementedTests {
        [SetUp]
        public void ClearDatabase() {
            DbContextObject1.Count = 0;
            DbContextMultiClass dbContextMultiClass = new DbContextMultiClass().MakeRealDbContext();
            dbContextMultiClass.ResetDatabase();            
        }
        [Test, Ignore("Not Implemented in Remotion.Linq")]
        public void Agregate_Native() {
            Aggregate(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test, Ignore("Not Implemented in Remotion.Linq")]
        public void Agregate_DXProvider() {
            Aggregate(() => new DbContextMultiClass());
        }
        private void Aggregate(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                Assert.AreEqual(0, context.dbContextDbSet1.Aggregate(0, (i, p) => i + p.ItemCount));
                Assert.AreEqual(0, DbContextObject1.Count);
            }
        }

        [Test, Ignore("Not Implemented in Remotion.Linq")]
        public void ConcatNative() {
            Concat(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test, Ignore("Not Implemented in Remotion.Linq")]
        public void ConcatDXProvider() {
            Concat(() => new DbContextMultiClass());
        }
        private void Concat(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                var res2 = context.dbContextDbSet1.Concat((new[] { new DbContextObject1() { ItemCount = 500 } }));
                Assert.AreEqual(res2.Count(), 4);//fail
            }
        }

        [Test, Ignore("Not implemented in Remotion.Linq")]
        public void ElementAtNative() {
            ElementAt(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test, Ignore("Not implemented in Remotion.Linq")]
        public void ElementAtDXProvider() {
            ElementAt(() => new DbContextMultiClass());
        }
        private void ElementAt(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                var itemres = context.dbContextDbSet1.ElementAt(1);
                Assert.AreNotEqual(itemres, null);
            }
        }

        [Test, Ignore("Not implemented in Remotion.Linq")]
        public void ExceptNative() {
            Except(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test, Ignore("Not implemented in Remotion.Linq")]
        public void ExceptDXProvider() {
            Except(() => new DbContextMultiClass());
        }
        private void Except(Func<DbContextMultiClass> createDbContext) {
            var item1 = new DbContextObject1() { ItemCount = 1 };
            var item2 = new DbContextObject1() { ItemCount = 2 };
            var item3 = new DbContextObject1() { ItemCount = 3 };
            var array = new[] { item1, item3 };
            using(var context = createDbContext()) {
                context.dbContextDbSet1.Add(item1);
                context.dbContextDbSet1.Add(item2);
                context.dbContextDbSet1.Add(item3);
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                var itemres = context.dbContextDbSet1.Except(array);
                var rescount = itemres.Count();
            }
        }

        [Test, Ignore("Doesn't work in the native provider")]
        public void IntersectNative() {
            Intersect(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test, Ignore("Doesn't work in the native provider")]
        public void IntersectDXProvider() {
            Intersect(() => new DbContextMultiClass());
        }
        private void Intersect(Func<DbContextMultiClass> createDbContext) {
            var item1 = new DbContextObject1() { ItemCount = 1 };
            var item2 = new DbContextObject1() { ItemCount = 2 };
            var item3 = new DbContextObject1() { ItemCount = 3 };
            var array = new[] { item1, item3 };
            var array1 = new[] { item1, item2, item3 };
            using(var context = createDbContext()) {
                context.dbContextDbSet1.Add(item1);
                context.dbContextDbSet1.Add(item2);
                context.dbContextDbSet1.Add(item3);
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                var itemres1 = array1.Intersect(array);
                var countitem1 = itemres1.Count();
                var itemres = context.dbContextDbSet1.Intersect(array);
                var countitem = itemres.Count();
            }
        }

        [Test, Ignore("Doesn't work in the native provider")]
        public void ReverseNative() {
            Reverse(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test, Ignore("Doesn't work in the native provider")]
        public void ReverseDXProvider() {
            Reverse(() => new DbContextMultiClass());
        }
        private void Reverse(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.SaveChanges();
            }
            DbContextObject1 res1 = null;
            using(var context = createDbContext()) {
                res1 = context.dbContextDbSet1.First();
                context.dbContextDbSet1.Reverse();
                context.SaveChanges();
            }
            DbContextObject1 res2 = null;
            using(var context = createDbContext()) {
                res2 = context.dbContextDbSet1.First();
                Assert.AreNotEqual(res2.ItemCount, res1.ItemCount);
            }
        }

        [Test, Ignore("Not implemented in Remotion.Linq")]
        public void SequenceEqualNative() {
            SequenceEqual(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test, Ignore("Not implemented in Remotion.Linq")]
        public void SequenceEqualDXProvider() {
            SequenceEqual(() => new DbContextMultiClass());
        }
        private void SequenceEqual(Func<DbContextMultiClass> createDbContext) {
            var item1 = new DbContextObject1() { ItemName = "Silver Coin", ItemCount = 200 };
            var item2 = new DbContextObject1() { ItemName = "Gold Coin", ItemCount = 100 };
            var item3 = new DbContextObject1() { ItemName = "Iron Coin", ItemCount = 300 };
            var array = new[] { item1, item2, item3 };
            using(var context = createDbContext()) {
                context.dbContextDbSet1.AddRange(new[] { item1, item2, item3 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                var resint = context.dbContextDbSet1.SequenceEqual(array);
            }
        }

        [Test, Ignore("Not implemented in Remotion.Linq")]
        public void SkipWhileNative() {
            SkipWhile(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test, Ignore("Not implemented in Remotion.Linq")]
        public void SkipWhileDXProvider() {
            SkipWhile(() => new DbContextMultiClass());
        }
        private void SkipWhile(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                var resint = context.dbContextDbSet1.SkipWhile(p => p.ItemCount == 2);
                Assert.AreEqual(resint.Count(), 2);
            }
        }

        [Test, Ignore("Not implemented in Remotion.Linq")]
        public void TakeWhileNative() {
            TakeWhile(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test, Ignore("Not implemented in Remotion.Linq")]
        public void TakeWhileDXProvider() {
            TakeWhile(() => new DbContextMultiClass());
        }
        private void TakeWhile(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                var resint = context.dbContextDbSet1.TakeWhile(p => p.ItemCount == 200);
                Assert.AreEqual(resint.Count(), 2);
            }
        }

        [Test, Ignore("TODO")]
        public void ThenByDescendingNative() {
            ThenByDescending(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test, Ignore("TODO")]
        public void ThenByDescendingDXProvider() {
            ThenByDescending(() => new DbContextMultiClass());
        }
        private void ThenByDescending(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemName = "Silver", ItemCount = 1 });
                context.Add(new DbContextObject1() { ItemName = "Gold", ItemCount = 2 });
                context.Add(new DbContextObject1() { ItemName = "Iron Coin", ItemCount = 3 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                //in native provider incorect converct {value(Microsoft.Data.Entity.Query.EntityQueryable`1[DevExpress.EntityFramework.DbContextDataStore.Tests.DbContextObject1]).OrderBy(p => p.ItemName.Length)}
                var resint = context.dbContextDbSet1.OrderBy(p => p.ItemName.Length).ThenByDescending(p => p.ItemCount).Last().ItemCount;
                Assert.AreEqual(1, resint);
                Assert.AreEqual(6, DbContextObject1.Count);
            }
        }

        [Test, Ignore("Doesn't work in the native provider")]
        public void UnionNative() {
            Union(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test, Ignore("Doesn't work in the native provider")]
        public void UnionDXProvider() {
            Union(() => new DbContextMultiClass());
        }
        private void Union(Func<DbContextMultiClass> createDbContext) {
            var item1 = new DbContextObject1() { ItemName = "Silver Coin", ItemCount = 200 };
            var item2 = new DbContextObject1() { ItemName = "Gold Coin", ItemCount = 100 };
            var item3 = new DbContextObject1() { ItemName = "Iron Coin", ItemCount = 300 };
            var item5 = new DbContextObject1() { ItemName = "Bad Coin", ItemCount = 100 };
            var item4 = new DbContextObject1() { ItemName = "Good Coin", ItemCount = 500 };
            var array = new[] { item4, item5 };
            var array1 = new[] { item1, item2, item3 };
            using(var context = createDbContext()) {
                context.dbContextDbSet1.AddRange(new[] { item1, item2, item3 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                var res1 = array1.Union(array);
                Assert.AreEqual(5, res1.Count());
                var resint = context.dbContextDbSet1.Union(array);
                Assert.AreEqual(5, resint.Count());
            }
        }
        [Test, Ignore("Not implemented in Remotion.Linq")]
        public void ZipNative() {
            Zip(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test, Ignore("Not implemented in Remotion.Linq")]
        public void ZipDXProvider() {
            Zip(() => new DbContextMultiClass());
        }
        private void Zip(Func<DbContextMultiClass> createDbContext) {
            var item1 = new DbContextObject1() { ItemName = "Silver Coin", ItemCount = 200 };
            var item2 = new DbContextObject1() { ItemName = "Gold Coin", ItemCount = 100 };
            var item3 = new DbContextObject1() { ItemName = "Iron Coin", ItemCount = 300 };
            var arrayint = new[] { 1, 2, 3 };
            using(var context = createDbContext()) {
                context.dbContextDbSet1.AddRange(new[] { item1, item2, item3 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                var res = context.dbContextDbSet1.Zip(arrayint, (p, b) => p + " " + b); ;
                Assert.AreEqual(res.Count(), 3);
            }
        }
    }
    [TestFixture]
    public class InMemoryDbSetQueryableOperationsNotImplementedTests : DbSetQueryableOperationsNotImplementedTests {
        [SetUp]
        public void Setup() {
            SecurityTestHelper.CurrentDatabaseProviderType = SecurityTestHelper.DatabaseProviderType.IN_MEMORY;
            base.ClearDatabase();
        }
    }

    [TestFixture]
    public class LocalDb2012DbSetQueryableOperationsNotImplementedTests : DbSetQueryableOperationsNotImplementedTests {
        [SetUp]
        public void Setup() {
            SecurityTestHelper.CurrentDatabaseProviderType = SecurityTestHelper.DatabaseProviderType.LOCALDB_2012;
            base.ClearDatabase();
        }
    }
}
