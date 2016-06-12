using NUnit.Framework;
using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using DevExpress.EntityFramework.SecurityDataStore.Tests.Helpers;
using DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.Performance.Collections {
    [TestFixture]
    public abstract class CollectionsCreateTests {
        [Test]
        public void CreateObjectsWithoutPermissions() {
            int count1 = 100;
            int count2 = 10;
            List<long> times = new List<long>();
            List<Func<IDbContextConnectionClass>> contexts = PerformanceTestsHelper.GetCollectionContextCreators();

            foreach(Func<IDbContextConnectionClass> createContext in contexts) {
                IDbContextConnectionClass contextInterface = createContext();
                DbContext context = (DbContext)contextInterface;
                context.ResetDatabase();

                Stopwatch watch = new Stopwatch();
                watch.Start();

                for (int companyIndex = 1; companyIndex < count1; companyIndex++) {
                    string companySuffix = companyIndex.ToString();

                    Company company = new Company();
                    company.CompanyName = companySuffix;
                    company.Description = companySuffix;

                    //Person person = new Person();
                    //person.PersonName = companySuffix;
                    //person.Description = companySuffix;
                    //company.Person = person;
                    //person.Company = company;

                    for (int officeIndex = 1; officeIndex < count2; officeIndex++) {
                        string officeSuffix = officeIndex.ToString();
                        Office office = new Office();
                        office.Name = officeSuffix;
                        office.Description = officeSuffix;

                        company.Offices.Add(office);
                    }
                    

                    // companyFirst.Offices.Add(office);
                    contextInterface.Company.Add(company);
                    // contextInterface.Persons.Add(person);
                }
                context.SaveChanges();

                watch.Stop();
                times.Add(watch.ElapsedMilliseconds);
            }

            long securedContextTime = PerformanceTestsHelper.GetSecuredContextTime(times);
            long nativeContextTime = PerformanceTestsHelper.GetNativeContextTime(times);

            Assert.IsTrue(false, "our: " + securedContextTime.ToString() + " ms, native: " + nativeContextTime.ToString() + " ms");
        }
        [Test]
        public void CreateObjectsWithOnePermission() {
            int count1 = 100;
            int count2 = 10;
            List<long> times = new List<long>();
            List<Func<IDbContextConnectionClass>> contexts = PerformanceTestsHelper.GetCollectionContextCreators();

            foreach (Func<IDbContextConnectionClass> createContext in contexts) {
                IDbContextConnectionClass contextInterface = createContext();
                DbContext context = (DbContext)contextInterface;
                context.ResetDatabase();

                SecurityDbContext securityDbContext = context as SecurityDbContext;
                if (securityDbContext != null)
                    PerformanceTestsHelper.AddOneCollectionPermission(securityDbContext, SecurityOperation.Create);

                Stopwatch watch = new Stopwatch();
                watch.Start();

                for (int companyIndex = 1; companyIndex < count1; companyIndex++) {
                    string companySuffix = companyIndex.ToString();

                    Company company = new Company();
                    company.CompanyName = companySuffix;
                    company.Description = "Description" + companySuffix;

                    for (int officeIndex = 1; officeIndex < count2; officeIndex++) {
                        string officeSuffix = officeIndex.ToString();
                        Office office = new Office();
                        office.Name = officeSuffix;
                        office.Description = "Description" + companySuffix; ;

                        company.Offices.Add(office);
                    }

                    contextInterface.Company.Add(company);
                }
                context.SaveChanges();

                watch.Stop();
                times.Add(watch.ElapsedMilliseconds);
            }

            long securedContextTime = PerformanceTestsHelper.GetSecuredContextTime(times);
            long nativeContextTime = PerformanceTestsHelper.GetNativeContextTime(times);

            Assert.IsTrue(false, "our: " + securedContextTime.ToString() + " ms, native: " + nativeContextTime.ToString() + " ms");
        }
        [Test]
        public void CreateObjectsWithMultiplePermissions() {
            int count1 = 100;
            int count2 = 10;
            List<long> times = new List<long>();
            List<Func<IDbContextConnectionClass>> contexts = PerformanceTestsHelper.GetCollectionContextCreators();

            foreach (Func<IDbContextConnectionClass> createContext in contexts) {
                IDbContextConnectionClass contextInterface = createContext();
                DbContext context = (DbContext)contextInterface;
                context.ResetDatabase();

                SecurityDbContext securityDbContext = context as SecurityDbContext;
                if (securityDbContext != null)
                    PerformanceTestsHelper.AddMultipleCollectionPermissions(securityDbContext, SecurityOperation.Create);

                Stopwatch watch = new Stopwatch();
                watch.Start();

                for (int companyIndex = 1; companyIndex < count1; companyIndex++) {
                    string companySuffix = companyIndex.ToString();

                    Company company = new Company();
                    company.CompanyName = companySuffix;
                    company.Description = "Description" + companySuffix;

                    for (int officeIndex = 1; officeIndex < count2; officeIndex++) {
                        string officeSuffix = officeIndex.ToString();
                        Office office = new Office();
                        office.Name = officeSuffix;
                        office.Description = "Description" + companySuffix; ;

                        company.Offices.Add(office);
                    }

                    contextInterface.Company.Add(company);
                }
                context.SaveChanges();

                watch.Stop();
                times.Add(watch.ElapsedMilliseconds);
            }

            long securedContextTime = PerformanceTestsHelper.GetSecuredContextTime(times);
            long nativeContextTime = PerformanceTestsHelper.GetNativeContextTime(times);

            Assert.IsTrue(false, "our: " + securedContextTime.ToString() + " ms, native: " + nativeContextTime.ToString() + " ms");
        }
    }

    [TestFixture]
    public class InMemoryCollectionsCreateTests : CollectionsCreateTests {
        [SetUp]
        public void Setup() {
            SecurityTestHelper.CurrentDatabaseProviderType = SecurityTestHelper.DatabaseProviderType.IN_MEMORY;
        }
    }

    [TestFixture]
    public class LocalDb2012CollectionsCreateTests : CollectionsCreateTests {
        [SetUp]
        public void Setup() {
            SecurityTestHelper.CurrentDatabaseProviderType = SecurityTestHelper.DatabaseProviderType.LOCALDB_2012;
        }
    }

}
