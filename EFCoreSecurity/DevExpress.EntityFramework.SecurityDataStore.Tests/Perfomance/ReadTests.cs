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
    public abstract class ReadTests {
        [Test]
        public void ReadObjectsWithoutPermissions() {
            int count = 100;
            List<long> times = new List<long>();
            List<Func<IDbContextMultiClass>> contexts = PerfomanceTestsHelper.GetContextCreators();

            foreach(Func<IDbContextMultiClass> createContext in contexts) {
                using(IDisposable disposableInterface = (IDisposable)createContext()) {
                    IDbContextMultiClass contextInterface = (IDbContextMultiClass)disposableInterface;
                    DbContext context = (DbContext)contextInterface;
                    context.ResetDatabase();

                    for(int i = 0; i < count; i++) {
                        DbContextObject1 obj = new DbContextObject1();
                        obj.Description = "Description " + i.ToString();
                        context.Add(obj);
                    }
                    context.SaveChanges();
                }

                using(IDisposable disposableInterface = (IDisposable)createContext()) {
                    IDbContextMultiClass contextInterface = (IDbContextMultiClass)disposableInterface;
                    DbContext context = (DbContext)contextInterface;

                    Stopwatch watch = new Stopwatch();
                    watch.Start();

                    IQueryable<DbContextObject1> objectsQuery = contextInterface.dbContextDbSet1.Select(obj => obj);
                    List<DbContextObject1> objects = objectsQuery.ToList();
                    Assert.AreEqual(count, objects.Count);

                    watch.Stop();
                    times.Add(watch.ElapsedMilliseconds);
                }
            }

            long securedContextTime = PerfomanceTestsHelper.GetSecuredContextTime(times);
            long nativeContextTime = PerfomanceTestsHelper.GetNativeContextTime(times);

            Assert.IsTrue(false, "our: " + securedContextTime.ToString() + " ms, native: " + nativeContextTime.ToString() + " ms");
        }

        [Test]
        public void ReadObjectsWithOnePermission() {
            int count = 100;
            List<long> times = new List<long>();
            List<Func<IDbContextMultiClass>> contexts = PerfomanceTestsHelper.GetContextCreators();

            foreach(Func<IDbContextMultiClass> createContext in contexts) {
                using(IDisposable disposableInterface = (IDisposable)createContext()) {
                    IDbContextMultiClass contextInterface = (IDbContextMultiClass)disposableInterface;
                    DbContext context = (DbContext)contextInterface;
                    context.ResetDatabase();

                    for(int i = 0; i < count; i++) {
                        DbContextObject1 obj = new DbContextObject1();
                        obj.Description = "Description " + i.ToString();
                        context.Add(obj);
                    }
                    context.SaveChanges();
                }

                using(IDisposable disposableInterface = (IDisposable)createContext()) {
                    IDbContextMultiClass contextInterface = (IDbContextMultiClass)disposableInterface;
                    DbContext context = (DbContext)contextInterface;

                    SecurityDbContext securityDbContext = context as SecurityDbContext;
                    if(securityDbContext != null)
                        PerfomanceTestsHelper.AddOnePermission(securityDbContext, SecurityOperation.Read);

                    Stopwatch watch = new Stopwatch();
                    watch.Start();

                    List<DbContextObject1> objects = contextInterface.dbContextDbSet1.Select(obj => obj).ToList();
                    Assert.AreEqual(count, objects.Count);

                    watch.Stop();
                    times.Add(watch.ElapsedMilliseconds);
                }
            }

            long securedContextTime = PerfomanceTestsHelper.GetSecuredContextTime(times);
            long nativeContextTime = PerfomanceTestsHelper.GetNativeContextTime(times);

            Assert.IsTrue(false, "our: " + securedContextTime.ToString() + " ms, native: " + nativeContextTime.ToString() + " ms");
        }

        [Test]
        public void ReadObjectsWithMultiplePermissions() {
            int count = 100;
            List<long> times = new List<long>();
            List<Func<IDbContextMultiClass>> contexts = PerfomanceTestsHelper.GetContextCreators();

            foreach(Func<IDbContextMultiClass> createContext in contexts) {
                using(IDisposable disposableInterface = (IDisposable)createContext()) {
                    IDbContextMultiClass contextInterface = (IDbContextMultiClass)disposableInterface;
                    DbContext context = (DbContext)contextInterface;
                    context.ResetDatabase();

                    for(int i = 0; i < count; i++) {
                        DbContextObject1 obj = new DbContextObject1();
                        obj.Description = "Description " + i.ToString();
                        context.Add(obj);
                    }
                    context.SaveChanges();
                }

                using(IDisposable disposableInterface = (IDisposable)createContext()) {
                    IDbContextMultiClass contextInterface = (IDbContextMultiClass)disposableInterface;
                    DbContext context = (DbContext)contextInterface;

                    SecurityDbContext securityDbContext = context as SecurityDbContext;
                    if(securityDbContext != null)
                        PerfomanceTestsHelper.AddMultiplePermissions(securityDbContext, SecurityOperation.Read);

                    Stopwatch watch = new Stopwatch();
                    watch.Start();

                    List<DbContextObject1> objects = contextInterface.dbContextDbSet1.Select(obj => obj).ToList();
                    Assert.AreEqual(count, objects.Count);

                    watch.Stop();
                    times.Add(watch.ElapsedMilliseconds);
                }
            }

            long securedContextTime = PerfomanceTestsHelper.GetSecuredContextTime(times);
            long nativeContextTime = PerfomanceTestsHelper.GetNativeContextTime(times);

            Assert.IsTrue(false, "our: " + securedContextTime.ToString() + " ms, native: " + nativeContextTime.ToString() + " ms");
        }
    }

    [TestFixture]
    public class InMemoryReadTests : ReadTests {
        [SetUp]
        public void Setup() {
            SecurityTestHelper.CurrentDatabaseProviderType = SecurityTestHelper.DatabaseProviderType.IN_MEMORY;
        }
    }

    [TestFixture]
    public class LocalDb2012ReadTests : ReadTests {
        [SetUp]
        public void Setup() {
            SecurityTestHelper.CurrentDatabaseProviderType = SecurityTestHelper.DatabaseProviderType.LOCALDB_2012;
        }
    }
}
