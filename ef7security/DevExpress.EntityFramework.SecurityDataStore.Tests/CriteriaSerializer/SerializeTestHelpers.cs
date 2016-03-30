using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests {
    internal class SerializeTestHelper {
        static internal XElement CreateBaseCriteriaXElementWithIntObj() {
            return CreateBaseCriteriaXElement("System.Int32");
        }
        static internal XElement CreateBaseCriteriaXElement(string objectType, string dbContextType = "DevExpress.EntityFramework.SecurityDataStore.SecurityDbContext") {
            bool genericType = objectType.Contains("|");
            XElement objectTypeElement;
            if(genericType) {
                objectTypeElement = GetTypeElement(objectType, false);
            }
            else {
                objectTypeElement = new XElement("Type",
                            new XAttribute("Name", objectType)
                        );
            }

            bool isGenericType = objectType.Contains("|");
            string realObjectType = isGenericType ? objectType.Split('|')[1] : objectType;
            return new XElement("LambdaExpression",
                new XAttribute("NodeType", "Lambda"),
                new XAttribute("Name", ""),
                new XAttribute("TailCall", "false"),
                new XAttribute("CanReduce", "false"),

                new XElement("Type",
                    new XElement("Type",
                        new XAttribute("Name", "System.Func`3"),
                        new XElement("Type",
                            new XAttribute("Name", dbContextType)
                        ),
                        objectTypeElement,
                        new XElement("Type",
                            new XAttribute("Name", "System.Boolean")
                        )
                    )
                ),
                new XElement("Parameters",
                    GetParameterExpression(dbContextType, "db"),
                    GetParameterExpression(objectType, "obj")
                ),
                new XElement("Body"),
                new XElement("ReturnType",
                    new XElement("Type",
                        new XAttribute("Name", "System.Boolean")
                    )
                )
            );
        }
        static internal XElement GetTypeElement(string typeName, bool generateWrapperForGeneric = true) {
            bool genericType = typeName.Contains("|");
            if(genericType) {
                string[] types = typeName.Split('|');
                string genericTypeName = types[0];
                string genericArgTypeName = types[1];
                var element =  new XElement("Type",
                            new XAttribute("Name", genericTypeName),
                            new XElement("Type",
                                new XAttribute("Name", genericArgTypeName)
                            )
                        );
                if(generateWrapperForGeneric)
                    element = new XElement("Type", element);
                return element;
            } 
            else { 
                return new XElement("Type",
                            new XElement("Type",
                                new XAttribute("Name", typeName)
                            )
                        );
            }
        }
        static internal XElement GetParameterExpression(string typeName, string name) {
            return new XElement("ParameterExpression",
                        new XAttribute("NodeType", "Parameter"),
                        new XAttribute("Name", name),
                        new XAttribute("IsByRef", "false"),
                        new XAttribute("CanReduce", "false"),
                        GetTypeElement(typeName)
                    );
        }
        
        static internal XElement GetConstantExpression(string typeName, string value) {
            return new XElement("ConstantExpression",
                            new XAttribute("NodeType", "Constant"),
                            new XAttribute("CanReduce", "false"),
                            GetTypeElement(typeName),
                            new XElement("Value", value)
                       );
        }
        static internal XElement SetMethodElement(XElement element, XElement method) {
            element.Element("Method").ReplaceWith(method);
            return element;
        }
        static internal XElement SetLifted(XElement element, bool isLifted, bool isLiftedToNull) {
            element.Attribute("IsLifted").SetValue(isLifted);
            element.Attribute("IsLiftedToNull").SetValue(isLiftedToNull);
            return element;
        }
        static internal XElement GetBinaryExpression(string nodeType, XElement right, XElement left, string resultType = "System.Boolean") {
            return new XElement("BinaryExpression",
                        new XAttribute("NodeType", nodeType),
                        new XAttribute("CanReduce", "false"),
                        new XAttribute("IsLifted", "false"),
                        new XAttribute("IsLiftedToNull", "false"),
                        GetTypeElement(resultType),
                        new XElement("Right", right),
                        new XElement("Left", left),
                        new XElement("Method"),
                        new XElement("Conversion")
                    );
        }
        static internal XElement GetIntegerBinaryExpression(string nodeType, string value) {
            return GetBinaryExpression(nodeType,
                       GetConstantExpression("System.Int32", value),
                       GetParameterExpression("System.Int32", "obj"));
        }
        static internal XElement GetUnaryExpression(string nodeType, string resultType, XElement operand) {
            var expression = new XElement("UnaryExpression",
                        new XAttribute("NodeType", nodeType),
                        new XAttribute("IsLifted", "false"),
                        new XAttribute("IsLiftedToNull", "false"),
                        new XAttribute("CanReduce", "false"),
                        GetTypeElement(resultType),
                        new XElement("Operand",
                            operand
                        ),
                        new XElement("Method")
                    );
            return expression;
        }
        static internal XElement GetConstantConvertUnaryExpression(string convertType, string constantType, string constantValue) {
            return GetUnaryExpression("Convert", convertType, GetConstantExpression(constantType, constantValue));
        }
        static internal XElement GetParameterConvertUnaryExpression(string convertType, string parameterType, string parameterName) {
            return GetUnaryExpression("Convert", convertType, GetParameterExpression(parameterType, parameterName));
        }
        static internal XElement GetMemberConvertUnaryExpression(string convertType, string objectType, string objectName, string parameterType, string parameterName) {
            return GetUnaryExpression("Convert", convertType, GetMemberExpression(objectType, objectName, parameterType, parameterName));
        }
        static internal XElement GetMemberExpression(string objectType, string objectName, string propertyType, string propertyName) {
            return new XElement("MemberExpression",
                        new XAttribute("NodeType", "MemberAccess"),
                        new XAttribute("CanReduce", "false"),
                        GetTypeElement(propertyType),
                        new XElement("Member",
                            new XAttribute("MemberType", "Property"),
                            new XAttribute("PropertyName", propertyName),
                            new XElement("DeclaringType",
                                new XElement("Type",
                                   new XAttribute("Name", objectType)
                                )
                            )
                        ),
                        new XElement("Expression",
                            GetParameterExpression(objectType, objectName)
                        )
                    );
        }
        static internal XElement GetSingleParameterMethod(string methodName, string objectType, string parameterType, string[] genericArgTypes = null) {
            return GetMethod(methodName, objectType, new string[] { parameterType }, genericArgTypes);
        }
        static internal XElement GetMethod(string methodName, string objectType, string[] parametersType, string[] genericArgTypes = null) {
            if(genericArgTypes == null)
                genericArgTypes = new string[] { };
            return new XElement("Method",
                        new XAttribute("MemberType", "Method"),
                        new XAttribute("MethodName", methodName),
                        new XElement("DeclaringType",
                            new XElement("Type",
                                new XAttribute("Name", objectType)
                            )
                        ),
                        new XElement("Parameters",
                            from parameterType in parametersType
                            select GetTypeElement(parameterType)
                        ),
                        new XElement("GenericArgTypes",
                            from genericArgType in genericArgTypes
                            select GetTypeElement(genericArgType)
                        )
                    );
        }
        static internal void SetLambdaBody(XElement lambda, XElement body) {
            XElement bodyElement = lambda.Nodes().ElementAt(2) as XElement;
            bodyElement.Add(body);
        }
        static internal void CheckIfNominalAndSerializedAreEqual(XElement nominal, XElement serialized) {
            Assert.AreEqual(nominal.Nodes().Count(), serialized.Nodes().Count());

            for(int i = 0; i < nominal.Nodes().Count(); i++) {
                Assert.AreEqual(nominal.Nodes().ElementAt(i).ToString(), serialized.Nodes().ElementAt(i).ToString());
            }
        }
        static internal void CheckIfExpressionsAreEqual(Expression nominal, Expression deserialized) {
            string nominalString = nominal.ToString();
            string deserializedString = deserialized.ToString();

            // Console.WriteLine("nom {0}", nominalString);
            // Console.WriteLine("des {0}", deserialized);

            Assert.AreEqual(nominalString, nominalString);
        }
    }
}
