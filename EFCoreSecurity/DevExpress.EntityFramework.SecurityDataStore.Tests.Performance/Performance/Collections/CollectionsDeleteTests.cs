using NUnit.Framework;
using System;
using System.Linq;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using DevExpress.EntityFramework.SecurityDataStore.Tests.Helpers;
using DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.Performance.Collections {
    [TestFixture]
    public abstract class CollectionsDeleteTests : BasePerformanceTestClass {
        [Test]
        public void DeleteObjectsWithoutPermissions() {
            DeleteObjects(TestType.WithoutPermissions);
        }
        [Test]
        public void DeleteObjectsWithOnePermission() {
            DeleteObjects(TestType.WithOnePermission);
        }
        [Test]
        public void DeleteObjectsWithMultiplePermissions() {
            DeleteObjects(TestType.WithMultiplePermissions);
        }

        public void DeleteObjects(TestType testType) {
            int count1 = 100;
            int count2 = 10;
            List<long> times = new List<long>();
            List<Func<IDbContextConnectionClass>> contexts = PerformanceTestsHelper.GetCollectionContextCreators();

            foreach(Func<IDbContextConnectionClass> createContext in contexts) {

                using(IDisposable disposableContextInterface = (IDisposable)createContext()) {
                    IDbContextConnectionClass contextInterface = (IDbContextConnectionClass)disposableContextInterface;
                    DbContext context = (DbContext)contextInterface;
                    context.ResetDatabase();

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

                using(IDisposable disposableContextInterface = (IDisposable)createContext()) {
                    IDbContextConnectionClass contextInterface = (IDbContextConnectionClass)disposableContextInterface;
                    DbContext context = (DbContext)contextInterface;

                    if(testType == TestType.WithOnePermission) {
                        SecurityDbContext securityDbContext = context as SecurityDbContext;
                        if(securityDbContext != null)
                            PerformanceTestsHelper.AddOneCollectionPermission(securityDbContext, SecurityOperation.Delete);
                    }

                    if(testType == TestType.WithMultiplePermissions) {
                        SecurityDbContext securityDbContext = context as SecurityDbContext;
                        if(securityDbContext != null)
                            PerformanceTestsHelper.AddMultipleCollectionPermissions(securityDbContext, SecurityOperation.Delete);
                    }

                    

                    List<Company> objects = contextInterface.Company.Select(obj => obj).Include(obj => obj.Offices).ToList();
                    Assert.AreEqual(count1, objects.Count);

                    Stopwatch watch = new Stopwatch();
                    watch.Start();

                    for(int companyIndex = 1; companyIndex < count1; companyIndex++) {
                        Company company = objects[companyIndex];

                        foreach(var office in company.Offices.Where(office => office.Id % 2 == 0).ToList())
                            company.Offices.Remove(office);
                    }

                    context.SaveChanges();

                    watch.Stop();
                    times.Add(watch.ElapsedMilliseconds);
                }
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
    public class InMemoryCollectionsDeleteTests : CollectionsDeleteTests {
        [SetUp]
        public void Setup() {
            SecurityTestHelper.CurrentDatabaseProviderType = SecurityTestHelper.DatabaseProviderType.IN_MEMORY;

            SetTimeDifference(TestType.WithoutPermissions, 200);
            SetTimeDifference(TestType.WithOnePermission, 200);
            SetTimeDifference(TestType.WithMultiplePermissions, 200);
        }
    }

    [TestFixture]
    public class LocalDb2012CollectionsDeleteTests : CollectionsDeleteTests {
        [SetUp]
        public void Setup() {
            SecurityTestHelper.CurrentDatabaseProviderType = SecurityTestHelper.DatabaseProviderType.LOCALDB_2012;

            SetTimeDifference(TestType.WithoutPermissions, 200);
            SetTimeDifference(TestType.WithOnePermission, 300);
            SetTimeDifference(TestType.WithMultiplePermissions, 300);
        }
    }

}
