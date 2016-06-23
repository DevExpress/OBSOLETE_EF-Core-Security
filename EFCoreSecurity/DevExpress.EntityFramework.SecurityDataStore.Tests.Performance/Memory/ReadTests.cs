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

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.Performance.Memory {
    [TestFixture]
    public abstract class ReadTests {
        [Test]
        public void ReadObjectsWithoutPermissions() {
            ReadObjects(TestType.WithoutPermissions);
        }
        [Test]
        public void ReadObjectsWithOnePermission() {
            ReadObjects(TestType.WithOnePermission);
        }
        [Test]
        public void ReadObjectsWithMultiplePermissions() {
            ReadObjects(TestType.WithMultiplePermissions);
        }

        public void ReadObjects(TestType testType) {
            int count = 100;
            List<long> memoryUsages = new List<long>();
            List<Func<IDbContextMultiClass>> contexts = PerformanceTestsHelper.GetMemoryTestsContextCreators();

            foreach(Func<IDbContextMultiClass> createContext in contexts) {
                long initialUsedMemory = 0;
                long usedMemory = 0;

                initialUsedMemory = PerformanceTestsHelper.GetCurrentUsedMemory();

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

                    if(testType == TestType.WithOnePermission) {
                        SecurityDbContext securityDbContext = context as SecurityDbContext;
                        if(securityDbContext != null)
                            PerformanceTestsHelper.AddOnePermission(securityDbContext, SecurityOperation.Read);
                    }

                    if(testType == TestType.WithMultiplePermissions) {
                        SecurityDbContext securityDbContext = context as SecurityDbContext;
                        if(securityDbContext != null)
                            PerformanceTestsHelper.AddMultiplePermissions(securityDbContext, SecurityOperation.Read);
                    }

                    List<DbContextObject1> objects = contextInterface.dbContextDbSet1.Select(obj => obj).ToList();
                    Assert.AreEqual(count, objects.Count);
                }

                long beforeCollect = GC.GetTotalMemory(true);
                usedMemory = PerformanceTestsHelper.GetCurrentUsedMemory();

                memoryUsages.Add(usedMemory - initialUsedMemory);
            }

            double securedContextBytesGrow = PerformanceTestsHelper.GetSecuredContextValue(memoryUsages);
            double nativeContextBytesGrow = PerformanceTestsHelper.GetNativeContextValue(memoryUsages);

            Assert.IsTrue(false, "our: " + securedContextBytesGrow.ToString() + " bytes, native: " + nativeContextBytesGrow.ToString() + " bytes");
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
