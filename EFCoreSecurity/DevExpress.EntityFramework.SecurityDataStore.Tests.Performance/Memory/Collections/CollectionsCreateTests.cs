using NUnit.Framework;
using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using DevExpress.EntityFramework.SecurityDataStore.Tests.Helpers;
using DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.Performance.Memory.Collections {
    [TestFixture]
    public abstract class CollectionsCreateTests {
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
        [Test]
        public void CreateObjects(TestType testType) {
            int count1 = 100;
            int count2 = 10;
            List<long> memoryUsages = new List<long>();
            List<Func<IDbContextConnectionClass>> contexts = PerformanceTestsHelper.GetMemoryTestsCollectionContextCreators();

            foreach(Func<IDbContextConnectionClass> createContext in contexts) {
                long initialUsedMemory = 0;
                long usedMemory = 0;

                initialUsedMemory = PerformanceTestsHelper.GetCurrentUsedMemory();

                using(IDisposable disposableContextInterface = (IDisposable)createContext()) {
                    IDbContextConnectionClass contextInterface = (IDbContextConnectionClass)disposableContextInterface;
                    DbContext context = (DbContext)contextInterface;
                    context.ResetDatabase();

                    if(testType == TestType.WithOnePermission) {
                        SecurityDbContext securityDbContext = context as SecurityDbContext;
                        if(securityDbContext != null)
                            PerformanceTestsHelper.AddOneCollectionPermission(securityDbContext, SecurityOperation.Create);
                    }

                    if(testType == TestType.WithMultiplePermissions) {
                        SecurityDbContext securityDbContext = context as SecurityDbContext;
                        if(securityDbContext != null)
                            PerformanceTestsHelper.AddMultipleCollectionPermissions(securityDbContext, SecurityOperation.Create);
                    }

                    for(int companyIndex = 0; companyIndex < count1; companyIndex++) {
                        string companySuffix = companyIndex.ToString();

                        Company company = new Company();
                        company.CompanyName = companySuffix;
                        company.Description = "Description" + companySuffix;

                        for(int officeIndex = 0; officeIndex < count2; officeIndex++) {
                            string officeSuffix = officeIndex.ToString();
                            Office office = new Office();
                            office.Name = officeSuffix;
                            office.Description = "Description" + companySuffix;

                            company.Offices.Add(office);
                        }

                        contextInterface.Company.Add(company);
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

    [TestFixture, Ignore("Memory tests are disabled")]
    public class InMemoryCollectionsCreateTests : CollectionsCreateTests {
        [SetUp]
        public void Setup() {
            SecurityTestHelper.CurrentDatabaseProviderType = SecurityTestHelper.DatabaseProviderType.IN_MEMORY;
        }
    }

    [TestFixture, Ignore("Memory tests are disabled")]
    public class LocalDb2012CollectionsCreateTests : CollectionsCreateTests {
        [SetUp]
        public void Setup() {
            SecurityTestHelper.CurrentDatabaseProviderType = SecurityTestHelper.DatabaseProviderType.LOCALDB_2012;
        }
    }

}
