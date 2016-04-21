using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Xml.Linq;
using NUnit.Framework;
using DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts;
using System.Reflection;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests {
    [TestFixture]
    public class DeserializeTests {
        [Test]
        public void CheckCriteriaObjectIsNullTest() {
            Expression<Func<SecurityDbContext, Person, bool>> criteria = (db, obj) => obj != null;

            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            var serialize = criteriaSerializer.Serialize(criteria);

            Expression deserializedCriteria = criteriaSerializer.Deserialize(serialize);
            SerializeTestHelper.CheckIfExpressionsAreEqual(criteria, deserializedCriteria);
        }
        [Test]
        public void ReturnTrueDeserializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => true;
            
            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();
            XElement body = SerializeTestHelper.GetConstantExpression("System.Boolean", "True");

            SerializeTestHelper.SetLambdaBody(nominal, body);

            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            Expression deserializedCriteria = criteriaSerializer.Deserialize(nominal);
            SerializeTestHelper.CheckIfExpressionsAreEqual(criteria, deserializedCriteria);
        }
        [Test]
        public void ReturnFalseDeserializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => false;

            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();
            XElement body = SerializeTestHelper.GetConstantExpression("System.Boolean", "False");

            SerializeTestHelper.SetLambdaBody(nominal, body);

            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            Expression deserializedCriteria = criteriaSerializer.Deserialize(nominal);
            SerializeTestHelper.CheckIfExpressionsAreEqual(criteria, deserializedCriteria);
        }
        [Test]
        public void LessThanConstantDeserializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => obj < 5;
            
            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();
            XElement body = SerializeTestHelper.GetIntegerBinaryExpression("LessThan", "5");

            SerializeTestHelper.SetLambdaBody(nominal, body);

            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            Expression deserializedCriteria = criteriaSerializer.Deserialize(nominal);
            SerializeTestHelper.CheckIfExpressionsAreEqual(criteria, deserializedCriteria);
        }

        [Test]
        public void LessThanOrEqualConstantDeerializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => obj <= 5;

            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();
            XElement body = SerializeTestHelper.GetIntegerBinaryExpression("LessThanOrEqual", "5");

            SerializeTestHelper.SetLambdaBody(nominal, body);

            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            Expression deserializedCriteria = criteriaSerializer.Deserialize(nominal);
            SerializeTestHelper.CheckIfExpressionsAreEqual(criteria, deserializedCriteria);
        }
        [Test]
        public void GreaterThanConstantDeserializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => obj > 5;

            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();
            XElement body = SerializeTestHelper.GetIntegerBinaryExpression("GreaterThan", "5");

            SerializeTestHelper.SetLambdaBody(nominal, body);

            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            Expression deserializedCriteria = criteriaSerializer.Deserialize(nominal);
            SerializeTestHelper.CheckIfExpressionsAreEqual(criteria, deserializedCriteria);
        }
        [Test]
        public void GreaterThanOrEqualConstantDeserializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => obj >= 5;

            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();
            XElement body = SerializeTestHelper.GetIntegerBinaryExpression("GreaterThanOrEqual", "5");

            SerializeTestHelper.SetLambdaBody(nominal, body);

            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            Expression deserializedCriteria = criteriaSerializer.Deserialize(nominal);
            SerializeTestHelper.CheckIfExpressionsAreEqual(criteria, deserializedCriteria);
        }
        [Test]
        public void EqualsConstantDeserializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => obj == 5;

            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();
            XElement body = SerializeTestHelper.GetIntegerBinaryExpression("Equal", "5");

            SerializeTestHelper.SetLambdaBody(nominal, body);

            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            Expression deserializedCriteria = criteriaSerializer.Deserialize(nominal);
            SerializeTestHelper.CheckIfExpressionsAreEqual(criteria, deserializedCriteria);
        }
        [Test]
        public void NotEqualConstantDeserializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => obj != 5;

            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();
            XElement body = SerializeTestHelper.GetIntegerBinaryExpression("NotEqual", "5");

            SerializeTestHelper.SetLambdaBody(nominal, body);

            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            Expression deserializedCriteria = criteriaSerializer.Deserialize(nominal);
            SerializeTestHelper.CheckIfExpressionsAreEqual(criteria, deserializedCriteria);
        }
        [Test]
        public void ArithmeticPlusDeserializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => obj + 1 < 5;

            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();

            XElement left = SerializeTestHelper.GetBinaryExpression("Add",
                                SerializeTestHelper.GetConstantExpression("System.Int32", "1"),
                                SerializeTestHelper.GetParameterExpression("System.Int32", "obj"),
                                "System.Int32");
            XElement right = SerializeTestHelper.GetConstantExpression("System.Int32", "5");
            XElement body = SerializeTestHelper.GetBinaryExpression("LessThan", right, left);

            SerializeTestHelper.SetLambdaBody(nominal, body);

            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            Expression deserializedCriteria = criteriaSerializer.Deserialize(nominal);
            SerializeTestHelper.CheckIfExpressionsAreEqual(criteria, deserializedCriteria);
        }
        [Test]
        public void ArithmeticMinusDeserializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => obj - 1 < 5;

            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();
            XElement left = SerializeTestHelper.GetBinaryExpression("Subtract",
                                SerializeTestHelper.GetConstantExpression("System.Int32", "1"),
                                SerializeTestHelper.GetParameterExpression("System.Int32", "obj"),
                                "System.Int32");
            XElement right = SerializeTestHelper.GetConstantExpression("System.Int32", "5");
            XElement body = SerializeTestHelper.GetBinaryExpression("LessThan", right, left);

            SerializeTestHelper.SetLambdaBody(nominal, body);

            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            Expression deserializedCriteria = criteriaSerializer.Deserialize(nominal);
            SerializeTestHelper.CheckIfExpressionsAreEqual(criteria, deserializedCriteria);
        }
        [Test]
        public void ArithmeticMultiplyDeserializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => obj * 2 < 5;

            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();
            XElement left = SerializeTestHelper.GetBinaryExpression("Multiply",
                                SerializeTestHelper.GetConstantExpression("System.Int32", "2"),
                                SerializeTestHelper.GetParameterExpression("System.Int32", "obj"),
                                "System.Int32");
            XElement right = SerializeTestHelper.GetConstantExpression("System.Int32", "5");
            XElement body = SerializeTestHelper.GetBinaryExpression("LessThan", right, left);

            SerializeTestHelper.SetLambdaBody(nominal, body);

            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            Expression deserializedCriteria = criteriaSerializer.Deserialize(nominal);
            SerializeTestHelper.CheckIfExpressionsAreEqual(criteria, deserializedCriteria);
        }
        [Test]
        public void ArithmeticDivideDeserializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => obj / 2 < 5;

            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();
            XElement left = SerializeTestHelper.GetBinaryExpression("Divide",
                                SerializeTestHelper.GetConstantExpression("System.Int32", "2"),
                                SerializeTestHelper.GetParameterExpression("System.Int32", "obj"),
                                "System.Int32");
            XElement right = SerializeTestHelper.GetConstantExpression("System.Int32", "5");
            XElement body = SerializeTestHelper.GetBinaryExpression("LessThan", right, left);

            SerializeTestHelper.SetLambdaBody(nominal, body);

            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            Expression deserializedCriteria = criteriaSerializer.Deserialize(nominal);
            SerializeTestHelper.CheckIfExpressionsAreEqual(criteria, deserializedCriteria);
        }
        [Test]
        public void ArithmeticModuloDeserializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => obj % 2 < 5;

            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();
            XElement left = SerializeTestHelper.GetBinaryExpression("Modulo",
                                SerializeTestHelper.GetConstantExpression("System.Int32", "2"),
                                SerializeTestHelper.GetParameterExpression("System.Int32", "obj"),
                                "System.Int32");
            XElement right = SerializeTestHelper.GetConstantExpression("System.Int32", "5");
            XElement body = SerializeTestHelper.GetBinaryExpression("LessThan", right, left);

            SerializeTestHelper.SetLambdaBody(nominal, body);

            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            Expression deserializedCriteria = criteriaSerializer.Deserialize(nominal);
            SerializeTestHelper.CheckIfExpressionsAreEqual(criteria, deserializedCriteria);
        }
        [Test]
        public void ArithmeticAndDeserializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => (obj & 2) < 5;

            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();
            XElement left = SerializeTestHelper.GetBinaryExpression("And",
                                SerializeTestHelper.GetConstantExpression("System.Int32", "2"),
                                SerializeTestHelper.GetParameterExpression("System.Int32", "obj"),
                                "System.Int32");
            XElement right = SerializeTestHelper.GetConstantExpression("System.Int32", "5");
            XElement body = SerializeTestHelper.GetBinaryExpression("LessThan", right, left);

            SerializeTestHelper.SetLambdaBody(nominal, body);

            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            Expression deserializedCriteria = criteriaSerializer.Deserialize(nominal);
            SerializeTestHelper.CheckIfExpressionsAreEqual(criteria, deserializedCriteria);
        }
        [Test]
        public void ArithmeticOrDeserializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => (obj | 2) < 5;

            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();
            XElement left = SerializeTestHelper.GetBinaryExpression("Or",
                                SerializeTestHelper.GetConstantExpression("System.Int32", "2"),
                                SerializeTestHelper.GetParameterExpression("System.Int32", "obj"),
                                "System.Int32");
            XElement right = SerializeTestHelper.GetConstantExpression("System.Int32", "5");
            XElement body = SerializeTestHelper.GetBinaryExpression("LessThan", right, left);

            SerializeTestHelper.SetLambdaBody(nominal, body);

            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            Expression deserializedCriteria = criteriaSerializer.Deserialize(nominal);
            SerializeTestHelper.CheckIfExpressionsAreEqual(criteria, deserializedCriteria);
        }
        [Test]
        public void ArithmeticXorDeserializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => (obj ^ 2) < 5;
            
            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();
            XElement left = SerializeTestHelper.GetBinaryExpression("ExclusiveOr",
                                SerializeTestHelper.GetConstantExpression("System.Int32", "2"),
                                SerializeTestHelper.GetParameterExpression("System.Int32", "obj"),
                                "System.Int32");
            XElement right = SerializeTestHelper.GetConstantExpression("System.Int32", "5");
            XElement body = SerializeTestHelper.GetBinaryExpression("LessThan", right, left);

            SerializeTestHelper.SetLambdaBody(nominal, body);

            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            Expression deserializedCriteria = criteriaSerializer.Deserialize(nominal);
            SerializeTestHelper.CheckIfExpressionsAreEqual(criteria, deserializedCriteria);
        }
        [Test]
        public void LiftedArithmeticPlusDeserializeTest() {
            Expression<Func<SecurityDbContext, int?, bool>> criteria = (db, obj) => obj + 1 < 5;

            string objectType = "System.Nullable`1|System.Int32";
            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElement(objectType);

            XElement left = SerializeTestHelper.GetBinaryExpression("Add",
                                SerializeTestHelper.SetLifted(SerializeTestHelper.GetConstantConvertUnaryExpression(objectType, "System.Int32", "1"), true, true),
                                SerializeTestHelper.GetParameterExpression(objectType, "obj"),
                                objectType);
            XElement right = SerializeTestHelper.GetConstantConvertUnaryExpression(objectType, "System.Int32", "5");

            SerializeTestHelper.SetLifted(left, true, true);
            SerializeTestHelper.SetLifted(right, true, true);

            XElement body = SerializeTestHelper.GetBinaryExpression("LessThan", right, left);

            SerializeTestHelper.SetLifted(body, true, false);

            SerializeTestHelper.SetLambdaBody(nominal, body);

            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            Expression deserializedCriteria = criteriaSerializer.Deserialize(nominal);
            SerializeTestHelper.CheckIfExpressionsAreEqual(criteria, deserializedCriteria);
        }
        [Test]
        public void ConvertToDoubleDeserializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => obj / 2.0 < 5.2;

            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();
            XElement left = SerializeTestHelper.GetBinaryExpression("Divide",
                                SerializeTestHelper.GetConstantExpression("System.Double", "2"),
                                SerializeTestHelper.GetParameterConvertUnaryExpression("System.Double", "System.Int32", "obj"),
                                "System.Double");
            XElement right = SerializeTestHelper.GetConstantExpression("System.Double", "5.2");
            XElement body = SerializeTestHelper.GetBinaryExpression("LessThan", right, left);

            SerializeTestHelper.SetLambdaBody(nominal, body);

            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            Expression deserializedCriteria = criteriaSerializer.Deserialize(nominal);
            SerializeTestHelper.CheckIfExpressionsAreEqual(criteria, deserializedCriteria);
        }
        [Test]
        public void LogicalNotDeserializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => !(obj < 5);

            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();
            XElement body = SerializeTestHelper.GetUnaryExpression("Not", "System.Boolean", SerializeTestHelper.GetIntegerBinaryExpression("LessThan", "5"));

            SerializeTestHelper.SetLambdaBody(nominal, body);

            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            Expression deserializedCriteria = criteriaSerializer.Deserialize(nominal);
            SerializeTestHelper.CheckIfExpressionsAreEqual(criteria, deserializedCriteria);
        }
        [Test]
        public void LogicalAndDeserializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => obj < 5 && obj > 1;

            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();
            XElement left = SerializeTestHelper.GetIntegerBinaryExpression("LessThan", "5");
            XElement right = SerializeTestHelper.GetIntegerBinaryExpression("GreaterThan", "1");

            XElement body = SerializeTestHelper.GetBinaryExpression("AndAlso", right, left);

            SerializeTestHelper.SetLambdaBody(nominal, body);

            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            Expression deserializedCriteria = criteriaSerializer.Deserialize(nominal);
            SerializeTestHelper.CheckIfExpressionsAreEqual(criteria, deserializedCriteria);
        }
        [Test]
        public void LogicalOrDeserializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => obj < 5 || obj > 1;

            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();
            XElement left = SerializeTestHelper.GetIntegerBinaryExpression("LessThan", "5");
            XElement right = SerializeTestHelper.GetIntegerBinaryExpression("GreaterThan", "1");

            XElement body = SerializeTestHelper.GetBinaryExpression("OrElse", right, left);

            SerializeTestHelper.SetLambdaBody(nominal, body);

            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            Expression deserializedCriteria = criteriaSerializer.Deserialize(nominal);
            SerializeTestHelper.CheckIfExpressionsAreEqual(criteria, deserializedCriteria);
        }

        public int getSomeValue() {
            return 5;
        }
        [Test, Ignore("todo?")]
        public void LessThanFunctionCallDeserializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => obj < getSomeValue();
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            XElement serialized = criteriaSerializer.SerializeAsXElement(criteria);
            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();

            XElement body = new XElement("BinaryExpression",
                        new XAttribute("NodeType", "LessThan"),
                        new XAttribute("CanReduce", "false"),
                        new XAttribute("IsLifted", "false"),
                        new XAttribute("IsLiftedToNull", "false"),
                        SerializeTestHelper.GetTypeElement("System.Boolean"),
                        new XElement("Right",
                            SerializeTestHelper.GetConstantExpression("System.Int32", "5")
                        ),
                        new XElement("Left",
                            SerializeTestHelper.GetParameterExpression("System.Int32", "obj")
                        ),
                        new XElement("Method"),
                        new XElement("Conversion")
                    );
            SerializeTestHelper.SetLambdaBody(nominal, body);
            SerializeTestHelper.CheckIfNominalAndSerializedAreEqual(nominal, serialized);
        }
        [Test]
        public void ObjectPropertyLessThanConstantDeserializeTest() {
            Expression<Func<SecurityDbContext, DbContextObject1, bool>> criteria = (db, obj) => obj.ItemCount < 3;

            string propertyType = "System.Int32";
            string objectType = "DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts.DbContextObject1";

            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElement(objectType);

            XElement body = SerializeTestHelper.GetBinaryExpression("LessThan",
                       SerializeTestHelper.GetConstantExpression("System.Int32", "3"),
                       SerializeTestHelper.GetMemberExpression(objectType, "obj", propertyType, "ItemCount"));

            SerializeTestHelper.SetLambdaBody(nominal, body);

            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            criteriaSerializer.RegisterAdditionalAssemblies(GetAdditionalAssemblies());
            Expression deserializedCriteria = criteriaSerializer.Deserialize(nominal);
            SerializeTestHelper.CheckIfExpressionsAreEqual(criteria, deserializedCriteria);
        }
        [Test]
        public void ObjectPropertyMethodDeserializeTest() {
            Expression<Func<SecurityDbContext, DbContextObject1, bool>> criteria = (db, obj) => obj.Description.StartsWith("hello");

            string propertyType = "System.String";
            string methodResultType = "System.Boolean";
            string methodObjectType = "System.String";
            string methodParameterType = "System.String";
            string objectType = "DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts.DbContextObject1";

            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElement(objectType);

            XElement body = new XElement("MethodCallExpression",
                        new XAttribute("NodeType", "Call"),
                        new XAttribute("CanReduce", "false"),
                        SerializeTestHelper.GetTypeElement(methodResultType),
                        SerializeTestHelper.GetSingleParameterMethod("StartsWith", methodObjectType, methodParameterType),
                        new XElement("Object",
                            SerializeTestHelper.GetMemberExpression(objectType, "obj", propertyType, "Description")
                        ),
                        new XElement("Arguments",
                            SerializeTestHelper.GetConstantExpression(methodParameterType, "hello")
                        )
                    );

            SerializeTestHelper.SetLambdaBody(nominal, body);

            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            criteriaSerializer.RegisterAdditionalAssemblies(GetAdditionalAssemblies());
            Expression deserializedCriteria = criteriaSerializer.Deserialize(nominal);
            SerializeTestHelper.CheckIfExpressionsAreEqual(criteria, deserializedCriteria);
        }
        [Test]
        public void TwoPropertiesWithImplicitConversionDeserializeTest() {
            Expression<Func<SecurityDbContext, DbContextObject1, bool>> criteria = (db, obj) => obj.ItemCount < obj.DecimalItem;

            string property1Type = "System.Int32";
            string property2Type = "System.Decimal";
            string property1Name = "ItemCount";
            string property2Name = "DecimalItem";
            string objectName = "obj";
            string objectType = "DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts.DbContextObject1";

            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElement(objectType);

            XElement implicitConversionMethod = SerializeTestHelper.GetSingleParameterMethod("op_Implicit", "System.Decimal", "System.Int32");
            XElement lessThanMethod = SerializeTestHelper.GetMethod("op_LessThan", "System.Decimal", new string[] { "System.Decimal", "System.Decimal" });

            XElement right = SerializeTestHelper.GetMemberExpression(objectType, objectName, property2Type, property2Name);
            XElement left = SerializeTestHelper.GetMemberConvertUnaryExpression(property2Type, objectType, objectName, property1Type, property1Name);

            SerializeTestHelper.SetMethodElement(left, implicitConversionMethod);

            XElement body = SerializeTestHelper.GetBinaryExpression("LessThan",
                right,
                left
            );

            SerializeTestHelper.SetMethodElement(body, lessThanMethod);

            SerializeTestHelper.SetLambdaBody(nominal, body);

            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            criteriaSerializer.RegisterAdditionalAssemblies(GetAdditionalAssemblies());
            Expression deserializedCriteria = criteriaSerializer.Deserialize(nominal);
            SerializeTestHelper.CheckIfExpressionsAreEqual(criteria, deserializedCriteria);
        }
        [Test]
        public void ComplexDeserializeTest() {
            Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria = (db, obj) => obj.ItemCount < db.dbContextDbSet1.Count();

            string propertyType = "System.Int32";
            string objectType = "DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts.DbContextObject1";
            string dbContextType = "DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts.DbContextMultiClass";
            string methodCallResultType = "System.Int32";
            string methodObjectType = "System.Linq.Queryable";
            string methodParameterType = "System.Linq.IQueryable`1|DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts.DbContextObject1";
            string dbSetMemberType = "Microsoft.EntityFrameworkCore.DbSet`1|DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts.DbContextObject1";
            string dbSetPropertyName = "dbContextDbSet1";
            string dbSetType = "DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts.DbContextDbSet";

            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElement(objectType, dbContextType);

            XElement memberExpression = new XElement("MemberExpression",
                        new XAttribute("NodeType", "MemberAccess"),
                        new XAttribute("CanReduce", "false"),
                        SerializeTestHelper.GetTypeElement(dbSetMemberType),
                        new XElement("Member",
                            new XAttribute("MemberType", "Property"),
                            new XAttribute("PropertyName", dbSetPropertyName),
                            new XElement("DeclaringType",
                                new XElement("Type",
                                   new XAttribute("Name", dbSetType)
                                )
                            )
                        ),
                        new XElement("Expression",
                            SerializeTestHelper.GetParameterExpression(dbContextType, "db")
                        )
                    );

            XElement right = new XElement("MethodCallExpression",
                        new XAttribute("NodeType", "Call"),
                        new XAttribute("CanReduce", "false"),
                        SerializeTestHelper.GetTypeElement(methodCallResultType),
                        SerializeTestHelper.GetSingleParameterMethod("Count", methodObjectType, methodParameterType, new string[] { objectType }),
                        new XElement("Object"),
                        new XElement("Arguments",
                            memberExpression
                        )
                    );

            XElement body = SerializeTestHelper.GetBinaryExpression("LessThan",
                       right,
                       SerializeTestHelper.GetMemberExpression(objectType, "obj", propertyType, "ItemCount"));

            SerializeTestHelper.SetLambdaBody(nominal, body);

            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            criteriaSerializer.RegisterAdditionalAssemblies(GetAdditionalAssemblies());
            Expression deserializedCriteria = criteriaSerializer.Deserialize(nominal);
            SerializeTestHelper.CheckIfExpressionsAreEqual(criteria, deserializedCriteria);
        }
        [Test]
        public void DeserializeFromStringTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => obj < 5;
            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();

            XElement body = SerializeTestHelper.GetIntegerBinaryExpression("LessThan", "5");

            SerializeTestHelper.SetLambdaBody(nominal, body);

            string nominalString = nominal.ToString();

            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            criteriaSerializer.RegisterAdditionalAssemblies(GetAdditionalAssemblies());
            Expression deserializedCriteria = criteriaSerializer.Deserialize(nominalString);
            SerializeTestHelper.CheckIfExpressionsAreEqual(criteria, deserializedCriteria);
        }
        [Test]
        public void MultipleDeserializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => obj < 5;
            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();

            XElement body = SerializeTestHelper.GetIntegerBinaryExpression("LessThan", "5");

            SerializeTestHelper.SetLambdaBody(nominal, body);

            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            criteriaSerializer.RegisterAdditionalAssemblies(GetAdditionalAssemblies());

            Expression deserializedCriteria = criteriaSerializer.Deserialize(nominal);
            SerializeTestHelper.CheckIfExpressionsAreEqual(criteria, deserializedCriteria);

            Expression<Func<SecurityDbContext, DbContextObject1, bool>> criteria2 = (db, obj) => obj.ItemCount < 3;

            string propertyType = "System.Int32";
            string objectType = "DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts.DbContextObject1";

            XElement nominal2 = SerializeTestHelper.CreateBaseCriteriaXElement(objectType);

            XElement body2 = SerializeTestHelper.GetBinaryExpression("LessThan",
                       SerializeTestHelper.GetConstantExpression("System.Int32", "3"),
                       SerializeTestHelper.GetMemberExpression(objectType, "obj", propertyType, "ItemCount"));

            SerializeTestHelper.SetLambdaBody(nominal2, body2);

            Expression deserializedCriteria2 = criteriaSerializer.Deserialize(nominal2);
            SerializeTestHelper.CheckIfExpressionsAreEqual(criteria2, deserializedCriteria2);

            SerializeTestHelper.CheckIfExpressionsAreEqual(criteria, deserializedCriteria);
        }
        [Test]
        public void BadXmlStringDeserializeTest() {
            string nominalString = "SomeBadString";

            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();

            bool withXmlException = false;
            try {
                Expression deserializedCriteria = criteriaSerializer.Deserialize(nominalString);
            }
            catch(System.Xml.XmlException) {
                withXmlException = true;
            }
            catch(Exception e) {
                Assert.Fail(e.Message);
            }
            Assert.IsTrue(withXmlException);
        }
        [Test]
        public void BadCriteriaDeserializeTest() {
            string nominalString = "<DeclaringType><Type Name=\"System.Linq.Queryable\" /></DeclaringType> ";

            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();

            bool withNotSupportedException = false;
            try {
                Expression deserializedCriteria = criteriaSerializer.Deserialize(nominalString);
            }
            catch(NotSupportedException) {
                withNotSupportedException = true;
            }
            catch(Exception e) {
                Assert.Fail(e.Message);
            }
            Assert.IsTrue(withNotSupportedException);
        }
        private IList<Assembly> GetAdditionalAssemblies() {
            return new List<Assembly> { typeof(DbContextObject1).Assembly };
        }
    }
}
