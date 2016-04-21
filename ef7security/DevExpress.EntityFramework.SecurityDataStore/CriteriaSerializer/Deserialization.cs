using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Diagnostics;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public partial class CriteriaSerializer {
        private CriteriaSerializeHelper helper;
        private Dictionary<string, ParameterExpression> parameters = new Dictionary<string, ParameterExpression>();
        private CriteriaSerializeHelper Helper {
            get {
                if(helper == null)
                    helper = new CriteriaSerializeHelper();
                return helper;
            }
        }
        public void RegisterAdditionalAssemblies(IList<Assembly> assemblies) {
            Helper.RegisterAdditionalAssemblies(assemblies);
        }
        public Expression Deserialize(string serializedCriteria) {
            parameters.Clear();
            return ExpressionFromXml(XElement.Parse(serializedCriteria));
        }
        public Expression Deserialize(XElement serializedCriteria) {
            parameters.Clear();
            return ExpressionFromXml(serializedCriteria);
        }
        private Expression ExpressionFromFirstNode(XElement xml) {
            if(xml.IsEmpty || xml.Nodes().Count() < 1)
                return null;
            return ExpressionFromXml(xml.FirstNode as XElement);
        }
        private Expression ExpressionFromXml(XElement xml) {
            if(xml.IsEmpty)
                return null;

            switch(xml.Name.LocalName) {
                case "BinaryExpression":
                    return BinaryExpresssionFromXml(xml);
                case "ConstantExpression":
                case "TypedConstantExpression":
                    return ConstantExpressionFromXml(xml);
                case "ParameterExpression":
                    return ParameterExpressionFromXml(xml);
                case "LambdaExpression":
                    return LambdaExpressionFromXml(xml);
                case "MethodCallExpression":
                    return MethodCallExpressionFromXml(xml);
                case "UnaryExpression":
                    return UnaryExpressionFromXml(xml);
                case "MemberExpression":
                case "FieldExpression":
                case "PropertyExpression":
                    return MemberExpressionFromXml(xml);
                /*
                case "NewExpression":
                case "ListInitExpression":
                case "MemberInitExpression"
                case "ConditionalExpression":
                case "NewArrayExpression":
                case "TypeBinaryExpression":
                case "InvocationExpression":
                */
                default:
                    throw new NotSupportedException(xml.Name.LocalName);
            }
        }
        private IEnumerable<T> ExpressionListFromXml<T>(XElement xml, string elemName) where T : Expression {
            IEnumerable<XElement> elements = xml.Elements(elemName).Elements();
            List<T> list = new List<T>();
            foreach(XElement tXml in elements) {
                object parsed = ExpressionFromXml(tXml);
                list.Add((T)parsed);
            }
            return list;
        }
        private Type TypeFromXml(XElement xml) {
            Debug.Assert(xml.Elements().Count() == 1);
            return TypeFromXmlCore(xml.Elements().First());
        }
        private Type TypeFromXmlCore(XElement xml) {
            if(!xml.HasElements)
                return Helper.GetType(xml.Attribute("Name").Value);

            var genericArgumentTypes = from genArgXml in xml.Elements()
                                       select TypeFromXmlCore(genArgXml);
            return Helper.GetType(xml.Attribute("Name").Value, genericArgumentTypes);
        }
        private Type ParameterFromXml(XElement xml) {
            string name = (string)ConstantFromAttribute<string>(xml, "Name");
            Type type = TypeFromXml(xml.Element("Type"));
            return type;
        }
        private object ConstantFromAttribute<T>(XElement xml, string attrName) {
            string objectStringValue = xml.Attribute(attrName).Value;
            if(typeof(Enum).IsAssignableFrom(typeof(T)))
                return Enum.Parse(typeof(T), objectStringValue, false);
            return Convert.ChangeType(objectStringValue, typeof(T), default(IFormatProvider));
        }
        private object ConstantFromAttribute(XElement xml, string attrName, Type type) {
            string objectStringValue = xml.Attribute(attrName).Value;
            if(typeof(Enum).IsAssignableFrom(type))
                return Enum.Parse(type, objectStringValue, false);
            return Convert.ChangeType(objectStringValue, type, default(IFormatProvider));
        }
        private object ConstantFromElement(XElement xml, string elemName, Type expectedType) {
            string objectStringValue = xml.Element(elemName).Value;
            if (objectStringValue.Length == 0)
                return null;
            if(typeof(Type).IsAssignableFrom(expectedType))
                return TypeFromXml(xml.Element("Value"));
            if(typeof(Enum).IsAssignableFrom(expectedType))
                return Enum.Parse(expectedType, objectStringValue, false);
            return Convert.ChangeType(objectStringValue, expectedType, default(IFormatProvider));
        }
        private Expression BinaryExpresssionFromXml(XElement xml) {
            var expressionType = (ExpressionType)ConstantFromAttribute<ExpressionType>(xml, "NodeType");
            ;
            var left = ExpressionFromFirstNode(xml.Element("Left"));
            var right = ExpressionFromFirstNode(xml.Element("Right"));

            if(left.Type != right.Type)
                BinaryExpressionConvert(ref left, ref right);

            var isLifted = (bool)ConstantFromAttribute<bool>(xml, "IsLifted");
            var isLiftedToNull = (bool)ConstantFromAttribute<bool>(xml, "IsLiftedToNull");
            // TODO: IsLifted
            var type = TypeFromXml(xml.Element("Type"));
            var method = MethodInfoFromXml(xml.Element("Method"));
            LambdaExpression conversion = ExpressionFromXml(xml.Element("Conversion")) as LambdaExpression;
            if(expressionType == ExpressionType.Coalesce)
                return Expression.Coalesce(left, right, conversion);
            return Expression.MakeBinary(expressionType, left, right, isLiftedToNull, method);
        }
        private Expression ConstantExpressionFromXml(XElement xml) {
            Type type = TypeFromXml(xml.Element("Type"));
            dynamic result = ConstantFromElement(xml, "Value", type);
            //return Expression.Constant(result, result.GetType());
            return Expression.Constant(result, type);
        }
        private Expression ParameterExpressionFromXml(XElement xml) {
            Type type = TypeFromXml(xml.Element("Type"));
            string name = (string)ConstantFromAttribute<string>(xml, "Name");
            string parameterID = string.Format("{0}|{1}", name, type.FullName);
            if(!parameters.ContainsKey(parameterID))
                parameters.Add(parameterID, Expression.Parameter(type, name));
            return parameters[parameterID];
        }
        private Expression LambdaExpressionFromXml(XElement xml) {
            var body = ExpressionFromFirstNode(xml.Element("Body"));
            var parameters = ExpressionListFromXml<ParameterExpression>(xml, "Parameters");
            var type = TypeFromXml(xml.Element("Type"));
            return Expression.Lambda(type, body, parameters);
        }
        private Expression MethodCallExpressionFromXml(XElement xml) {
            Expression instance = ExpressionFromFirstNode(xml.Element("Object"));
            MethodInfo method = MethodInfoFromXml(xml.Element("Method"));
            IEnumerable<Expression> arguments = ExpressionListFromXml<Expression>(xml, "Arguments");
            if(arguments == null || arguments.Count() == 0)
                arguments = new Expression[0];

            bool isStaticMethod = instance == null;
            if(isStaticMethod)
                return Expression.Call(method, arguments);
            else
				return Expression.Call(instance, method, arguments);
        }
        private Expression UnaryExpressionFromXml(XElement xml) {
            Expression operand = ExpressionFromFirstNode(xml.Element("Operand"));
            MethodInfo method = MethodInfoFromXml(xml.Element("Method"));
            var isLifted = (bool)ConstantFromAttribute<bool>(xml, "IsLifted");
            var isLiftedToNull = (bool)ConstantFromAttribute<bool>(xml, "IsLiftedToNull");
            var expressionType = (ExpressionType)ConstantFromAttribute<ExpressionType>(xml, "NodeType");
            var type = TypeFromXml(xml.Element("Type"));
            // TODO: IsLifted and IsLiftedToNull
            return Expression.MakeUnary(expressionType, operand, type, method);
        }
        private Expression MemberExpressionFromXml(XElement xml) {
            Expression expression = ExpressionFromFirstNode(xml.Element("Expression"));
            MemberInfo member = MemberInfoFromXml(xml.Element("Member"));
            return Expression.MakeMemberAccess(expression, member);
        }
        void BinaryExpressionConvert(ref Expression left, ref Expression right) {
            bool leftIsNull = false;
            ConstantExpression leftAsConstantExpression = left as ConstantExpression;
            if(leftAsConstantExpression != null) {
                if (leftAsConstantExpression.Value == null)
                    leftIsNull = true;
            }

            bool rightIsNull = false;
            ConstantExpression rightAsConstantExpression = right as ConstantExpression;
            if (rightAsConstantExpression != null) {
                if (rightAsConstantExpression.Value == null)
                    rightIsNull = true;
            }

            if (left.Type != right.Type && !leftIsNull && !rightIsNull) {
                UnaryExpression unary;
                if(right is ConstantExpression) {
                    unary = Expression.Convert(left, right.Type);
                    left = unary;
                }
                else if(left is ConstantExpression) {
                    unary = Expression.Convert(right, left.Type);
                    right = unary;
                }
                else {
                    throw new ArgumentException();
                }
            }
        }
        private MethodInfo MethodInfoFromXml(XElement xml) {
            if(xml.IsEmpty)
                return null;
            string name = (string)ConstantFromAttribute<string>(xml, "MethodName");
            Type declaringType = TypeFromXml(xml.Element("DeclaringType"));
            var parametersTypes = from parameters in xml.Element("Parameters").Elements()
                     select TypeFromXml(parameters);
            var genericArgsTypes = from argXml in xml.Element("GenericArgTypes").Elements()
                          select TypeFromXml(argXml);
            return Helper.GetMethod(declaringType, name, parametersTypes.ToArray(), genericArgsTypes.ToArray());
        }
        private MemberInfo PropertyInfoFromXml(XElement xml) {
            string propertyName = (string)ConstantFromAttribute<string>(xml, "PropertyName");
            Type declaringType = TypeFromXml(xml.Element("DeclaringType"));
            /*
            var ps = from paramXml in xml.Element("IndexParameters").Elements()
                     select TypeFromXml(paramXml);
            */
            return declaringType.GetProperty(propertyName);
        }
        private MemberInfo MemberInfoFromXml(XElement xml) {
            MemberTypes memberType = (MemberTypes)ConstantFromAttribute<MemberTypes>(xml, "MemberType");
            switch(memberType) {
                case MemberTypes.Property:
                    return PropertyInfoFromXml(xml);
                case MemberTypes.Method:
                    return MethodInfoFromXml(xml);
                case MemberTypes.Constructor:
                case MemberTypes.Field:
                case MemberTypes.Custom:
                case MemberTypes.Event:
                case MemberTypes.NestedType:
                case MemberTypes.TypeInfo:
                default:
                    throw new NotSupportedException(string.Format("MemberType {0} not supported", memberType));
            }
        }
    }
}
