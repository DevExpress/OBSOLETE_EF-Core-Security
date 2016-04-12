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
        public string Serialize(Expression criteria) {
            return SerializeAsXElement(criteria).ToString();
        }
        public XElement SerializeAsXElement(Expression criteria) {
            if(!(criteria is ParameterExpression)) {
                if(!(criteria is LambdaExpression))
                    throw new ArgumentException("A criteria must be LambdaExpression");
                LambdaExpression lambdaExpression = (LambdaExpression)criteria;
                if(lambdaExpression.Parameters.Count != 2)
                    throw new ArgumentException("A criteria must have 2 parameters (object and dbContext)");
                if(lambdaExpression.ReturnType != typeof(bool))
                    throw new ArgumentException("A criteria must return Boolean");
            }
            return GetXmlFromExpressionCore(criteria);
        }
        private XElement GenerateXmlFromExpression(string propName, Expression e) {
            return new XElement(propName, GetXmlFromExpressionCore(e));
        }
        private string GetExpressionType(Expression e) {
            string result = "";
            if(e is BinaryExpression) {
                result = "BinaryExpression";
            }
            else if(e is BlockExpression) {
                result = "BlockExpression";
            }
            else if(e is ConditionalExpression) {
                result = "ConditionalExpression";
            }
            else if(e is ConstantExpression) {
                result = "ConstantExpression";
            }
            else if(e is DebugInfoExpression) {
                result = "DebugInfoExpression";
            }
            else if(e is DefaultExpression) {
                result = "DefaultExpression";
            }
            else if(e is DynamicExpression) {
                result = "DynamicExpression";
            }
            else if(e is GotoExpression) {
                result = "GotoExpression";
            }
            else if(e is IndexExpression) {
                result = "IndexExpression";
            }
            else if(e is InvocationExpression) {
                result = "InvocationExpression";
            }
            else if(e is LabelExpression) {
                result = "LabelExpression";
            }
            else if(e is LambdaExpression) {
                result = "LambdaExpression";
            }
            else if(e is ListInitExpression) {
                result = "ListInitExpression";
            }
            else if(e is LoopExpression) {
                result = "LoopExpression";
            }
            else if(e is MemberExpression) {
                result = "MemberExpression";
            }
            else if(e is MemberInitExpression) {
                result = "MemberInitExpression";
            }
            else if(e is MethodCallExpression) {
                result = "MethodCallExpression";
            }
            else if(e is NewArrayExpression) {
                result = "NewArrayExpression";
            }
            else if(e is NewExpression) {
                result = "NewExpression";
            }
            else if(e is ParameterExpression) {
                result = "ParameterExpression";
            }
            else if(e is RuntimeVariablesExpression) {
                result = "RuntimeVariablesExpression";
            }
            else if(e is SwitchExpression) {
                result = "SwitchExpression";
            }
            else if(e is TryExpression) {
                result = "TryExpression";
            }
            else if(e is TypeBinaryExpression) {
                result = "TypeBinaryExpression";
            }
            else if(e is UnaryExpression) {
                result = "UnaryExpression";
            }
            if(result != string.Empty)
                return result;
            else
                throw new NotSupportedException(e.GetType().ToString());
        }
        public XElement GetXmlFromExpressionCore(Expression e) {
            if(e == null)
                return null;

            string xName = GetExpressionType(e);

            PropertyInfo[] properties = e.GetType().GetProperties();
            object[] XElementValues = new object[properties.Count()];

            int index = 0;
            foreach(var property in e.GetType().GetProperties()) {
                object value = property.GetValue(e, null);
                XElementValues[index++] = GenerateXmlFromProperty(property.PropertyType,
                    property.Name, value ?? string.Empty);
            }
            return new XElement(xName, XElementValues);
        }
        private object GenerateXmlFromProperty(Type propertyType, string propertyName, object value) {
            if(IsPrimitiveType(propertyType))
                return PrimitiveToXml(propertyName, value);
            if(propertyType.Equals(typeof(object))) {
                return ObjectToXml(propertyName, value);
            }
            if(typeof(Expression).IsAssignableFrom(propertyType))
                return GenerateXmlFromExpression(propertyName, value as Expression);
            if(propertyType.Equals(typeof(Type)))
                return TypeToXml(propertyName, value as Type);
            if(value is MethodInfo || propertyType.Equals(typeof(MethodInfo)))
                return MethodInfoToXml(propertyName, value as MethodInfo);
            if(value is MemberInfo || propertyType.Equals(typeof(MemberInfo)))
                return MemberInfoToXml(propertyName, value as MemberInfo);

            if(IsIEnumerableOf<Expression>(propertyType))
                return ExpressionListToXml(propertyName, AsIEnumerableOf<Expression>(value));
            /*
            if(IsIEnumerableOf<MemberInfo>(propertyType))
                return null; // GenerateXmlFromMemberInfoList(propName, AsIEnumerableOf<MemberInfo>(value));
            if(IsIEnumerableOf<ElementInit>(propertyType))
                return null; // GenerateXmlFromElementInitList(propName, AsIEnumerableOf<ElementInit>(value));
            if(IsIEnumerableOf<MemberBinding>(propertyType))
                return null; // GenerateXmlFromBindingList(propName, AsIEnumerableOf<MemberBinding>(value));
            */
            throw new NotSupportedException(propertyName);
        }
        private bool IsIEnumerableOf<T>(Type propType) {
            if(!propType.IsGenericType)
                return false;
            Type[] typeArgs = propType.GetGenericArguments();
            if(typeArgs.Length != 1)
                return false;
            if(!typeof(T).IsAssignableFrom(typeArgs[0]))
                return false;
            if(!typeof(IEnumerable<>).MakeGenericType(typeArgs).IsAssignableFrom(propType))
                return false;
            return true;
        }
        private bool IsPrimitiveType(Type type) {
            Type[] primitiveTypes = new[] { typeof(string), typeof(int), typeof(bool), typeof(ExpressionType) };
            return primitiveTypes.Contains(type);
        }
        private IEnumerable<T> AsIEnumerableOf<T>(object value) {
            if(value == null)
                return null;
            return (value as IEnumerable).Cast<T>();
        }
        private object MemberInfoToXml(string propName, MemberInfo memberInfo) {
            if(memberInfo == null)
                return new XElement(propName);
            return new XElement(propName,
                        new XAttribute("MemberType", memberInfo.MemberType),
                        new XAttribute("PropertyName", memberInfo.Name),
                            TypeToXml("DeclaringType", memberInfo.DeclaringType));
            //new XElement("IndexParameters",
            //from param in memberInfo.GetIndexParameters()
            //select GenerateXmlFromType("Type", param.ParameterType)));
        }
        private object MethodInfoToXml(string propName, MethodInfo methodInfo) {
            if(methodInfo == null)
                return new XElement(propName);
            return new XElement(propName,
                        new XAttribute("MemberType", methodInfo.MemberType),
                        new XAttribute("MethodName", methodInfo.Name),
                        TypeToXml("DeclaringType", methodInfo.DeclaringType),
                        new XElement("Parameters",
                            from param in methodInfo.GetParameters()
                            select TypeToXml("Type", param.ParameterType)),
                        new XElement("GenericArgTypes",
                            from argType in methodInfo.GetGenericArguments()
                            select TypeToXml("Type", argType)));
        }
        private object ExpressionListToXml(string propName, IEnumerable<Expression> expressions) {
            XElement result = new XElement(propName,
                    from expression in expressions
                    select GetXmlFromExpressionCore(expression));
            return result;
        }
        private object ObjectToXml(string propertyName, object value) {
            object result = null;
            if(value is Type)
                result = TypeCoreToXml((Type)value);
            else
                result = value.ToString();
            return new XElement(propertyName, result);
        }
        private object TypeToXml(string propName, Type type) {
            return new XElement(propName, TypeCoreToXml(type));
        }
        private object TypeCoreToXml(Type type) {
            if(type.IsGenericType) {
                return new XElement("Type",
                                        new XAttribute("Name", type.GetGenericTypeDefinition().FullName),
                                        from genArgType in type.GetGenericArguments()
                                        select TypeCoreToXml(genArgType));
            }
            else {
                return new XElement("Type", new XAttribute("Name", type.FullName));
            }
        }
        private object PrimitiveToXml(string propName, object value) {
            return new XAttribute(propName, value);
        }
    }
}
