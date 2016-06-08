using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.EntityFramework.SecurityDataStore.Tests.Helpers;
using DevExpress.EntityFramework.SecurityDataStore.Utility;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.Utility {
    [TestFixture]
    public class QueryableMethodsHelperTests {
        [Test]
        public void SelectTest() {
            Assert.IsNotNull(QueryableMethodsHelper.Select);            
        }
        [Test]
        public void SelectManyTest() {
            Assert.IsNotNull(QueryableMethodsHelper.SelectMany);
        }
    }
    [TestFixture]
    public class InMemoryQueryableMethodsHelperTests : QueryableMethodsHelperTests {
        [SetUp]
        public void Setup() {
            SecurityTestHelper.CurrentDatabaseProviderType = SecurityTestHelper.DatabaseProviderType.IN_MEMORY;
        }
    }

    [TestFixture]
    public class LocalDb2012QueryableMethodsHelperTests : QueryableMethodsHelperTests {
        [SetUp]
        public void Setup() {
            SecurityTestHelper.CurrentDatabaseProviderType = SecurityTestHelper.DatabaseProviderType.LOCALDB_2012;
        }
    }
}
