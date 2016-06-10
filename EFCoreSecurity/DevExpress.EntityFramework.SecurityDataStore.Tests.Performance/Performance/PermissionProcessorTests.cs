using DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts;
using DevExpress.EntityFramework.SecurityDataStore.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.Performance {
    [TestFixture]
    public abstract class PermissionProcessorPerformanceTests {
        private DbContextMultiClass dbContextMultiClass;
        private NativeDbContextMultiClass nativeDbContextMultiClass;
        [SetUp]
        public void SetUp() {
            dbContextMultiClass = new DbContextMultiClass();
            nativeDbContextMultiClass = new NativeDbContextMultiClass();
            dbContextMultiClass.ResetDatabase();
            nativeDbContextMultiClass.ResetDatabase();
        }

        [Test]
        public void DeleteTest() {
            int count = 100;
            bool firstStart = true;
            List<long> times = new List<long>();
            List<Func<DbContextMultiClass>> contexts = new List<Func<DbContextMultiClass>>();
            for(int i = 0; i < 5; i++) {
                contexts.Add(() =>new DbContextMultiClass());
            }
            foreach(Func<DbContextMultiClass> createContext in contexts) {
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

                using(DbContextMultiClass context = createContext()) {

                    List<DbContextObject1> objects = context.dbContextDbSet1.Select(obj => obj).ToList();
                    Assert.AreEqual(count, objects.Count);

                    SecurityDbContext securityDbContext = context;
                    PerformanceTestsHelper.AddMultiplePermissions(securityDbContext, SecurityOperation.Delete);

                    Stopwatch watch = new Stopwatch();
                    watch.Start();

                    foreach(DbContextObject1 obj in objects) {
                        context.Security.PermissionProcessor.IsGranted(typeof(DbContextObject1), SecurityOperation.Delete, obj);
                    }
                    watch.Stop();
                    if(firstStart) {
                        firstStart = false;
                    }
                    else {
                        times.Add(watch.ElapsedMilliseconds);
                    }
                }
            }
            
            Assert.LessOrEqual(times.Average(),1);
        }
        [Test]
        public void WriteTest() {
            int count = 100;
            bool firstStart = true;
            List<long> times = new List<long>();
            List<Func<DbContextMultiClass>> contexts = new List<Func<DbContextMultiClass>>();
            for(int i = 0; i < 5; i++) {
                contexts.Add(() => new DbContextMultiClass());
            }
            foreach(Func<DbContextMultiClass> createContext in contexts) {
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

                using(DbContextMultiClass context = createContext()) {

                    List<DbContextObject1> objects = context.dbContextDbSet1.Select(obj => obj).ToList();
                    Assert.AreEqual(count, objects.Count);

                    SecurityDbContext securityDbContext = context as SecurityDbContext;
                    if(securityDbContext != null)
                        PerformanceTestsHelper.AddMultiplePermissions(securityDbContext, SecurityOperation.Write);

                    Stopwatch watch = new Stopwatch();
                    watch.Start();

                    foreach(DbContextObject1 obj in objects) {
                        context.Security.PermissionProcessor.IsGranted(typeof(DbContextObject1), SecurityOperation.Write, obj);
                    }
                    watch.Stop();
                    if(firstStart) {
                        firstStart = false;
                    }
                    else {
                        times.Add(watch.ElapsedMilliseconds);
                    }
                }
            }
            Assert.LessOrEqual(times.Average(), 1);
        }
        [Test]
        public void ReadTest() {
            int count = 100;
            bool firstStart = true;
            List<long> times = new List<long>();
            List<Func<DbContextMultiClass>> contexts = new List<Func<DbContextMultiClass>>();
            for(int i = 0; i < 5; i++) {
                contexts.Add(() => new DbContextMultiClass());
            }
            foreach(Func<DbContextMultiClass> createContext in contexts) {
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

                using(DbContextMultiClass context = createContext()) {

                    List<DbContextObject1> objects = context.dbContextDbSet1.Select(obj => obj).ToList();
                    Assert.AreEqual(count, objects.Count);

                    SecurityDbContext securityDbContext = context as SecurityDbContext;
                    if(securityDbContext != null)
                        PerformanceTestsHelper.AddMultiplePermissions(securityDbContext, SecurityOperation.Read);

                    Stopwatch watch = new Stopwatch();
                    watch.Start();

                    foreach(DbContextObject1 obj in objects) {
                        context.Security.PermissionProcessor.IsGranted(typeof(DbContextObject1), SecurityOperation.Read, obj);
                    }
                    watch.Stop();
                    if(firstStart) {
                        firstStart = false;
                    }
                    else {
                        times.Add(watch.ElapsedMilliseconds);
                    }
                }
            }
            Assert.LessOrEqual(times.Average(), 1);
        }
    }
    [TestFixture]
    public class InMemoryPermissionProcessorPerformanceTests : PermissionProcessorPerformanceTests {
        [SetUp]
        public void Setup() {
            SecurityTestHelper.CurrentDatabaseProviderType = SecurityTestHelper.DatabaseProviderType.IN_MEMORY;
        }
    }
}
