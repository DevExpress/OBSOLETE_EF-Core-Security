using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DevExpress.EntityFramework.SecurityDataStore.Tests.Helpers;
using DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.TransparentWrapper {
    [TestFixture]
    public abstract class DbSetQueryableExtensionsOperationsTests  {
        [SetUp]
        public void ClearDatabase() {
            DbContextObject1.Count = 0;
            DbContextMultiClass dbContextMultiClass = new DbContextMultiClass().MakeRealDbContext();
            dbContextMultiClass.ResetDatabase();
        }        
        [Test]
        public void AsNoTrackingNative() {
            AsNoTracking(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void AsNoTrackingDXProvider() {
            AsNoTracking(() => new DbContextMultiClass());
        }
        private void AsNoTracking(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                var r = context.dbContextDbSet1.AsNoTracking();
                foreach(var i in r)
                    ;
                int countEntri = context.ChangeTracker.Entries().Count();
                Assert.AreEqual(countEntri, 0);
                Assert.AreEqual(r.Count(), 1);
            }
        }
        [Test]
        public void ForEachAsyncNative() {
            ForEachAsync(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void ForEachAsyncDXProvider() {
            ForEachAsync(() => new DbContextMultiClass());
        }
        private void ForEachAsync(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                for(int i = 0; i < 10; i++) {
                    context.Add(new DbContextObject1() { ItemName = i.ToString() });
                }
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                List<string> log = new List<string>();
                Task taskForEachAsync = context.dbContextDbSet1.ForEachAsync(p => {
                    Assert.IsNotNull(p);
                    log.Add(p.ItemName);
                });
                taskForEachAsync.Wait();
                log.Sort();
                Assert.AreEqual("0;1;2;3;4;5;6;7;8;9", string.Join(";", log.ToArray()));
            }
        }
        [Test]
        public void AllAsyncTest() {
            int savedCount1, savedCount2;

            Func<DbContextMultiClass> createRealDbContext = () => new DbContextMultiClass().MakeRealDbContext();
            using(var context = createRealDbContext()) {
                context.ResetDatabase();
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.SaveChanges();
            }
            using(var context = createRealDbContext()) {
                DbContextObject1.Count = 0;
                Task<bool> taskForEachAsync = context.dbContextDbSet1.AllAsync(p => p.ItemCount > 0);
                taskForEachAsync.Wait();
                bool res = taskForEachAsync.Result;
                Assert.IsTrue(taskForEachAsync.Result);
                savedCount1 = DbContextObject1.Count;

                DbContextObject1.Count = 0;
                Task<bool> taskForEachAsync2 = context.dbContextDbSet1.AllAsync(p => p.ItemCount > 2);
                taskForEachAsync2.Wait();
                Assert.IsFalse(taskForEachAsync2.Result);
                savedCount2 = DbContextObject1.Count;
            }

            Func<DbContextMultiClass> createDbContext = () => new DbContextMultiClass();
            using(var context = createDbContext()) {
                context.ResetDatabase();
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                Task<bool> taskForEachAsync = context.dbContextDbSet1.AllAsync(p => p.ItemCount > 0);
                taskForEachAsync.Wait();
                bool res = taskForEachAsync.Result;
                Assert.IsTrue(taskForEachAsync.Result);
                Assert.AreEqual(savedCount1, DbContextObject1.Count);

                DbContextObject1.Count = 0;
                Task<bool> taskForEachAsync2 = context.dbContextDbSet1.AllAsync(p => p.ItemCount > 2);
                taskForEachAsync2.Wait();
                Assert.IsFalse(taskForEachAsync2.Result);
                Assert.AreEqual(savedCount2, DbContextObject1.Count);
            }
        }

        [Test]
        public void AnyAsyncNative() {
            AnyAsync_(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void AnyAsyncDXProvider() {
            AnyAsync_(() => new DbContextMultiClass());
        }
        private void AnyAsync_(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                Task<bool> taskForEachAsync = context.dbContextDbSet1.AnyAsync(p => p.ItemCount > 1);
                taskForEachAsync.Wait();
                bool res = taskForEachAsync.Result;
                Assert.IsTrue(res);
                Task<bool> taskForEachAsync1 = context.dbContextDbSet1.AnyAsync(p => p.ItemCount > 2);
                taskForEachAsync1.Wait();
                bool res1 = taskForEachAsync1.Result;
                Assert.IsFalse(res1);
            }
        }
        [Test]
        public void AverageAsyncNative() {
            AverageAsync_(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void AverageAsyncDXProvider() {
            AverageAsync_(() => new DbContextMultiClass());
        }
        private void AverageAsync_(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                Task<double> taskForEachAsync = context.dbContextDbSet1.AverageAsync(p => p.ItemCount);
                taskForEachAsync.Wait();
                double res = taskForEachAsync.Result;
                Assert.AreEqual(2, res);
            }
        }
        [Test] 
        public void ContainsAsyncNative() {
            ContainsAsync_(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test, Ignore("TODO")]
        public void ContainsAsyncDXProvider() {
            ContainsAsync_(() => new DbContextMultiClass());
        }
        //incorrect working in native provider
        private void ContainsAsync_(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.Add(new DbContextObject1() { ItemCount = 2 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                var item = context.dbContextDbSet1.Where(p => p.ItemCount == 2).First();
                Task<bool> taskForEachAsync = context.dbContextDbSet1.ContainsAsync(item);
                taskForEachAsync.Wait();
                bool res = taskForEachAsync.Result;
                Assert.IsTrue(res);
            }
        }
        [Test]
        public void CountAsyncNative() {
            CountAsync_(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void CountAsyncDXProvider() {
            CountAsync_(() => new DbContextMultiClass());
        }
        private void CountAsync_(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1());
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                Task<int> tAsync = context.dbContextDbSet1.CountAsync();
                tAsync.Wait();
                int res = tAsync.Result;
                Assert.AreNotEqual(res, null);
                Assert.AreEqual(res, 1);
            }
        }
        [Test]
        public void FirstAsyncNative() {
            FirstAsync_(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void FirstAsyncDXProvider() {
            FirstAsync_(() => new DbContextMultiClass());
        }
        private void FirstAsync_(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1());
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                Task<DbContextObject1> tAsync = context.dbContextDbSet1.FirstAsync();
                tAsync.Wait();
                var res = tAsync.Result;
                Assert.AreNotEqual(res, null);
            }
        }
        [Test]
        public void LoadAsyncNative() {
            LoadAsync_(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void LoadAsyncDXProvider() {
            LoadAsync_(() => new DbContextMultiClass());
        }
        private void LoadAsync_(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1());
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                DbContextObject1.Count = 0;
                Task tAsync = context.dbContextDbSet1.LoadAsync();
                tAsync.Wait();
//                Assert.AreEqual(1, DbContextObject1.Count);
            }
        }
        [Test]
        public void FirstOrDefaultAsyncNative() {
            FirstOrDefaultAsync_(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void FirstOrDefaultAsyncDXProvider() {
            FirstOrDefaultAsync_(() => new DbContextMultiClass());
        }
        private void FirstOrDefaultAsync_(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1());
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                Task<DbContextObject1> tAsync = context.dbContextDbSet1.FirstOrDefaultAsync(p => p.ItemCount == 5);
                tAsync.Wait();
                var res = tAsync.Result;
                Assert.IsNull(res);
                Task<DbContextObject1> tAsync1 = context.dbContextDbSet1.FirstOrDefaultAsync();
                tAsync1.Wait();
                var res1 = tAsync1.Result;
                Assert.IsNotNull(res1);

            }
        }
        [Test]
        public void MaxAsyncNative() {
            MaxAsync_(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void MaxAsyncDXProvider() {
            MaxAsync_(() => new DbContextMultiClass());
        }
        private void MaxAsync_(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                Task<int> tAsync = context.dbContextDbSet1.MaxAsync(p => p.ItemCount);
                tAsync.Wait();
                var res = tAsync.Result;
                Assert.AreEqual(res, 3);
            }
        }
        [Test]
        public void MinAsyncNative() {
            MinAsync_(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void MinAsyncDXProvider() {
            MinAsync_(() => new DbContextMultiClass());
        }
        private void MinAsync_(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                Task<int> tAsync = context.dbContextDbSet1.MinAsync(p => p.ItemCount);
                tAsync.Wait();
                var res = tAsync.Result;
                Assert.AreEqual(res, 1);
            }
        }
        [Test]
        public void SingleAsyncNative() {
            SingleAsync(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void SingleAsyncDXProvider() {
            SingleAsync(() => new DbContextMultiClass());
        }
        private void SingleAsync(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1());
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                Task<DbContextObject1> tAsync = context.dbContextDbSet1.SingleAsync();
                tAsync.Wait();
                var res = tAsync.Result;
                Assert.IsNotNull(res);
            }
        }
        [Test]
        public void SingleOrDefaultAsyncNative() {
            SingleOrDefaultAsync(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void SingleOrDefaultAsyncDXProvider() {
            SingleOrDefaultAsync(() => new DbContextMultiClass());
        }
        private void SingleOrDefaultAsync(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1());
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                Task<DbContextObject1> tAsync = context.dbContextDbSet1.SingleOrDefaultAsync();
                tAsync.Wait();
                var res = tAsync.Result;
                Assert.IsNotNull(res);
            }
        }
        [Test]
        public void SumAsyncNative() {
            SumAsync(() => new DbContextMultiClass().MakeRealDbContext());
        }
        [Test]
        public void SumAsyncDXProvider() {
            SumAsync(() => new DbContextMultiClass());
        }
        private void SumAsync(Func<DbContextMultiClass> createDbContext) {
            using(var context = createDbContext()) {
                context.Add(new DbContextObject1() { ItemCount = 1 });
                context.Add(new DbContextObject1() { ItemCount = 3 });
                context.SaveChanges();
            }
            using(var context = createDbContext()) {
                Task<int> tAsync = context.dbContextDbSet1.SumAsync(p => p.ItemCount);
                tAsync.Wait();
                var res = tAsync.Result;
                Assert.AreEqual(res, 4);
            }
        }
    }

    [TestFixture]
    public class InMemoryDbSetQueryableExtensionsOperationsTests : DbSetQueryableExtensionsOperationsTests {
        [SetUp]
        public void Setup() {
            SecurityTestHelper.CurrentDatabaseProviderType = SecurityTestHelper.DatabaseProviderType.IN_MEMORY;
            base.ClearDatabase();
        }
    }

    [TestFixture]
    public class LocalDb2012DbSetQueryableExtensionsOperationsTests : DbSetQueryableExtensionsOperationsTests {
        [SetUp]
        public void Setup() {
            SecurityTestHelper.CurrentDatabaseProviderType = SecurityTestHelper.DatabaseProviderType.LOCALDB_2012;
            base.ClearDatabase();
        }
    }
}
