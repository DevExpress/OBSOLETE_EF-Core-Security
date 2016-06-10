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

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.Performance {
    [TestFixture]
    public abstract class WriteTests {
        [Test]
        public void WriteObjectsWithoutPermissions() {
            int count = 100;
            List<long> times = new List<long>();
            List<Func<IDbContextMultiClass>> contexts = PerformanceTestsHelper.GetContextCreators();

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

                    List<DbContextObject1> objects = contextInterface.dbContextDbSet1.Select(obj => obj).ToList();
                    Assert.AreEqual(count, objects.Count);

                    Stopwatch watch = new Stopwatch();
                    watch.Start();

                    for(int i = 0; i < count; i++) {
                        DbContextObject1 obj = objects[i];
                        obj.Description = "Description " + (i + 1).ToString();
                    }

                    context.SaveChanges();

                    watch.Stop();
                    times.Add(watch.ElapsedMilliseconds);
                }
            }

            long securedContextTime = PerformanceTestsHelper.GetSecuredContextTime(times);
            long nativeContextTime = PerformanceTestsHelper.GetNativeContextTime(times);

            Assert.IsTrue(false, "our: " + securedContextTime.ToString() + " ms, native: " + nativeContextTime.ToString() + " ms");
        }
        [Test]
        public void WriteObjectsWithOnePermission() {
            int count = 100;
            List<long> times = new List<long>();
            List<Func<IDbContextMultiClass>> contexts = PerformanceTestsHelper.GetContextCreators();

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

                    List<DbContextObject1> objects = contextInterface.dbContextDbSet1.Select(obj => obj).ToList();
                    Assert.AreEqual(count, objects.Count);

                    SecurityDbContext securityDbContext = context as SecurityDbContext;
                    if(securityDbContext != null)
                        PerformanceTestsHelper.AddOnePermission(securityDbContext, SecurityOperation.Write);

                    Stopwatch watch = new Stopwatch();
                    watch.Start();

                    for(int i = 0; i < count; i++) {
                        DbContextObject1 obj = objects[i];
                        obj.Description = "Description " + (i + 1).ToString();
                    }

                    context.SaveChanges();

                    watch.Stop();
                    times.Add(watch.ElapsedMilliseconds);
                }
            }

            long securedContextTime = PerformanceTestsHelper.GetSecuredContextTime(times);
            long nativeContextTime = PerformanceTestsHelper.GetNativeContextTime(times);

            Assert.IsTrue(false, "our: " + securedContextTime.ToString() + " ms, native: " + nativeContextTime.ToString() + " ms");
        }
        [Test]
        public void WriteObjectsWithMultiplePermissions() {
            int count = 100;
            List<long> times = new List<long>();
            List<Func<IDbContextMultiClass>> contexts = PerformanceTestsHelper.GetContextCreators();

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

                    List<DbContextObject1> objects = contextInterface.dbContextDbSet1.Select(obj => obj).ToList();
                    Assert.AreEqual(count, objects.Count);

                    SecurityDbContext securityDbContext = context as SecurityDbContext;
                    if(securityDbContext != null)
                        PerformanceTestsHelper.AddMultiplePermissions(securityDbContext, SecurityOperation.Write);

                    Stopwatch watch = new Stopwatch();
                    watch.Start();

                    for(int i = 0; i < count; i++) {
                        DbContextObject1 obj = objects[i];
                        obj.Description = "Description " + (i + 1).ToString();
                    }

                    context.SaveChanges();

                    watch.Stop();
                    times.Add(watch.ElapsedMilliseconds);
                }
            }

            long securedContextTime = PerformanceTestsHelper.GetSecuredContextTime(times);
            long nativeContextTime = PerformanceTestsHelper.GetNativeContextTime(times);

            Assert.IsTrue(false, "our: " + securedContextTime.ToString() + " ms, native: " + nativeContextTime.ToString() + " ms");
        }
    }

    [TestFixture]
    public class InMemoryWriteTests : WriteTests {
        [SetUp]
        public void Setup() {
            SecurityTestHelper.CurrentDatabaseProviderType = SecurityTestHelper.DatabaseProviderType.IN_MEMORY;
        }
    }

    [TestFixture]
    public class LocalDb2012WriteTests : WriteTests {
        [SetUp]
        public void Setup() {
            SecurityTestHelper.CurrentDatabaseProviderType = SecurityTestHelper.DatabaseProviderType.LOCALDB_2012;
        }
    }
}
