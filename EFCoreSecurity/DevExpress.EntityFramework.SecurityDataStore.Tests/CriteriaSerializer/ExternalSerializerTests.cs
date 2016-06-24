using System;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts;
using NUnit.Framework;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.ExternalSerializer {
    [TestFixture]
    public abstract class ExternalSerializerTests {
        public abstract ExternalCriteriaSerializer GetSerializer();      
        [Test]
        public void ParameterExpressionTest() {
            Expression criteria = Expression.Parameter(typeof(int), "intParam");
            Expression deserialized = SerializeAndGetDeserialized(criteria);
            Assert.AreEqual(criteria.ToString(), deserialized.ToString());
        }
        [Test]
        public void SimpleCriteriaTest() {
            Expression<Func<SecurityDbContext, DbContextObject1, bool>> criteria = (db, obj) => obj.ItemCount < 3;
            Expression deserialized = SerializeAndGetDeserialized(criteria);
            Assert.AreEqual(criteria.ToString(), deserialized.ToString());
        }
        [Test]
        public void ComplexCriteriaTest() {
            Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria = (db, obj) => obj.ItemCount < db.dbContextDbSet1.Count();
            Expression deserialized = SerializeAndGetDeserialized(criteria);
            Assert.AreEqual(criteria.ToString(), deserialized.ToString());
        }
        private Expression SerializeAndGetDeserialized(Expression criteria) {
            ExternalCriteriaSerializer criteriaSerializer = GetSerializer();
            string serialized = criteriaSerializer.Serialize(criteria);
            return criteriaSerializer.Deserialize(serialized);
        }
    }
    [TestFixture]
    public class XmlExternalSerializerTests : ExternalSerializerTests {
        public override ExternalCriteriaSerializer GetSerializer() {
            return new ExternalCriteriaSerializer(ExternalCriteriaSerializer.SerializerType.XmlSerializer);
        }
    }

    [TestFixture]
    public class JsonExternalSerializerTests : ExternalSerializerTests {
        public override ExternalCriteriaSerializer GetSerializer() {
            return new ExternalCriteriaSerializer(ExternalCriteriaSerializer.SerializerType.JsonSerializer);
        }
    }
}
