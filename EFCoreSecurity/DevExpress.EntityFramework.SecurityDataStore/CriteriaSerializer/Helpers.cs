using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DevExpress.EntityFramework.SecurityDataStore {
    class CriteriaSerializeHelper {
        HashSet<Assembly> assemblies = new HashSet<Assembly>();
        public CriteriaSerializeHelper() {
            assemblies = new HashSet<Assembly> {
                typeof(ExpressionType).Assembly,
                typeof(string).Assembly,
                typeof(List<>).Assembly,
                typeof(XElement).Assembly,
                Assembly.GetExecutingAssembly(),
                Assembly.GetEntryAssembly()
            };
            foreach(var asm in AppDomain.CurrentDomain.GetAssemblies()) {
                assemblies.Add(asm);
            }   
        }
        public void RegisterAdditionalAssemblies(IList<Assembly> additionalAssemblies) {
            foreach(Assembly assembly in additionalAssemblies)
                assemblies.Add(assembly);
        }
        public Type GetType(string typeName) {
            Type type;
            if(string.IsNullOrEmpty(typeName))
                throw new ArgumentNullException("typeName");

            bool isArrayType = typeName.EndsWith("[]");
            if(isArrayType)
                return GetType(typeName.Substring(0, typeName.Length - 2)).MakeArrayType();

            foreach(Assembly assembly in assemblies) {
                if(assembly == null)
                    continue;
                type = assembly.GetType(typeName);
                if(type != null)
                    return type;
            }

            type = this.GetType().Assembly.GetType(typeName);
            
            // if(type == null)
            //    type = Type.GetType(typeName);

            if(type != null)
                return type;

            throw new ArgumentException("Couldn't find a type", typeName);
        }
        public Type GetType(string typeName, IEnumerable<Type> genericArgumentTypes) {
            return GetType(typeName).MakeGenericType(genericArgumentTypes.ToArray());
        }
        public MethodInfo GetMethod(Type declaringType, string name, Type[] parameterTypes, Type[] genArgTypes) {
            var methods = from methodInfo in declaringType.GetMethods()
                          where methodInfo.Name == name
                          select methodInfo;
            foreach(var method in methods) {
                // Would be nice to remove the try/catch
                try {
                    MethodInfo realMethod = method;
                    if(method.IsGenericMethod) {
                        realMethod = method.MakeGenericMethod(genArgTypes);
                    }
                    var methodParameterTypes = realMethod.GetParameters().Select(p => p.ParameterType);
                    if(AreEnumerablesEqual(parameterTypes, methodParameterTypes)) {
                        return realMethod;
                    }
                }
                catch(ArgumentException) {
                    continue;
                }
            }
            return null;
        }
        private bool AreEnumerablesEqual<T>(IEnumerable<T> first, IEnumerable<T> second) {
            T[] firstArray = first.ToArray();
            T[] secondArray = second.ToArray();
            if(firstArray.Length != secondArray.Length)
                return false;
            for(int i = 0; i < firstArray.Length; i++)
                if(!firstArray[i].Equals(secondArray[i]))
                    return false;
            return true;
        }
    }
}
