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
        public static IEnumerable<object> GetAllLinkedObjects(this IModel model, IEnumerable<object> targetObjects) {
            HashSet<object> resultObjects = new HashSet<object>();
            foreach(object targetObject in targetObjects) {
                IEnumerable<object> allObjects = GetAllLinkedObjects(model, targetObject);
                foreach(object obj in allObjects)
                    resultObjects.Add(obj);
            }
            return resultObjects;
        }
        public static IEnumerable<object> GetAllLinkedObjects(this IModel model, object obj) {
            List<object> linkedObjects = new List<object>();
            GetAllLinkedObjects(model, obj, linkedObjects);
            return linkedObjects;
        }   
        private static void GetAllLinkedObjects(IModel model, object targetObject, List<object> objects) {
            if(objects.Contains(targetObject) || targetObject == null) {
                return;
            }
            Type targetType = targetObject.GetType();
            IEntityType entityInfo = model.FindEntityType(targetType);
            if(entityInfo != null) {
                objects.Add(targetObject);
                IEnumerable<INavigation> navigations = entityInfo.GetNavigations();
                foreach(INavigation navigationProperty in navigations) {
                    PropertyInfo navigationPropertyInfo = targetType.GetRuntimeProperties().First(p => p.Name == navigationProperty.Name);
                    object navigationPropertyValue = navigationPropertyInfo.GetValue(targetObject);
                    if(navigationPropertyValue != null) {
                        if(navigationProperty.IsCollection())
                            foreach(object collectionObject in (IEnumerable)navigationPropertyValue) {
                                GetAllLinkedObjects(model, collectionObject, objects);
                            }
                        else
                            GetAllLinkedObjects(model, navigationPropertyValue, objects);
                    }
                }
            }
        }
    }
}
