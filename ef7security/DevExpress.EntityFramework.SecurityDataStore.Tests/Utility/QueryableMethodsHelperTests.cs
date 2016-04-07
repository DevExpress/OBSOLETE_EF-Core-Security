using DevExpress.EntityFramework.SecurityDataStore.Utility;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
