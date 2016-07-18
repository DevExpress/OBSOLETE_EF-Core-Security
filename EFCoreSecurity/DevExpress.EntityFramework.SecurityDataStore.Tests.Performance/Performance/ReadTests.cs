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
    public abstract class ReadTests : BasePerformanceTestClass{
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

                    Stopwatch watch = new Stopwatch();
                    watch.Start();

                    List<DbContextObject1> objects = contextInterface.dbContextDbSet1.Select(obj => obj).ToList();
                    Assert.AreEqual(count, objects.Count);

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
    public class InMemoryReadTests : ReadTests {
        [SetUp]
        public void Setup() {
            SecurityTestHelper.CurrentDatabaseProviderType = SecurityTestHelper.DatabaseProviderType.IN_MEMORY;

            SetTimeDifference(TestType.WithoutPermissions, 15);
            SetTimeDifference(TestType.WithOnePermission, 15);
            SetTimeDifference(TestType.WithMultiplePermissions, 25);
        }
    }

    [TestFixture]
    public class LocalDb2012ReadTests : ReadTests {
        [SetUp]
        public void Setup() {
            SecurityTestHelper.CurrentDatabaseProviderType = SecurityTestHelper.DatabaseProviderType.LOCALDB_2012;

            SetTimeDifference(TestType.WithoutPermissions, 10);
            SetTimeDifference(TestType.WithOnePermission, 30);
            SetTimeDifference(TestType.WithMultiplePermissions, 60);
        }
    }
}
