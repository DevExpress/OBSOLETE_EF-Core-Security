using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Security {
    public static class ModificationsMembersHelper {
        public static IEnumerable<SecurityObjectBuilder> GetModificationsDifferences(IPermissionProcessor processor, IModel model, IEnumerable<object> processingEntity, IEnumerable<object> blockedObjects) {
            List<SecurityObjectBuilder> modifiedObjects = new List<SecurityObjectBuilder>();
            foreach(object targetObject in processingEntity) {
                SecurityObjectBuilder securityObjectBuilder = new SecurityObjectBuilder();                
                modifiedObjects.Add(securityObjectBuilder);
                securityObjectBuilder.RealObject = targetObject;
                securityObjectBuilder.BlockedProperties = GetBlockedProperties(targetObject, model, processor);
                securityObjectBuilder.BlockedNavigationProperties = GetBlockedNavigationProperties(targetObject, blockedObjects, model, processor);
                securityObjectBuilder.BlockedObjectsInListProperty = GetBlockedObjectsInListProperty(targetObject, blockedObjects, model, processor);
            }
            foreach(SecurityObjectBuilder modyficationsObject in modifiedObjects) {
                modyficationsObject.ModifyObjectsInListProperty = GetModifyObjectsInListProperty(modyficationsObject, modifiedObjects, model, processor);
            }

            return modifiedObjects;
        }
        private static List<string> GetBlockedProperties(object targetObject, IModel model, IPermissionProcessor processor) {
            List<string> denyMembers = new List<string>();
            Type targetType = targetObject.GetType();
            IEntityType entityType = model.FindEntityType(targetType);
            IEnumerable<IProperty> properties = entityType.GetProperties();
            foreach(IProperty property in properties) {                
                if(property.IsKey()) {
                    continue;
                }
                if(property.GetContainingForeignKeys().Count() > 0) {
                    continue;
                }
                if(property.GetContainingKeys().Count() > 0) {
                    continue;
                }                
                bool isGranted = processor.IsGranted(targetType, SecurityOperation.Read, targetObject, property.Name);
                if(!isGranted) {
                    denyMembers.Add(property.Name);
                }
            }
            return denyMembers;
        }
        private static List<string> GetBlockedNavigationProperties(object targetObject, IEnumerable<object> denyObjects, IModel model, IPermissionProcessor processor) {
            List<string> denyNavigationProperties = new List<string>();
            Type targetType = targetObject.GetType();
            IEntityType entityType = model.FindEntityType(targetType);
            IEnumerable<INavigation> properties = entityType.GetNavigations();
            IEnumerable<PropertyInfo> propertiesInfo = targetType.GetRuntimeProperties();
            foreach(INavigation propertyNavigation in properties) {
                bool isGranted = processor.IsGranted(targetObject.GetType(), SecurityOperation.Read, targetObject, propertyNavigation.Name);
                if(!isGranted) {
                    denyNavigationProperties.Add(propertyNavigation.Name);
                    denyNavigationProperties.AddRange(GetDenyForeignKey(propertyNavigation).Select(p=>p.Name));
                    
                }
                else {
                    PropertyInfo navigationPropertyInfo = propertiesInfo.First(p => p.Name == propertyNavigation.Name);
                    if(!propertyNavigation.IsCollection()) {
                        object valueNavigationProperty = navigationPropertyInfo.GetValue(targetObject);

                        if(valueNavigationProperty != null) {
                            bool isDenyNavigationObject = denyObjects.Contains(valueNavigationProperty);
                            if(isDenyNavigationObject) {
                                denyNavigationProperties.Add(propertyNavigation.Name);
                                denyNavigationProperties.AddRange(GetDenyForeignKey(propertyNavigation).Select(p => p.Name));
                            }
                        }
                    }
                }
            }
            return denyNavigationProperties;
        }

        private static IEnumerable<IPropertyBase> GetDenyForeignKey(INavigation propertyNavigation) {
            List<IPropertyBase> vaseProperties = new List<IPropertyBase>(); 
            foreach(IProperty property in propertyNavigation.ForeignKey.Properties) {
                vaseProperties.Add(property);
            }
            return vaseProperties;
        }

        private static Dictionary<string, List<object>> GetBlockedObjectsInListProperty(object targetObject, IEnumerable<object> denyObjects, IModel model, IPermissionProcessor processor) {
            Dictionary<string, List<object>> denyObjectsInListProperty = new Dictionary<string, List<object>>();
            Type targetType = targetObject.GetType();
            IEntityType entityType = model.FindEntityType(targetType);
            IEnumerable<INavigation> properties = entityType.GetNavigations();
            IEnumerable<PropertyInfo> propertiesInfo = targetType.GetRuntimeProperties();
            foreach(INavigation propertyNavigation in properties) {
                PropertyInfo navigationListPropertyInfo = propertiesInfo.First(p => p.Name == propertyNavigation.Name);
                if(propertyNavigation.IsCollection()) {
                    IEnumerable listObject = (IEnumerable)navigationListPropertyInfo.GetValue(targetObject);
                    if(listObject != null) {
                        foreach(object objectInList in listObject) {
                            if(denyObjects.Contains(objectInList)) {
                                List<object> denyObjectInList;
                                if(!denyObjectsInListProperty.TryGetValue(propertyNavigation.Name, out denyObjectInList)) {
                                    denyObjectInList = new List<object>();
                                    denyObjectsInListProperty.Add(propertyNavigation.Name, denyObjectInList);
                                }
                                denyObjectInList.Add(objectInList);
                            }
                        } 
                    }
                }
            }
            return denyObjectsInListProperty;
        }
        private static Dictionary<string, List<SecurityObjectBuilder>> GetModifyObjectsInListProperty(SecurityObjectBuilder modyficationsObject, List<SecurityObjectBuilder> modyficationsObjects, IModel model, IPermissionProcessor processor) {
            Dictionary<string, List<SecurityObjectBuilder>> denyObjectsInListProperty = new Dictionary<string, List<SecurityObjectBuilder>>();
            Type targetType = modyficationsObject.RealObject.GetType();
            IEntityType entityType = model.FindEntityType(targetType);
            IEnumerable<INavigation> properties = entityType.GetNavigations();
            IEnumerable<PropertyInfo> propertiesInfo = targetType.GetRuntimeProperties();
            foreach(INavigation propertyNavigation in properties) {
                PropertyInfo navigationListPropertyInfo = propertiesInfo.First(p => p.Name == propertyNavigation.Name);
                if(propertyNavigation.IsCollection()) {
                    IEnumerable listObject = (IEnumerable)navigationListPropertyInfo.GetValue(modyficationsObject.RealObject);
                    if(listObject != null) {
                        foreach(object objectInList in listObject) {
                            List<object> denyObject;
                            modyficationsObject.BlockedObjectsInListProperty.TryGetValue(propertyNavigation.Name, out denyObject);
                            if(denyObject != null && denyObject.Contains(objectInList)) {
                                continue;
                            }
                            SecurityObjectBuilder securityObjectMetaData = modyficationsObjects.FirstOrDefault(p => p.RealObject == objectInList);

                            if(securityObjectMetaData != null && securityObjectMetaData.NeedToModify()) {
                                List<SecurityObjectBuilder> modyfiObjectInList;
                                if(!denyObjectsInListProperty.TryGetValue(propertyNavigation.Name, out modyfiObjectInList)) {
                                    modyfiObjectInList = new List<SecurityObjectBuilder>();
                                    denyObjectsInListProperty.Add(propertyNavigation.Name, modyfiObjectInList);
                                }
                                SecurityObjectBuilder objectInListMetaInfo = modyficationsObjects.First(p => p.RealObject == objectInList);
                                modyfiObjectInList.Add(objectInListMetaInfo);
                            }
                        } 
                    }
                }
            }
            return denyObjectsInListProperty;
        }
    }
}
