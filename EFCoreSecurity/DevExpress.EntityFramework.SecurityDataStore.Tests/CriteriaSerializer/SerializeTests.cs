using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Xml.Linq;
using NUnit.Framework;
using DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.Serializer {
    [TestFixture]
    public class SerializeTests {
        [Test]
        public void ParameterExpressionSerializeTest() {
            Expression criteria = Expression.Parameter(typeof(int), "intParam");
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();

            XElement serialized = criteriaSerializer.SerializeAsXElement(criteria);
            XElement nominal = SerializeTestHelper.GetParameterExpression("System.Int32", "intParam");

            SerializeTestHelper.CheckIfNominalAndSerializedAreEqual(nominal, serialized);
        }
        [Test]
        public void ReturnTrueSerializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => true;
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            XElement serialized = criteriaSerializer.SerializeAsXElement(criteria);
            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();

            XElement body = SerializeTestHelper.GetConstantExpression("System.Boolean", "True");

            SerializeTestHelper.SetLambdaBody(nominal, body);
            SerializeTestHelper.CheckIfNominalAndSerializedAreEqual(nominal, serialized);
        }
        [Test]
        public void ReturnFalseSerializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => false;
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            XElement serialized = criteriaSerializer.SerializeAsXElement(criteria);
            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();

            XElement body = SerializeTestHelper.GetConstantExpression("System.Boolean", "False");

            SerializeTestHelper.SetLambdaBody(nominal, body);
            SerializeTestHelper.CheckIfNominalAndSerializedAreEqual(nominal, serialized);
        }        
        [Test]
        public void LessThanConstantSerializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => obj < 5;
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            XElement serialized = criteriaSerializer.SerializeAsXElement(criteria);
            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();

            XElement body = SerializeTestHelper.GetIntegerBinaryExpression("LessThan", "5");

            SerializeTestHelper.SetLambdaBody(nominal, body);
            SerializeTestHelper.CheckIfNominalAndSerializedAreEqual(nominal, serialized);
        }
        [Test]
        public void LessThanOrEqualConstantSerializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => obj <= 5;
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            XElement serialized = criteriaSerializer.SerializeAsXElement(criteria);
            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();

            XElement body = SerializeTestHelper.GetIntegerBinaryExpression("LessThanOrEqual", "5");

            SerializeTestHelper.SetLambdaBody(nominal, body);
            SerializeTestHelper.CheckIfNominalAndSerializedAreEqual(nominal, serialized);
        }
        [Test]
        public void GreaterThanConstantSerializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => obj > 5;
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            XElement serialized = criteriaSerializer.SerializeAsXElement(criteria);
            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();

            XElement body = SerializeTestHelper.GetIntegerBinaryExpression("GreaterThan", "5");

            SerializeTestHelper.SetLambdaBody(nominal, body);
            SerializeTestHelper.CheckIfNominalAndSerializedAreEqual(nominal, serialized);
        }
        [Test]
        public void GreaterThanOrEqualConstantSerializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => obj >= 5;
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            XElement serialized = criteriaSerializer.SerializeAsXElement(criteria);
            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();

            XElement body = SerializeTestHelper.GetIntegerBinaryExpression("GreaterThanOrEqual", "5");

            SerializeTestHelper.SetLambdaBody(nominal, body);
            SerializeTestHelper.CheckIfNominalAndSerializedAreEqual(nominal, serialized);
        }
        [Test]
        public void EqualsConstantSerializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => obj == 5;
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            XElement serialized = criteriaSerializer.SerializeAsXElement(criteria);
            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();

            XElement body = SerializeTestHelper.GetIntegerBinaryExpression("Equal", "5");

            SerializeTestHelper.SetLambdaBody(nominal, body);
            SerializeTestHelper.CheckIfNominalAndSerializedAreEqual(nominal, serialized);
        }
        [Test]
        public void NotEqualConstantSerializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => obj != 5;
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            XElement serialized = criteriaSerializer.SerializeAsXElement(criteria);
            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();

            XElement body = SerializeTestHelper.GetIntegerBinaryExpression("NotEqual", "5");

            SerializeTestHelper.SetLambdaBody(nominal, body);
            SerializeTestHelper.CheckIfNominalAndSerializedAreEqual(nominal, serialized);
        }
        [Test]
        public void ArithmeticPlusSerializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => obj + 1 < 5;
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            XElement serialized = criteriaSerializer.SerializeAsXElement(criteria);
            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();

            XElement left =  SerializeTestHelper.GetBinaryExpression("Add",
                                SerializeTestHelper.GetConstantExpression("System.Int32", "1"),
                                SerializeTestHelper.GetParameterExpression("System.Int32", "obj"),
                                "System.Int32");
            XElement right = SerializeTestHelper.GetConstantExpression("System.Int32", "5");
            XElement body = SerializeTestHelper.GetBinaryExpression("LessThan", right, left);

            SerializeTestHelper.SetLambdaBody(nominal, body);
            SerializeTestHelper.CheckIfNominalAndSerializedAreEqual(nominal, serialized);
        }        
        [Test]
        public void ArithmeticMinusSerializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => obj - 1 < 5;
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            XElement serialized = criteriaSerializer.SerializeAsXElement(criteria);
            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();

            XElement left = SerializeTestHelper.GetBinaryExpression("Subtract",
                                SerializeTestHelper.GetConstantExpression("System.Int32", "1"),
                                SerializeTestHelper.GetParameterExpression("System.Int32", "obj"),
                                "System.Int32");
            XElement right = SerializeTestHelper.GetConstantExpression("System.Int32", "5");
            XElement body = SerializeTestHelper.GetBinaryExpression("LessThan", right, left);

            SerializeTestHelper.SetLambdaBody(nominal, body);
            SerializeTestHelper.CheckIfNominalAndSerializedAreEqual(nominal, serialized);
        }
        [Test]
        public void ArithmeticMultiplySerializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => obj * 2 < 5;
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            XElement serialized = criteriaSerializer.SerializeAsXElement(criteria);
            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();

            XElement left = SerializeTestHelper.GetBinaryExpression("Multiply",
                                SerializeTestHelper.GetConstantExpression("System.Int32", "2"),
                                SerializeTestHelper.GetParameterExpression("System.Int32", "obj"),
                                "System.Int32");
            XElement right = SerializeTestHelper.GetConstantExpression("System.Int32", "5");
            XElement body = SerializeTestHelper.GetBinaryExpression("LessThan", right, left);

            SerializeTestHelper.SetLambdaBody(nominal, body);
            SerializeTestHelper.CheckIfNominalAndSerializedAreEqual(nominal, serialized);
        }
        [Test]
        public void ArithmeticDivideSerializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => obj / 2 < 5;
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            XElement serialized = criteriaSerializer.SerializeAsXElement(criteria);
            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();

            XElement left = SerializeTestHelper.GetBinaryExpression("Divide",
                                SerializeTestHelper.GetConstantExpression("System.Int32", "2"),
                                SerializeTestHelper.GetParameterExpression("System.Int32", "obj"),
                                "System.Int32");
            XElement right = SerializeTestHelper.GetConstantExpression("System.Int32", "5");
            XElement body = SerializeTestHelper.GetBinaryExpression("LessThan", right, left);

            SerializeTestHelper.SetLambdaBody(nominal, body);
            SerializeTestHelper.CheckIfNominalAndSerializedAreEqual(nominal, serialized);
        }
        [Test]
        public void ArithmeticModuloSerializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => obj % 2 < 5;
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            XElement serialized = criteriaSerializer.SerializeAsXElement(criteria);
            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();

            XElement left = SerializeTestHelper.GetBinaryExpression("Modulo",
                                SerializeTestHelper.GetConstantExpression("System.Int32", "2"),
                                SerializeTestHelper.GetParameterExpression("System.Int32", "obj"),
                                "System.Int32");
            XElement right = SerializeTestHelper.GetConstantExpression("System.Int32", "5");
            XElement body = SerializeTestHelper.GetBinaryExpression("LessThan", right, left);

            SerializeTestHelper.SetLambdaBody(nominal, body);
            SerializeTestHelper.CheckIfNominalAndSerializedAreEqual(nominal, serialized);
        }
        [Test]
        public void ArithmeticAndSerializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => (obj & 2) < 5;
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            XElement serialized = criteriaSerializer.SerializeAsXElement(criteria);
            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();

            XElement left = SerializeTestHelper.GetBinaryExpression("And",
                                SerializeTestHelper.GetConstantExpression("System.Int32", "2"),
                                SerializeTestHelper.GetParameterExpression("System.Int32", "obj"),
                                "System.Int32");
            XElement right = SerializeTestHelper.GetConstantExpression("System.Int32", "5");
            XElement body = SerializeTestHelper.GetBinaryExpression("LessThan", right, left);

            SerializeTestHelper.SetLambdaBody(nominal, body);
            SerializeTestHelper.CheckIfNominalAndSerializedAreEqual(nominal, serialized);
        }
        [Test]
        public void ArithmeticOrSerializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => (obj | 2) < 5;
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            XElement serialized = criteriaSerializer.SerializeAsXElement(criteria);
            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();

            XElement left = SerializeTestHelper.GetBinaryExpression("Or",
                                SerializeTestHelper.GetConstantExpression("System.Int32", "2"),
                                SerializeTestHelper.GetParameterExpression("System.Int32", "obj"),
                                "System.Int32");
            XElement right = SerializeTestHelper.GetConstantExpression("System.Int32", "5");
            XElement body = SerializeTestHelper.GetBinaryExpression("LessThan", right, left);

            SerializeTestHelper.SetLambdaBody(nominal, body);
            SerializeTestHelper.CheckIfNominalAndSerializedAreEqual(nominal, serialized);
        }
        [Test]
        public void ArithmeticXorSerializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => (obj ^ 2) < 5;
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            XElement serialized = criteriaSerializer.SerializeAsXElement(criteria);
            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();

            XElement left = SerializeTestHelper.GetBinaryExpression("ExclusiveOr",
                                SerializeTestHelper.GetConstantExpression("System.Int32", "2"),
                                SerializeTestHelper.GetParameterExpression("System.Int32", "obj"),
                                "System.Int32");
            XElement right = SerializeTestHelper.GetConstantExpression("System.Int32", "5");
            XElement body = SerializeTestHelper.GetBinaryExpression("LessThan", right, left);

            SerializeTestHelper.SetLambdaBody(nominal, body);
            SerializeTestHelper.CheckIfNominalAndSerializedAreEqual(nominal, serialized);
        }
        [Test]
        public void LiftedArithmeticPlusSerializeTest() {
            Expression<Func<SecurityDbContext, int?, bool>> criteria = (db, obj) => obj + 1 < 5;
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            XElement serialized = criteriaSerializer.SerializeAsXElement(criteria);

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
            SerializeTestHelper.CheckIfNominalAndSerializedAreEqual(nominal, serialized);
        }
        [Test]
        public void ConvertToDoubleSerializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => obj / 2.0 < 5.2;
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            XElement serialized = criteriaSerializer.SerializeAsXElement(criteria);
            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();

            XElement left = SerializeTestHelper.GetBinaryExpression("Divide",
                                SerializeTestHelper.GetConstantExpression("System.Double", "2"),
                                SerializeTestHelper.GetParameterConvertUnaryExpression("System.Double", "System.Int32", "obj"),
                                "System.Double");
            XElement right = SerializeTestHelper.GetConstantExpression("System.Double", "5.2");
            XElement body = SerializeTestHelper.GetBinaryExpression("LessThan", right, left);

            SerializeTestHelper.SetLambdaBody(nominal, body);
            SerializeTestHelper.CheckIfNominalAndSerializedAreEqual(nominal, serialized);
        }
        [Test]
        public void LogicalNotSerializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => !(obj < 5);
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            XElement serialized = criteriaSerializer.SerializeAsXElement(criteria);
            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();

            XElement body = SerializeTestHelper.GetUnaryExpression("Not", "System.Boolean", SerializeTestHelper.GetIntegerBinaryExpression("LessThan", "5"));

            SerializeTestHelper.SetLambdaBody(nominal, body);
            SerializeTestHelper.CheckIfNominalAndSerializedAreEqual(nominal, serialized);
        }
        [Test]
        public void LogicalAndSerializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => obj < 5 && obj > 1;
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            XElement serialized = criteriaSerializer.SerializeAsXElement(criteria);
            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();

            XElement left = SerializeTestHelper.GetIntegerBinaryExpression("LessThan", "5");
            XElement right = SerializeTestHelper.GetIntegerBinaryExpression("GreaterThan", "1");

            XElement body = SerializeTestHelper.GetBinaryExpression("AndAlso", right, left);

            SerializeTestHelper.SetLambdaBody(nominal, body);
            SerializeTestHelper.CheckIfNominalAndSerializedAreEqual(nominal, serialized);
        }
        [Test]
        public void LogicalOrSerializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => obj < 5 || obj > 1;
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            XElement serialized = criteriaSerializer.SerializeAsXElement(criteria);
            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();

            XElement left = SerializeTestHelper.GetIntegerBinaryExpression("LessThan", "5");
            XElement right = SerializeTestHelper.GetIntegerBinaryExpression("GreaterThan", "1");

            XElement body = SerializeTestHelper.GetBinaryExpression("OrElse", right, left);

            SerializeTestHelper.SetLambdaBody(nominal, body);
            SerializeTestHelper.CheckIfNominalAndSerializedAreEqual(nominal, serialized);
        }
        public int getSomeValue() {
            return 5;
        }
        [Test, Ignore("todo?")]
        public void LessThanFunctionCallSerializeTest() {
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
        public void NotEqualNullSerializeTest() {
            Expression<Func<SecurityDbContext, DbContextObject1, bool>> criteria = (db, obj) => obj != null;

            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            XElement serialized = criteriaSerializer.SerializeAsXElement(criteria);

            string objectType = "DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts.DbContextObject1";

            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElement(objectType);

            XElement body = SerializeTestHelper.GetBinaryExpression("NotEqual",
                       SerializeTestHelper.GetConstantExpression("System.Object", ""),
                       SerializeTestHelper.GetParameterExpression(objectType, "obj"));

            SerializeTestHelper.SetLambdaBody(nominal, body);
            SerializeTestHelper.CheckIfNominalAndSerializedAreEqual(nominal, serialized);
        }
        [Test]
        public void ObjectNullablleIsNotNullSerializeTest() {
            Expression<Func<SecurityDbContext, DbContextObject1, bool>> criteria = (db, obj) => obj.ItemCountNull != null;

            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            XElement serialized = criteriaSerializer.SerializeAsXElement(criteria);

            string objectType = "DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts.DbContextObject1";
            string propertyType = "System.Nullable`1|System.Int32";
            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElement(objectType);

            XElement left = SerializeTestHelper.GetMemberExpression(objectType, "obj", propertyType, "ItemCountNull");
            XElement right = SerializeTestHelper.GetConstantExpression(propertyType, "");

            XElement body = SerializeTestHelper.GetBinaryExpression("NotEqual", right, left);
            SerializeTestHelper.SetLifted(body, true, false);

            SerializeTestHelper.SetLambdaBody(nominal, body);
            SerializeTestHelper.CheckIfNominalAndSerializedAreEqual(nominal, serialized);
        }
        [Test]
        public void ObjectPropertyLessThanConstantSerializeTest() {
            Expression<Func<SecurityDbContext, DbContextObject1, bool>> criteria = (db, obj) => obj.ItemCount < 3;
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            XElement serialized = criteriaSerializer.SerializeAsXElement(criteria);

            string propertyType = "System.Int32";
            string objectType = "DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts.DbContextObject1";

            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElement(objectType);

            XElement body = SerializeTestHelper.GetBinaryExpression("LessThan",
                       SerializeTestHelper.GetConstantExpression("System.Int32", "3"),
                       SerializeTestHelper.GetMemberExpression(objectType, "obj", propertyType, "ItemCount"));

            SerializeTestHelper.SetLambdaBody(nominal, body);
            SerializeTestHelper.CheckIfNominalAndSerializedAreEqual(nominal, serialized);
        }
        [Test]
        public void ObjectPropertyMethodSerializeTest() {
            Expression<Func<SecurityDbContext, DbContextObject1, bool>> criteria = (db, obj) => obj.Description.StartsWith("hello");
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            XElement serialized = criteriaSerializer.SerializeAsXElement(criteria);

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
            SerializeTestHelper.CheckIfNominalAndSerializedAreEqual(nominal, serialized);
        }
        [Test]
        public void TwoPropertiesWithImplicitConversionSerializeTest() {
            Expression<Func<SecurityDbContext, DbContextObject1, bool>> criteria = (db, obj) => obj.ItemCount < obj.DecimalItem;
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            XElement serialized = criteriaSerializer.SerializeAsXElement(criteria);

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
            SerializeTestHelper.CheckIfNominalAndSerializedAreEqual(nominal, serialized);
        }
        [Test]
        public void ComplexSerializeTest() {
            Expression<Func<DbContextMultiClass, DbContextObject1, bool>> criteria = (db, obj) => obj.ItemCount < db.dbContextDbSet1.Count();
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            XElement serialized = criteriaSerializer.SerializeAsXElement(criteria);

            string propertyType = "System.Int32";
            string objectType = "DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts.DbContextObject1";
            string dbContextType = "DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts.DbContextMultiClass";
            string methodCallResultType = "System.Int32";
            string methodObjectType = "System.Linq.Queryable";
            string methodParameterType = "System.Linq.IQueryable`1|DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts.DbContextObject1";
            string dbSetMemberType = "Microsoft.EntityFrameworkCore.DbSet`1|DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts.DbContextObject1";
            string dbSetPropertyName = "dbContextDbSet1";
            string dbSetType = "DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts.DbContextMultiClass";

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
            SerializeTestHelper.CheckIfNominalAndSerializedAreEqual(nominal, serialized);
        }        
        [Test]
        public void SerializeToStringTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => obj < 5;
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            string serializedString = criteriaSerializer.Serialize(criteria);
            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();

            XElement body = SerializeTestHelper.GetIntegerBinaryExpression("LessThan", "5");

            SerializeTestHelper.SetLambdaBody(nominal, body);

            Assert.AreEqual(nominal.ToString(), serializedString);
        }
        [Test]
        public void BadExpressionExpressionTypeSerializeTest() {
            Expression criteria = Expression.Constant(10, typeof(int));
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();

            bool withArgumentException = false;
            try {
                XElement serialized = criteriaSerializer.SerializeAsXElement(criteria);
            }
            catch(ArgumentException) {
                withArgumentException = true;
            }
            catch(Exception e) {
                Assert.Fail(e.Message);
            }
            Assert.IsTrue(withArgumentException);
        }
        [Test]
        public void BadExpressionExpressionType2SerializeTest() {
            Expression<Func<SecurityDbContext, int, int>> criteria = (db, obj) => obj + 5;
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();

            bool withArgumentException = false;
            try {
                XElement serialized = criteriaSerializer.SerializeAsXElement(criteria.Body);
            }
            catch(ArgumentException) {
                withArgumentException = true;
            }
            catch(Exception e) {
                Assert.Fail(e.Message);
            }
            Assert.IsTrue(withArgumentException);
        }
        [Test]
        public void BadExpressionIntResultSerializeTest() {
            Expression<Func<SecurityDbContext, int, int>> criteria = (db, obj) => obj + 5;
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            
            bool withArgumentException = false;
            try {
                XElement serialized = criteriaSerializer.SerializeAsXElement(criteria);
            }
            catch(ArgumentException) {
                withArgumentException = true;
            }
            catch(Exception e) {
                Assert.Fail(e.Message);
            }
            Assert.IsTrue(withArgumentException);
        }
        [Test]
        public void BadExpressionParametersCountSerializeTest() {
            Expression<Func<SecurityDbContext, int, int, bool>> criteria = (db, obj, obj2) => obj < obj2;
            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();

            bool withArgumentException = false;
            try {
                XElement serialized = criteriaSerializer.SerializeAsXElement(criteria);
            }
            catch(ArgumentException) {
                withArgumentException = true;
            }
            catch(Exception e) {
                Assert.Fail(e.Message);
            }
            Assert.IsTrue(withArgumentException);
        }
        [Test]
        public void MultipleSerializeTest() {
            Expression<Func<SecurityDbContext, int, bool>> criteria = (db, obj) => obj < 5;
            XElement nominal = SerializeTestHelper.CreateBaseCriteriaXElementWithIntObj();

            XElement body = SerializeTestHelper.GetIntegerBinaryExpression("LessThan", "5");

            SerializeTestHelper.SetLambdaBody(nominal, body);

            CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
            XElement serialized = criteriaSerializer.SerializeAsXElement(criteria);
            SerializeTestHelper.CheckIfNominalAndSerializedAreEqual(nominal, serialized);

            XElement serialized2 = criteriaSerializer.SerializeAsXElement(criteria);
            SerializeTestHelper.CheckIfNominalAndSerializedAreEqual(nominal, serialized2);
        }
    }
}
