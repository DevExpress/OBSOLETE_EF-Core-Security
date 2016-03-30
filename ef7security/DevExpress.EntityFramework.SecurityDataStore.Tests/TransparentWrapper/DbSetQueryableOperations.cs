using DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.TransparentWrapper {
    [TestFixture]
    public class DbSetQueryableOperations {
        [SetUp]
        public void SetUp() {
            DbContextObject1.Count = 0;
            DbContextMultiClass dbContextMultiClass = new DbContextMultiClass().MakeRealDbContext();
            dbContextMultiClass.Database.EnsureDeleted();
            dbContextMultiClass.Database.EnsureCreated();        
        }
        [Test]
        public void AllNative() {
            All(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void AllDXProvider() {
            All(() => new DbContextMultiClass());
        }
        private DbContextObject1 GetItem(DbContextMultiClass context) {
            return context.dbContextDbSet1.First();
        }
        private void All(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                Assert.IsTrue(context.dbContextDbSet1.All(p => p.ItemCount > 0));
                Assert.AreEqual(2, DbContextObject1.Count);
                Assert.IsFalse(context.dbContextDbSet1.All(p => p.ItemCount > 1));
                Assert.AreEqual(3, DbContextObject1.Count);
                DbContextObject1.Count = 0;
            }
        }
        [Test, Ignore("Don't Implemented in Remotion.Linq")]
        public void Agregate_Native() {
            Aggregate(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test, Ignore("Don't Implemented in Remotion.Linq")]
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
        [Test]
        public void AnyNative() {
            Any(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void AnyDXProvider() {
            Any(() => new DbContextMultiClass());
        }
        private void Any(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                Assert.IsTrue(context.dbContextDbSet1.Any(p => p.ItemCount > 1));
                Assert.IsTrue(context.dbContextDbSet1.Any());
                Assert.IsFalse(context.dbContextDbSet1.Any(p => p.ItemCount > 2));
                Assert.AreEqual(5, DbContextObject1.Count);
            }
        }
        [Test]
        public void AverageNative() {
            Average(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void AverageDXProvider() {
            Average(() => new DbContextMultiClass());
        }
        private void Average(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 1, DecimalItem = 3, LongItem = 5000000000, DoubleItem = 2.42, FloatItem = 3.5F, ItemCountNull = 3, DecimalItemNull = 3, LongItemNull = 7000000000, DoubleItemNull = 7.48, FloatItemNull = 3.5F });
                context.Add(new DbContextObject1() { ItemCount = 2, DecimalItem = 4, LongItem = 6000000000, DoubleItem = 5.32, FloatItem = 5.5F, ItemCountNull = null, DecimalItemNull = null, LongItemNull = null, DoubleItemNull = null, FloatItemNull = null });
                context.Add(new DbContextObject1() { ItemCount = 3, DecimalItem = 5, LongItem = 7000000000, DoubleItem = 7.48, FloatItem = 7.5F, ItemCountNull = 1, DecimalItemNull = 5, LongItemNull = 7000000000, DoubleItemNull = 7.48, FloatItemNull = 3.2F });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                {
                    DbContextObject1.Count = 0;
                    double intResult = context.dbContextDbSet1.Average(p => p.ItemCount);
                    Assert.AreEqual(2, intResult);
                    decimal decResult = context.dbContextDbSet1.Average(p => p.DecimalItem);
                    Assert.AreEqual(4, decResult);
                    var longResult = context.dbContextDbSet1.Average(p => p.LongItem);
                    Assert.AreEqual(6000000000, longResult);
                    double doubleResult = context.dbContextDbSet1.Average(p => p.DoubleItem);
                    Assert.AreEqual(5.0733333333333333, doubleResult);
                    float floatResult = context.dbContextDbSet1.Average(p => p.FloatItem);
                    Assert.AreEqual(5.5f, floatResult);
                    DbContextObject1.Count = 0;
                }
                {
                    DbContextObject1.Count = 0;
                    double? intResult = context.dbContextDbSet1.Average(p => p.ItemCountNull);
                    Assert.AreEqual(2, intResult);
                    decimal? decResult = context.dbContextDbSet1.Average(p => p.DecimalItemNull);
                    Assert.AreEqual(4, decResult);
                    var longResult = context.dbContextDbSet1.Average(p => p.LongItemNull);
                    Assert.AreEqual(7000000000, longResult);
                    double? doubleResult = context.dbContextDbSet1.Average(p => p.DoubleItemNull);
                    Assert.AreEqual(7.4800000000000004d, doubleResult);
                    float? floatResult = context.dbContextDbSet1.Average(p => p.FloatItemNull);
                    Assert.AreEqual(3.3499999f, floatResult);
                    DbContextObject1.Count = 0;
                }
            }
        }
        [Test]
        public void CastNative() {
            Cast(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void CastDXProvider() {
            Cast(() => new DbContextMultiClass());
        }
        private void Cast(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                var result = context.dbContextDbSet1.Cast<object>();
                Assert.IsTrue(result is IQueryable<object>);
                Assert.AreEqual(0, DbContextObject1.Count);
            }
        }
        [Test, Ignore("Don't Implemented in Remotion.Linq")]
        public void ConcatNative() {
            Concat(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test, Ignore("Don't Implemented in Remotion.Linq")]
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
        //[Test] //todo
        //public void ContainsNative() {
        //    Contains(() => new DbContextMultiClass().MakeRealDbContext());
        //}
        //[Test] 
        //public void ContainsDXProvider() {
        //    Contains(() => new DbContextMultiClass());
        //}
        ////incorrect working in native provider run in scope
        //private void Contains(Func<DbContextMultiClass> createDbContext) {
        //    using(var context = createDbContext()) {
        //        context.Add(new DbContextObject1() { ItemCount = 1 });
        //        context.Add(new DbContextObject1() { ItemCount = 2 });
        //        context.Add(new DbContextObject1() { ItemCount = 3 });
        //        context.SaveChanges();
        //    }
        //    using(var context = createDbContext()) {
        //        var itemfirst = context.dbContextDbSet1.First();
        //        var asfasf = context.ChangeTracker.Entries();
        //        DbContextObject1.Count = 0;
        //        var res1 = context.dbContextDbSet1.Contains(itemfirst); //return false for native provider when run in scope test
        //        Assert.AreEqual(res1, true);
        //        Assert.AreEqual(0, DbContextObject1.Count);
        //    }
        //}
        class DbContextObject1Comparer : IEqualityComparer<DbContextObject1> {
            public bool Equals(DbContextObject1 b1, DbContextObject1 b2) {
                if(b1.ItemCount == b2.ItemCount) {
                    return true;
                }
                else {
                    return false;
                }
            }
            public int GetHashCode(DbContextObject1 bx) {
                int hCode = bx.ItemCount ^ bx.GetHashCode();
                return hCode.GetHashCode();
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
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                Assert.AreEqual(context.dbContextDbSet1.Count(), 1);
                Assert.AreEqual(context.dbContextDbSet1.Count(p => p.ItemCount == 1), 1);
                Assert.AreEqual(2, DbContextObject1.Count);
                var itemFirst = context.dbContextDbSet1.Single();
                context.SaveChanges();
            }
        }
        [Test]
        public void DefaultIfEmptyNative() {
            DefaultIfEmpty(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void DefaultIfEmptyDXProvider() {
            DefaultIfEmpty(() => new DbContextMultiClass());
        }
        private void DefaultIfEmpty(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                var items = context.dbContextDbSet1.DefaultIfEmpty();
                Assert.AreEqual(0, DbContextObject1.Count);
                Assert.AreEqual(null, items.First());
                try {
                    var items1 = context.dbContextDbSet1.DefaultIfEmpty(new DbContextObject1() { ItemCount = 1 });
                    Assert.AreEqual(1, items1.First().ItemCount);
                    Assert.Fail();
                }
                catch {
                }
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                var items = context.dbContextDbSet1.DefaultIfEmpty();
                Assert.AreEqual(0, DbContextObject1.Count);
                Assert.AreEqual(3, items.First().ItemCount);
//                Assert.AreEqual(1, DbContextObject1.Count);
                try {
                    var items1 = context.dbContextDbSet1.DefaultIfEmpty(new DbContextObject1() { ItemCount = 1 });
                    Assert.AreEqual(3, items1.First().ItemCount);
                }
                catch {
                }
            }
        }
        [Test]
        public void DistinctNative() {
            Distinct(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void DistinctDXProvider() {
            Distinct(() => new DbContextMultiClass());
        }
        private void Distinct(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 5 });
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                var itemres = context.dbContextDbSet1.Select(p => p.ItemCount).Distinct().ToArray();
                Assert.AreEqual(itemres[0], 5);
                try {
                    // not released in native provider
                    var itemres1 = context.dbContextDbSet1.Distinct(new DbContextObject1Comparer()).ToArray();
                    Assert.Fail();
                }
                catch {
                }
                Assert.AreEqual(0, DbContextObject1.Count);             
            }
        }
        [Test, Ignore("Don't Implemented in Remotion.Linq")]
        public void ElementAtNative() {
            ElementAt(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test, Ignore("Don't Implemented in Remotion.Linq")]
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
        [Test, Ignore("Don't Implemented in Remotion.Linq")]
        public void ExceptNative() {
            Except(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test, Ignore("Don't Implemented in Remotion.Linq")]
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
        [Test]
        public void FirstNative() {
            First(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void FirstDXProvider() {
            First(() => new DbContextMultiClass());
        }
        private void First(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                var itemres = context.dbContextDbSet1.First().ItemCount;
                Assert.AreEqual(itemres, 1);
//                Assert.AreEqual(1, DbContextObject1.Count);
            }
        }
        [Test]
        public void ReadMultiClassNative() {
            ReadMultiClass(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void ReadMultiClassDXProvider() {
            ReadMultiClass(() => new DbContextMultiClass());
        }
        private static void ReadMultiClass(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1());
                context.Add(new DbContextObject2());
                context.Add(new DbContextObject3());
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                var item = context.dbContextDbSet1.Single();
                Assert.IsNotNull(item);
                var item1 = context.dbContextDbSet2.Single();
                Assert.IsNotNull(item1);
                var item2 = context.dbContextDbSet3.Single();
                Assert.IsNotNull(item2);
//                Assert.AreEqual(DbContextObject1.Count, 1);
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
            using(var context = createDbContext()) {
                var itemres1 = context.dbContextDbSet1.FirstOrDefault();
                Assert.AreEqual(itemres1, null);
                Assert.AreEqual(0, DbContextObject1.Count);
                context.Add(new DbContextObject1());
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                var itemres1 = context.dbContextDbSet1.FirstOrDefault();
                Assert.IsNotNull(itemres1);
//                Assert.AreEqual(1, DbContextObject1.Count);
            }
        }
        [Test]
        public void GroupByNative() {
            GroupBy(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void GroupByDXProvider() {
            GroupBy(() => new DbContextMultiClass());
        }
        private void GroupBy(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 1, ItemName = "1" });
                context.Add(new DbContextObject1() { ItemCount = 1, ItemName = "2" });
                context.Add(new DbContextObject1() { ItemCount = 3, ItemName = "3" });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                var itemres = context.dbContextDbSet1.GroupBy(p => p.ItemCount);
                var res = itemres.Where(p => p.Key == 1).First();
                Assert.AreEqual(3, DbContextObject1.Count);
                var item = res.First();
                var countEntri = context.ChangeTracker.Entries().Count();
                Assert.AreEqual(countEntri, 0);
                Assert.AreEqual(res.Count(), 2);
            }
        }
        [Test]
        public void GroupJoinNative() {
            GroupJoin(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void GroupJoinDXProvider() {
            GroupJoin(() => new DbContextMultiClass());
        }
        private void GroupJoin(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.dbContextDbSet1.Add(new DbContextObject1() { ItemName = "Silver Coin", ItemCount = 100, UseID = 1 });
                context.dbContextDbSet2.Add(new DbContextObject2() { User = "Mark", UserID = 1 });
                context.dbContextDbSet2.Add(new DbContextObject2() { User = "Jon", UserID = 1 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                var itemres1 = context.dbContextDbSet1.GroupJoin(context.dbContextDbSet2, p => p.UseID, p => p.UserID, (p, d) => d.First().ID).First();
                Assert.AreNotEqual(itemres1, null);
                var itemres2 = context.dbContextDbSet1.GroupJoin(context.dbContextDbSet2, p => p.UseID, p => p.UserID, (p, d) => new { name = d.Where(x => x.User == "Mark").Single().User }).Single();//sql ok
                Assert.AreEqual(itemres2.name, "Mark");
                var itemres = context.dbContextDbSet1.GroupJoin(context.dbContextDbSet2, p => p.UseID, p => p.UserID, (p, d) => new { name = p.ItemName }).First();
                Assert.AreEqual(itemres.name, "Silver Coin");
                var itemres3 = context.dbContextDbSet1.GroupJoin(context.dbContextDbSet2, p => p.UseID, p => p.UserID, (p, d) => new { name = p.ItemName, user = string.Join(", ", d.Select(q => q.User)) }).First();//sql ok
                Assert.AreEqual(itemres3.name, "Silver Coin");
                Assert.AreEqual(itemres3.user, "Mark, Jon");
                var itemres4 = context.dbContextDbSet1.GroupJoin(context.dbContextDbSet2, p => p.UseID, p => p.UserID, (p, d) => string.Join(", ", d.Select(q => q.User))).First();
                Assert.AreEqual(itemres4, "Mark, Jon");
            }
        }
        [Test, Ignore("don't work in native provider")]
        public void IntersectNative() {
            Intersect(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test, Ignore("don't work in native provider")]
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
        [Test]
        public void JoinNative() {
            Join(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void JoinDXProvider() {
            Join(() => new DbContextMultiClass());
        }
        private void Join(Func<DbContextMultiClass> createDbContext) {
            var item1 = new DbContextObject1() { ItemName = "Silver Coin", ItemCount = 1, UseID = 1 };
            var item2 = new DbContextObject1() { ItemName = "Gold Coin", ItemCount = 2 };
            var item3 = new DbContextObject1() { ItemName = "Iron Coin", ItemCount = 3 };
            var item4 = new DbContextObject2() { UserID = 1, User = "Mark" };
            using(var context = createDbContext()) {
                context.dbContextDbSet1.Add(item1);
                context.dbContextDbSet1.Add(item2);
                context.dbContextDbSet1.Add(item3);
                context.dbContextDbSet2.Add(item4);
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                var itemres = context.dbContextDbSet1.Join(context.dbContextDbSet2, p => p.UseID, p => p.UserID, (p, d) => p.ItemCount).First();
                //Assert.AreEqual(3, DbContextObject1.Count);
            }
        }
        [Test]
        public void LastOrDefaultNative() {
            LastOrDefault(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void LastOrDefaultDXProvider() {
            LastOrDefault(() => new DbContextMultiClass());
        }
        private void LastOrDefault(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                var itemres = context.dbContextDbSet1.LastOrDefault();
                Assert.AreEqual(DbContextObject1.Count, 0);
            }
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.SaveChanges();
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                var itemres = context.dbContextDbSet1.LastOrDefault();
                Assert.AreEqual(2, itemres.ItemCount);
//                Assert.AreEqual(2, DbContextObject1.Count);
            }
        }
        [Test]
        public void LastNative() {
            Last(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void LastDXProvider() {
            Last(() => new DbContextMultiClass());
        }
        private void Last(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.SaveChanges();
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {

                DbContextObject1.Count = 0;
                var itemres = context.dbContextDbSet1.Last();
                Assert.AreEqual(itemres.ItemCount, 2);
//                Assert.AreEqual(2, DbContextObject1.Count);
                DbContextObject1.Count = 0;
                var itemres1 = context.dbContextDbSet1.Last(p => p.ItemCount == 2);
                Assert.AreEqual(itemres1.ItemCount, 2);
//                Assert.AreEqual(1, DbContextObject1.Count);
            }
        }
        [Test]
        public void LoadNative() {
            Load(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void LoadDXProvider() {
            Load(() => new DbContextMultiClass());
        }
        private void Load(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                Assert.AreEqual(0, context.ChangeTracker.Entries().Count());
                context.dbContextDbSet1.Load();
//                Assert.AreEqual(3, DbContextObject1.Count);
            }
        }
        [Test]
        public void LongCountNative() {
            LongCount(() => new DbContextMultiClass().MakeRealDbContext(), 3);
        }
        [Test]
        public void LongCountDXProvider() {
            LongCount(() => new DbContextMultiClass(), 3);
        }
        private void LongCount(Func<DbContextMultiClass> createDbContext, int countRead) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1());
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                var res = context.dbContextDbSet1.LongCount();
                Assert.AreEqual(1, res);
            //    Assert.AreEqual(2, DbContextObject1.Count);
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
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 1, DecimalItem = 3, LongItem = 5000000000, DoubleItem = 2.42, FloatItem = 3.5F, ItemCountNull = 3, DecimalItemNull = 3, LongItemNull = 7000000000, DoubleItemNull = 7.48, FloatItemNull = 3.5F });
                context.Add(new DbContextObject1() { ItemCount = 2, DecimalItem = 4, LongItem = 6000000000, DoubleItem = 5.32, FloatItem = 5.5F, ItemCountNull = null, DecimalItemNull = null, LongItemNull = null, DoubleItemNull = null, FloatItemNull = null });
                context.Add(new DbContextObject1() { ItemCount = 3, DecimalItem = 5, LongItem = 7000000000, DoubleItem = 7.48, FloatItem = 7.5F, ItemCountNull = 1, DecimalItemNull = 5, LongItemNull = 7000000000, DoubleItemNull = 7.48, FloatItemNull = 3.2F });

                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                {
                    DbContextObject1.Count = 0;
                    double intResult = context.dbContextDbSet1.Max(p => p.ItemCount);
                    Assert.AreEqual(3.0, intResult);
                    decimal decResult = context.dbContextDbSet1.Max(p => p.DecimalItem);
                    Assert.AreEqual(5m, decResult);
                    var longResult = context.dbContextDbSet1.Max(p => p.LongItem);
                    Assert.AreEqual(7000000000, longResult);
                    double doubleResult = context.dbContextDbSet1.Max(p => p.DoubleItem);
                    Assert.AreEqual(7.48, doubleResult);
                    float floatResult = context.dbContextDbSet1.Max(p => p.FloatItem);
                    Assert.AreEqual(7.5F, floatResult);
                    DbContextObject1.Count = 0;
                }
                {
                    DbContextObject1.Count = 0;
                    double? intResult = context.dbContextDbSet1.Max(p => p.ItemCountNull);
                    Assert.AreEqual(3, intResult);
                    decimal? decResult = context.dbContextDbSet1.Max(p => p.DecimalItemNull);
                    Assert.AreEqual(5m, decResult);
                    var longResult = context.dbContextDbSet1.Max(p => p.LongItemNull);
                    Assert.AreEqual(7000000000, longResult);
                    double? doubleResult = context.dbContextDbSet1.Max(p => p.DoubleItemNull);
                    Assert.AreEqual(7.48, doubleResult);
                    float? floatResult = context.dbContextDbSet1.Max(p => p.FloatItemNull);
                    Assert.AreEqual(3.5F, floatResult);
                    DbContextObject1.Count = 0;
                }
            }
        }
        [Test]
        public void OfTypeNative() {
            OfType(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void OfTypeDXProvider() {
            OfType(() => new DbContextMultiClass());
        }
        private void OfType(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                var res = context.dbContextDbSet1.OfType<object>();
                var countres = res.Count();
            }
        }
        [Test]
        public void SumNative() {
            Sum(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void SumDXProvider() {
            Sum(() => new DbContextMultiClass());
        }
        private void Sum(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 1, DecimalItem = 3, LongItem = 5000000000, DoubleItem = 2.42, FloatItem = 1.5F, ItemCountNull = 3, DecimalItemNull = 3, LongItemNull = 5000000000, DoubleItemNull = 5.48, FloatItemNull = 3.5F });
                context.Add(new DbContextObject1() { ItemCount = 2, DecimalItem = 4, LongItem = 6000000000, DoubleItem = 5.32, FloatItem = 1.5F, ItemCountNull = null, DecimalItemNull = null, LongItemNull = null, DoubleItemNull = null, FloatItemNull = null });
                context.Add(new DbContextObject1() { ItemCount = 3, DecimalItem = 5, LongItem = 7000000000, DoubleItem = 7.48, FloatItem = 1.5F, ItemCountNull = 1, DecimalItemNull = 5, LongItemNull = 7000000000, DoubleItemNull = 7.48, FloatItemNull = 3.2F });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                {
                    DbContextObject1.Count = 0;
                    double intResult = context.dbContextDbSet1.Sum(p => p.ItemCount);
                    Assert.AreEqual(6.0, intResult);
                    decimal decResult = context.dbContextDbSet1.Sum(p => p.DecimalItem);
                    Assert.AreEqual(12m, decResult);
                    var longResult = context.dbContextDbSet1.Sum(p => p.LongItem);
                    Assert.AreEqual(18000000000, longResult);
                    double doubleResult = context.dbContextDbSet1.Sum(p => p.DoubleItem);
                    Assert.AreEqual(15.22, doubleResult);
                    try { //not work in notive
                        float floatResult = context.dbContextDbSet1.Sum(p => p.FloatItem);
                        Assert.AreEqual(4.5F, floatResult);
                        Assert.Fail();
                    }
                    catch {
                    }
                    DbContextObject1.Count = 0;
                }
                {
                    DbContextObject1.Count = 0;
                    double? intResult = context.dbContextDbSet1.Sum(p => p.ItemCountNull);
                    Assert.AreEqual(4, intResult);
                    decimal? decResult = context.dbContextDbSet1.Sum(p => p.DecimalItemNull);
                    Assert.AreEqual(8m, decResult);
                    var longResult = context.dbContextDbSet1.Sum(p => p.LongItemNull);
                    Assert.AreEqual(12000000000, longResult);
                    double? doubleResult = context.dbContextDbSet1.Sum(p => p.DoubleItemNull);
                    Assert.AreEqual(12.96, doubleResult);
                    try {
                        float? floatResult = context.dbContextDbSet1.Sum(p => p.FloatItemNull);
                        Assert.AreEqual(3.2F, floatResult);
                        Assert.Fail();
                    }
                    catch {
                    }
                    DbContextObject1.Count = 0;
                }
                {
                    DbContextObject1.Count = 0;
                    var intResult = context.dbContextDbSet1.Sum(p => 7);
                    Assert.AreEqual(intResult, 21);
                }
            }
        }
        [Test]
        public void OrderByNative() {
            OrderBy(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void OrderByDXProvider() {
            OrderBy(() => new DbContextMultiClass());
        }
        private void OrderBy(Func<DbContextMultiClass> createDbContext, int countAccessToObject = 1) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                var res = context.dbContextDbSet1.OrderBy(p => p.ItemCount).ToArray();
//                Assert.AreEqual(3 * countAccessToObject, DbContextObject1.Count);
                Assert.AreEqual(res.First().ItemCount, 1);
            }
        }
        [Test]
        public void OrderByDescendingNative() {
            OrderByDescending(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void OrderByDescendingDXProvider() {
            OrderByDescending(() => new DbContextMultiClass());
        }
        private void OrderByDescending(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                var res1 = context.dbContextDbSet1.OrderByDescending(p => p.ItemCount);
                Assert.AreEqual(res1.First().ItemCount, 3);
//                Assert.AreEqual(3, DbContextObject1.Count);
            }
        }
        [Test]
        public void RemoveNative() {
            Remove(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void RemoveDXProvider() {
            Remove(() => new DbContextMultiClass());
        }
        private void Remove(Func<DbContextMultiClass> createDbContext) {
            var item1 = new DbContextObject1() { ItemCount = 2 };
            using(var context = createDbContext()) {
                context.Add(item1);
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;                
                context.dbContextDbSet1.Remove(item1);
                context.SaveChanges();
                //Assert.AreEqual(0, DbContextObject1.Count);
            }
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                Assert.AreEqual(context.dbContextDbSet1.Count(), 2);
                Assert.AreEqual(2, DbContextObject1.Count);
                var item = context.dbContextDbSet1.First();
                context.Remove(item);
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                Assert.AreEqual(context.dbContextDbSet1.Count(), 1);
                context.SaveChanges();
                Assert.AreEqual(1, DbContextObject1.Count);
            }
        }
        [Test]
        public void RemoveRangeNative() {
            RemoveRange(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void RemoveRangeDXProvider() {
            RemoveRange(() => new DbContextMultiClass());
        }
        private void RemoveRange(Func<DbContextMultiClass> createDbContext) {
            var item1 = new DbContextObject1() { ItemCount = 1 };
            var item2 = new DbContextObject1() { ItemCount = 2 };
            var item3 = new DbContextObject1() { ItemCount = 3 };
            using(var context = createDbContext()) {
                context.dbContextDbSet1.AddRange(new[] { item1, item2, item3 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                context.dbContextDbSet1.RemoveRange(new[] { item1, item2 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                Assert.AreEqual(context.dbContextDbSet1.Count(), 1);
                Assert.AreEqual(1, DbContextObject1.Count);
                var itemfirst = context.dbContextDbSet1.First();
                context.RemoveRange(new[] { itemfirst });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                Assert.AreEqual(context.dbContextDbSet1.Count(), 0);
                Assert.AreEqual(0, DbContextObject1.Count);
            }
        }
        [Test, Ignore("don't work in native provider")]
        public void ReverseNative() {
            Reverse(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test, Ignore("don't work in native provider")]
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
        [Test]
        public void SelectNative() {
            Select(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void SelectDXProvider() {
            Select(() => new DbContextMultiClass());
        }
        private void Select(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                var resint1 = context.dbContextDbSet1.Select(p => p.ItemCount);
                var dsad = resint1.First();
                var resint2 = context.dbContextDbSet1.Where(p => p.ItemCount > 1).Select(p => p.ItemCount);
                Assert.AreEqual(0, DbContextObject1.Count);
                var itemMy = context.dbContextDbSet1.Select(p => new { itemCount = p.ItemCount });
                foreach(var tttt in itemMy) ;
            }
        }
        [Test]
        public void SelectManyNative() {
            SelectMany(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void SelectManyDXProvider() {
            SelectMany(() => new DbContextMultiClass());
        }
        private void SelectMany(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 2, ItemName = "2" });
                context.Add(new DbContextObject1() { ItemCount = 3, ItemName = "3" });
                context.Add(new DbContextObject1() { ItemCount = 1, ItemName = "1" });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                var resint = context.dbContextDbSet1.SelectMany(p => p.ItemName);

            }
        }
        [Test, Ignore("re-linq not supported")]
        public void SequenceEqualNative() {
            SequenceEqual(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test, Ignore("re-linq not supported")]
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
        [Test]
        public void SingleNative() {
            Single(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void SingleDXProvider() {
            Single(() => new DbContextMultiClass());
        }
        private void Single(Func<DbContextMultiClass> createDbContext, int flagDxProvider = 1) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                var resint = context.dbContextDbSet1.Single();
                //Assert.AreEqual(1 * flagDxProvider, DbContextObject1.Count);
                Assert.AreNotEqual(resint, null);
            }
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                try {
                    var resint = context.dbContextDbSet1.Single();
                    Assert.Fail();
                }
                catch {
                }
            }
            using(var context = createDbContext()) {
                try {
                    var resint = context.dbContextDbSet1.Single();
                    Assert.Fail();
                }
                catch {
                }
            }

            using(var context = createDbContext()) {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                context.Add(new DbContextObject1() { ItemCount = 5 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                var Entries = context.ChangeTracker.Entries();
                var resint = context.dbContextDbSet1.Single(p => p.ItemCount == 5);
                Assert.AreEqual(resint.ItemCount, 5);
            }
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                var resint = context.dbContextDbSet1.Single(p => p.ItemCount == 3);
               // Assert.AreEqual( 4, DbContextObject1.Count);
                Assert.AreEqual(resint.ItemCount, 3);
            }
            using(var context = createDbContext()) {
                try {
                    var resint = context.dbContextDbSet1.Single(p => p.ItemCount > 1);
                    Assert.Fail();
                }
                catch {
                }
            }
        }
        [Test]
        public void SingleOrDefaultNative() {
            SingleOrDefault(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void SingleOrDefaultDXProvider() {
            SingleOrDefault(() => new DbContextMultiClass());
        }
        private void SingleOrDefault(Func<DbContextMultiClass> createDbContext, int flagDxProvider = 1) {
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                var resint = context.dbContextDbSet1.SingleOrDefault();
                Assert.AreEqual(0, DbContextObject1.Count);
                Assert.AreEqual(resint, null);
            }
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                try {
                    var resint = context.dbContextDbSet1.SingleOrDefault();
                    Assert.Fail();
                }
                catch {
                }
            }
            using(var context = createDbContext()) {
                try {
                    var resint = context.dbContextDbSet1.SingleOrDefault();
                    Assert.Fail();
                }
                catch {
                }
            }
            using(var context = createDbContext()) {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                var resint = context.dbContextDbSet1.SingleOrDefault(p => p.ItemCount == 5);
                Assert.AreEqual(0, DbContextObject1.Count);
                Assert.AreEqual(resint, null);
            }
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                var resint = context.dbContextDbSet1.SingleOrDefault(p => p.ItemCount == 3);
              //  Assert.AreEqual(3, DbContextObject1.Count);
                Assert.AreEqual(resint.ItemCount, 3);
            }
            using(var context = createDbContext()) {
                try {
                    var resint = context.dbContextDbSet1.SingleOrDefault(p => p.ItemCount > 1);
                    Assert.Fail();
                }
                catch {
                }
            }
        }
        [Test, Ignore("re-linq not supported")]
        public void SkipWhileNative() {
            SkipWhile(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test, Ignore("re-linq not supported")]
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
        [Test]
        public void SkipNative() {
            Skip(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void SkipDXProvider() {
            Skip(() => new DbContextMultiClass());
        }
        private void Skip(Func<DbContextMultiClass> createDbContext, int multDxProvider = 1) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                var resint = context.dbContextDbSet1.OrderBy(p => p.ItemCount).Skip(1);
                Assert.AreEqual(resint.ToArray().Length, 2);               
            }
        }

        [Test]
        public void TakeNative() {
            Take(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void TakeDXProvider() {
            Take(() => new DbContextMultiClass());
        }
        private void Take(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                var resint1 = context.dbContextDbSet1.Take(10);
                Assert.AreEqual(resint1.Count(), 3);
                DbContextObject1.Count = 0;
                var resint = context.dbContextDbSet1.Take(2);
                Assert.AreEqual(resint.Count(), 2);
                Assert.AreEqual(2, DbContextObject1.Count);
            }
        }
        [Test, Ignore("re-linq not supported")]
        public void TakeWhileNative() {
            TakeWhile(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test, Ignore("re-linq not supported")]
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
        [Test]
        public void ThenByNative() {
            ThenBy(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void ThenByDXProvider() {
            ThenBy(() => new DbContextMultiClass());
        }
        private void ThenBy(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemName = "Siqweqweqwe", ItemCount = 1 });
                context.Add(new DbContextObject1() { ItemName = "Gold", ItemCount = 2 });
                context.Add(new DbContextObject1() { ItemName = "Iro", ItemCount = 2 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {

                var resint = context.dbContextDbSet1.OrderBy(p => p.ItemName.Length).ThenBy(p => p.ItemCount).ToArray();
                //Not supported yet, incorrect OrderBy in native provider
                try {
                    Assert.AreEqual(resint.Last().ItemCount, 3);
                    Assert.AreEqual(resint.ToArray().Count(), 3);
                    Assert.AreEqual(6, DbContextObject1.Count);
                }
                catch {
                }

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
        [Test]
        public void ToArrayNative() {
            ToArray(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void ToArrayDXProvider() {
            ToArray(() => new DbContextMultiClass());
        }
        private void ToArray(Func<DbContextMultiClass> createDbContext, int countAccesstoObject = 1) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                var resint = context.dbContextDbSet1.ToArray();
                Assert.AreEqual(resint.GetType().BaseType, typeof(Array));
               // Assert.AreEqual(3 * countAccesstoObject, DbContextObject1.Count);
            }
        }
        [Test]
        public void ToDictionaryNative() {
            ToDictionary(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void ToDictionaryDXProvider() {
            ToDictionary(() => new DbContextMultiClass());
        }
        private void ToDictionary(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                var resint = context.dbContextDbSet1.ToDictionary(p => p.ID);
                Assert.AreEqual(resint.GetType(), typeof(Dictionary<int, DbContextObject1>));
              //  Assert.AreEqual(6, DbContextObject1.Count);
            }
        }
        [Test]
        public void ToListNative() {
            ToList(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void ToListDXProvider() {
            ToList(() => new DbContextMultiClass());
        }
        private void ToList(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                var resint = context.dbContextDbSet1.ToList();
                Assert.AreEqual(resint.GetType(), typeof(List<DbContextObject1>));
            //    Assert.AreEqual(6, DbContextObject1.Count);
            }
        }
        [Test]
        public void ToLookupNative() {
            ToLookup(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void ToLookupDXProvider() {
            ToLookup(() => new DbContextMultiClass());
        }
        private void ToLookup(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                var resint = context.dbContextDbSet1.ToLookup(p => p.ID);
                Assert.AreEqual(resint.GetType(), typeof(Lookup<int, DbContextObject1>));
                //Assert.AreEqual(6, DbContextObject1.Count);
            }
        }
        [Test, Ignore("don't work in native provider")]
        public void UnionNative() {
            Union(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test, Ignore("don't work in native provider")]
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
        [Test]
        public void UpdateNative() {
            Update(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void UpdateDXProvider() {
            Update(() => new DbContextMultiClass());
        }
        private void Update(Func<DbContextMultiClass> createDbContext) {
            int id = 0;
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.SaveChanges();
                id = context.dbContextDbSet1.First().ID;
            }
            using(var context = createDbContext()) {
                var newitem = new DbContextObject1() { ID = id };
                DbContextObject1.Count = 0;
                var resitem = context.dbContextDbSet1.Update(newitem);
                Assert.AreEqual(0, DbContextObject1.Count);
                resitem.Entity.ItemCount = 1000;
                var count = context.ChangeTracker.Entries().Count();
                Assert.AreEqual(1, count);
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                Assert.AreEqual(context.dbContextDbSet1.Where(p => p.ItemCount == 1000).Count(), 1);
                var firstitem = context.dbContextDbSet1.First();
                firstitem.ItemCount = 5000;
                var count = context.ChangeTracker.Entries().Count();
                Assert.AreEqual(1, count);
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                Assert.AreEqual(context.dbContextDbSet1.Where(p => p.ItemCount == 5000).Count(), 1);
            }
        }
        [Test]
        public void UpdateRangeNative() {
            UpdateRange(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void UpdateRangeDXProvider() {
            UpdateRange(() => new DbContextMultiClass());
        }
        private void UpdateRange(Func<DbContextMultiClass> createDbContext) {
            int id = 0;
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.SaveChanges();
                id = context.dbContextDbSet1.First().ID;
            }
            using(var context = createDbContext()) {
                var newitem = new DbContextObject1() { ID = id };
                DbContextObject1.Count = 0;
                context.dbContextDbSet1.UpdateRange(new[] { newitem });
                newitem.ItemCount = 1000;
                context.SaveChanges();
              //  Assert.AreEqual(0, DbContextObject1.Count);
            }
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                Assert.AreEqual(context.dbContextDbSet1.Where(p => p.ItemCount == 1000).Count(), 1);
               // Assert.AreEqual(3, DbContextObject1.Count);
            }
        }
        [Test]
        public void WhereNative() {
            Where(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void WhereDXProvider() {
            Where(() => new DbContextMultiClass());
        }
        private void Where(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                var res = context.dbContextDbSet1.Where(p => p.ItemCount == 2).Single();
                Assert.AreEqual(res.ItemCount, 2);
             //   Assert.AreEqual(6, DbContextObject1.Count);
            }
            //this need to test primary expression
            using(var context = createDbContext()) {
                var queryableWhere1 = (typeof(Queryable).GetMethods().Where(p => p.Name == "Where").First().MakeGenericMethod(typeof(DbContextObject1)));
                BinaryExpression binaryExpression = Expression.MakeBinary(ExpressionType.And, Expression.Constant(false), Expression.Constant(true));
                ParameterExpression parametrToExpression = Expression.Parameter(typeof(DbContextObject1), "p");
                var exp = Expression.Lambda<Func<DbContextObject1, bool>>(binaryExpression, new[] { parametrToExpression });
                var queryableWhere = Expression.Call(queryableWhere1, new Expression[] { Expression.Constant(context.dbContextDbSet1), exp });
                var queryableCount = (typeof(Queryable).GetMethods().Where(p => p.Name == "Count").First().MakeGenericMethod(typeof(DbContextObject1)));
                var methCount = Expression.Call(queryableCount, queryableWhere);
                var result = Expression.Lambda(methCount).Compile().DynamicInvoke();
                Assert.AreEqual(0, result);
            }
        }

        [Test]
        public void WhereWhereComplexSQLNative() {
            WhereWhereComplexSQL(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void WhereWhereComplexSQLDXProvider() {
            WhereWhereComplexSQL(() => new DbContextMultiClass());
        }
        private void WhereWhereComplexSQL(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {

                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.Add(new DbContextObject1() { ItemCount = 3, Description = "qwe" });
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                Expression<Func<DbContextObject1, bool>> predicate = p => p.ItemCount > 0;
                var res = context.dbContextDbSet1.Where(p => p.ItemCount > 0).Where(p => p.ItemCount > 2).Single();
                Assert.AreEqual(res.ItemCount, 3);
               // Assert.AreEqual(3, DbContextObject1.Count);
            }
        }
        [Test, Ignore("re-linq not supported")]
        public void ZipNative() {
            Zip(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test, Ignore("re-linq not supported")]
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
        [Test]
        public void ForEachNative() {
            ForEach(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void ForEachDXProvider() {
            ForEach(() => new DbContextMultiClass(), 3);
        }
        private void ForEach(Func<DbContextMultiClass> createDbContext, int countDxProviderMult = 1) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                int count = 0;
                DbContextObject1.Count = 0;
                foreach(var item in context.dbContextDbSet1) {
                    break;
                }
                count = 0;
//                Assert.AreEqual(DbContextObject1.Count, 1 * countDxProviderMult);
                DbContextObject1.Count = 0;
              

                DbContextObject1.Count = 0;
            }
        }
    }
}
