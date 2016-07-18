using NUnit.Framework;
using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using DevExpress.EntityFramework.SecurityDataStore.Tests.Helpers;
using DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.Performance.Collections {
    [TestFixture]
    public abstract class CollectionsCreateTests : BasePerformanceTestClass {
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
            int count1 = 100;
            int count2 = 10;
            List<long> times = new List<long>();
            List<Func<IDbContextConnectionClass>> contexts = PerformanceTestsHelper.GetCollectionContextCreators();

            foreach(Func<IDbContextConnectionClass> createContext in contexts) {
                IDbContextConnectionClass contextInterface = createContext();
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

                Stopwatch watch = new Stopwatch();
                watch.Start();

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

                watch.Stop();
                times.Add(watch.ElapsedMilliseconds);
            }

            double securedContextTime = PerformanceTestsHelper.GetSecuredContextValue(times);
            double nativeContextTime = PerformanceTestsHelper.GetNativeContextValue(times);

            double nominalTimeDifference = GetTimeDifference(testType);
            double timeDifference = securedContextTime - nativeContextTime;
            Assert.IsTrue(timeDifference <= nominalTimeDifference, GetTimeDifferenceErrorString(timeDifference, nominalTimeDifference));
            Debug.WriteLine(GetDebugTimeString(securedContextTime, nativeContextTime));
        }
    }

    [TestFixture]
    public class InMemoryCollectionsCreateTests : CollectionsCreateTests {
        [SetUp]
        public void Setup() {
            SecurityTestHelper.CurrentDatabaseProviderType = SecurityTestHelper.DatabaseProviderType.IN_MEMORY;

            SetTimeDifference(TestType.WithoutPermissions, 300);
            SetTimeDifference(TestType.WithOnePermission, 300);
            SetTimeDifference(TestType.WithMultiplePermissions, 300);
        }
    }

    [TestFixture]
    public class LocalDb2012CollectionsCreateTests : CollectionsCreateTests {
        [SetUp]
        public void Setup() {
            SecurityTestHelper.CurrentDatabaseProviderType = SecurityTestHelper.DatabaseProviderType.LOCALDB_2012;

            SetTimeDifference(TestType.WithoutPermissions, 400);
            SetTimeDifference(TestType.WithOnePermission, 400);
            SetTimeDifference(TestType.WithMultiplePermissions, 400);
        }
    }

}
