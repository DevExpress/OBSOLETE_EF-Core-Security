using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public static class ModelExtensions {
        public static IEnumerable<object> GetAllObjects(this IModel model, IEnumerable<object> targetObjects) {
            List<object> resultObjects = new List<object>();
            foreach(object obj in targetObjects) {
                IEnumerable<object> resultObj = GetAllObjects(model, obj);
                foreach(object result in resultObj) {
                    if(!resultObjects.Contains(result)) {
                        resultObjects.Add(result);
                    }
                }
            }
            return resultObjects;
        }
        public static IEnumerable<object> GetAllObjects(this IModel model, object obj) {
            List<object> collectionObjects = new List<object>();
            RecursionGetEntity(model, obj, collectionObjects);
            return collectionObjects;
        }    
        private static void RecursionGetEntity(IModel model, object targetObject, List<object> allObject) {
            if(allObject.Contains(targetObject) || targetObject == null) {
                return;
            }
            Type targetType = targetObject.GetType();
            IEntityType entityInfo = model.FindEntityType(targetType);
            if(entityInfo != null) {
                allObject.Add(targetObject);
                IEnumerable<INavigation> navigations = entityInfo.GetNavigations();
                foreach(INavigation navigationProperty in navigations) {
                    PropertyInfo navigationPropertyInfo = targetType.GetRuntimeProperties().First(p => p.Name == navigationProperty.Name);
                    object valueNavigationProperty = navigationPropertyInfo.GetValue(targetObject);
                    if(valueNavigationProperty != null) {
                        if(navigationProperty.IsCollection()) {
                            foreach(object collectionObject in (IEnumerable)valueNavigationProperty) {
                                RecursionGetEntity(model, collectionObject, allObject);
                            }
                        }
                        else {
                            RecursionGetEntity(model, valueNavigationProperty, allObject);
                        }
                    }
                }
            }
        }
    }
}
