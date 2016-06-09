using NUnit.Framework;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Collections.Generic;
using DevExpress.EntityFramework.SecurityDataStore.Tests.Helpers;
using DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.Perfomance {
    [TestFixture]
    public abstract class CreateTests {
        [Test]
        public void CreateObjectsWithoutPermissions() {
            int count = 1000;
            List<long> times = new List<long>();
            List<Func<IDbContextMultiClass>> contexts = PerfomanceTestsHelper.GetContextCreators();

            foreach(Func<IDbContextMultiClass> createContext in contexts) {
                IDbContextMultiClass contextInterface = createContext();
                DbContext context = (DbContext)contextInterface;
                context.ResetDatabase();

                Stopwatch watch = new Stopwatch();
                watch.Start();

                for(int i = 0; i < count; i++) {
                    DbContextObject1 obj = new DbContextObject1();
                    obj.Description = "Description " + i.ToString();
                    context.Add(obj);
                }
                context.SaveChanges();
                watch.Stop();
                times.Add(watch.ElapsedMilliseconds);
            }

            long securedContextTime = PerfomanceTestsHelper.getSecuredContextTime(times);
            long nativeContextTime = PerfomanceTestsHelper.getNativeContextTime(times);

            Assert.IsTrue(false, "our: " + securedContextTime.ToString() + " ms, native: " + nativeContextTime.ToString() + " ms");
        }
        [Test]
        public void CreateObjectsWithOnePermission() {
            int count = 1000;
            List<long> times = new List<long>();
            List<Func<IDbContextMultiClass>> contexts = PerfomanceTestsHelper.GetContextCreators();

            foreach(Func<IDbContextMultiClass> createContext in contexts) {
                IDbContextMultiClass contextInterface = createContext();
                DbContext context = (DbContext)contextInterface;
                context.ResetDatabase();

                SecurityDbContext securityDbContext = context as SecurityDbContext;
                if(securityDbContext != null)
                    PerfomanceTestsHelper.AddOnePermission(securityDbContext, SecurityOperation.Create);

                Stopwatch watch = new Stopwatch();
                watch.Start();

                for(int i = 0; i < count; i++) {
                    DbContextObject1 obj = new DbContextObject1();
                    obj.Description = "Description " + i.ToString();
                    context.Add(obj);
                }
                context.SaveChanges();
                watch.Stop();
                times.Add(watch.ElapsedMilliseconds);
            }

            long securedContextTime = PerfomanceTestsHelper.getSecuredContextTime(times);
            long nativeContextTime = PerfomanceTestsHelper.getNativeContextTime(times);

            Assert.IsTrue(false, "our: " + securedContextTime.ToString() + " ms, native: " + nativeContextTime.ToString() + " ms");
        }
        [Test]
        public void CreateObjectsWithMultiplePermissions() {
            int count = 1000;
            List<long> times = new List<long>();
            List<Func<IDbContextMultiClass>> contexts = PerfomanceTestsHelper.GetContextCreators();

            foreach(Func<IDbContextMultiClass> createContext in contexts) {
                IDbContextMultiClass contextInterface = createContext();
                DbContext context = (DbContext)contextInterface;
                context.ResetDatabase();

                SecurityDbContext securityDbContext = context as SecurityDbContext;
                if(securityDbContext != null)
                    PerfomanceTestsHelper.AddMultiplePermissions(securityDbContext, SecurityOperation.Create);

                Stopwatch watch = new Stopwatch();
                watch.Start();

                for(int i = 0; i < count; i++) {
                    DbContextObject1 obj = new DbContextObject1();
                    obj.Description = "Description " + i.ToString();
                    context.Add(obj);
                }
                context.SaveChanges();
                watch.Stop();
                times.Add(watch.ElapsedMilliseconds);
            }

            long securedContextTime = PerfomanceTestsHelper.getSecuredContextTime(times);
            long nativeContextTime = PerfomanceTestsHelper.getNativeContextTime(times);

            Assert.IsTrue(false, "our: " + securedContextTime.ToString() + " ms, native: " + nativeContextTime.ToString() + " ms");
        }
    }

    [TestFixture]
    public class InMemoryCreateTests : CreateTests {
        [SetUp]
        public void Setup() {
            SecurityTestHelper.CurrentDatabaseProviderType = SecurityTestHelper.DatabaseProviderType.IN_MEMORY;
        }
    }

    [TestFixture]
    public class LocalDb2012CreateTests : CreateTests {
        [SetUp]
        public void Setup() {
            SecurityTestHelper.CurrentDatabaseProviderType = SecurityTestHelper.DatabaseProviderType.LOCALDB_2012;
        }
    }
}
