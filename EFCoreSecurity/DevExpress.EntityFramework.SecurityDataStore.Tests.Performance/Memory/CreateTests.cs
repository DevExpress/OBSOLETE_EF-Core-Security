using NUnit.Framework;
using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using DevExpress.EntityFramework.SecurityDataStore.Tests.Helpers;
using DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts;
using DevExpress.EntityFramework.SecurityDataStore.Tests.Performance;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.Performance.Memory {
    [TestFixture]
    public abstract class CreateTests {
        

        [Test]
        public void CreateObjectsWithoutPermissions() {
            CreateObjects(TestType.WithoutPermissions);
        }
        [Test]
        public void CreateObjectsWithOnePermission() {
            CreateObjects(TestType.WithOnePermission);
        }
        [Test]
        public void CreateObjectsWithMultiplePermissions() {
            CreateObjects(TestType.WithMultiplePermissions);
        }       
         
        public void CreateObjects(TestType testType) {
            int count = 1000;
            List<long> memoryUsages = new List<long>();
            List<Func<IDbContextMultiClass>> contexts = PerformanceTestsHelper.GetMemoryTestsContextCreators();

            foreach(Func<IDbContextMultiClass> createContext in contexts) {
                long initialUsedMemory = 0;
                long usedMemory = 0;

                initialUsedMemory = PerformanceTestsHelper.GetCurrentUsedMemory();

                using(IDisposable disposableContextInterface = (IDisposable)createContext()){
                    IDbContextMultiClass contextInterface = (IDbContextMultiClass)disposableContextInterface;
                    DbContext context = (DbContext)contextInterface;
                    context.ResetDatabase();

                    if(testType == TestType.WithOnePermission) {
                        SecurityDbContext securityDbContext = context as SecurityDbContext;
                        if(securityDbContext != null)
                            PerformanceTestsHelper.AddOnePermission(securityDbContext, SecurityOperation.Create);
                    }

                    if(testType == TestType.WithMultiplePermissions) {
                        SecurityDbContext securityDbContext = context as SecurityDbContext;
                        if(securityDbContext != null)
                            PerformanceTestsHelper.AddMultiplePermissions(securityDbContext, SecurityOperation.Create);
                    }                                      

                    for(int i = 0; i < count; i++) {
                        DbContextObject1 obj = new DbContextObject1();
                        obj.Description = "Description " + i.ToString();
                        context.Add(obj);
                    }
                    context.SaveChanges();                    
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
