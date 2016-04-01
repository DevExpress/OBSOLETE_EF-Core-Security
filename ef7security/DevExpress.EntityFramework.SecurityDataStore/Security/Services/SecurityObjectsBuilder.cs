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
    public class SecurityObjectsBuilder : ISecurityObjectsBuilder {
        private SecurityDbContext securityDbContext;
        private SecurityDbContext nativeDbContext;
        private SecurityObjectRepository securityObjectRepository;
        private PermissionProcessor permissionProcessor;
        private Dictionary<Type, IEntityType> entityInfoСache = new Dictionary<Type, IEntityType>();
        private Dictionary<IEntityType, IEnumerable<INavigation>> entityInfoNavigationsСache = new Dictionary<IEntityType, IEnumerable<INavigation>>();
        private Dictionary<IEntityType, IEnumerable<IProperty>> entityInfoPropertyСache = new Dictionary<IEntityType, IEnumerable<IProperty>>();
        private Dictionary<Type, IEnumerable<PropertyInfo>> entityPropertyInfoCache = new Dictionary<Type, IEnumerable<PropertyInfo>>();

        public SecurityObjectsBuilder(DbContext securityDbContext, SecurityObjectRepository securityObjectRepository, PermissionProcessor permissionProcessor) {
            this.securityDbContext = (SecurityDbContext)securityDbContext;
            this.nativeDbContext = ((SecurityDbContext)securityDbContext).realDbContext;
            this.securityObjectRepository = securityObjectRepository;
            this.permissionProcessor = permissionProcessor;
        }
        public IEnumerable<object> ProcessObjects(IEnumerable<object> objects) {
            List<object> processingEntity = GetAllEntity(objects);
            List<object> notEntityObjects = GetNotEntityObjects(objects);
            List<object> denyObjects = GetDenyObjects(processingEntity);
            foreach(object objectToDelete in denyObjects) {
                processingEntity.Remove(objectToDelete);
            }
            List<SecurityObjectBuilder> modyficationsObjects = GetModificationsMembers(processingEntity, denyObjects);
            List<object> securityObjects = CreateSecurityObjects(processingEntity, denyObjects, modyficationsObjects);
            List<object> resultObject = GetOrCreateResultObjects(securityObjects, objects, modyficationsObjects);
            List<object> allResultObjects = GetAllEntity(resultObject);     

            return resultObject;
        }

        private List<object> GetOrCreateResultObjects(List<object> securityObjects, IEnumerable<object> objects, List<SecurityObjectBuilder> modyficationsObjects) {
            List<object> resultObject = new List<object>();

            foreach(object targetObject in objects) {
                SecurityObjectBuilder objectMetaData = modyficationsObjects.FirstOrDefault(p => p.RealObject == targetObject);
                if(objectMetaData != null && objectMetaData.SecurityObject != null) {
                    resultObject.Add(objectMetaData.SecurityObject);
                }
                else {
                    resultObject.Add(targetObject);
                }
            }
            return resultObject;
        }

        private List<object> CreateSecurityObjects(List<object> processingEntity, List<object> denyObjects, List<SecurityObjectBuilder> modyficationsObjects) {
            List<object> securityObjects = new List<object>();
            foreach(object targetObject in processingEntity) {
                SecurityObjectBuilder modifyObjectMetaInfo = modyficationsObjects.First(p => p.RealObject == targetObject);
                if(modifyObjectMetaInfo != null) {
                    object securityObject;
                    if(modifyObjectMetaInfo.SecurityObject == null) {
                        securityObject = modifyObjectMetaInfo.CreateSecurityObject();
                    }
                    else {
                        securityObject = modifyObjectMetaInfo.SecurityObject;
                    }
                        securityObjects.Add(securityObject);
                }
            }
            return securityObjects;
        }

        private List<SecurityObjectBuilder> GetModificationsMembers(List<object> processingEntity, List<object> denyObjects) {
            List<SecurityObjectBuilder> modyficationsObjects = new List<SecurityObjectBuilder>();
            foreach(object targetObject in processingEntity) {
                SecurityObjectBuilder modifyObjectMetaInfo = new SecurityObjectBuilder(securityObjectRepository,securityDbContext);
                securityObjectRepository.RegisterObjects(modifyObjectMetaInfo);
                modyficationsObjects.Add(modifyObjectMetaInfo);
                modifyObjectMetaInfo.RealObject = targetObject;
                modifyObjectMetaInfo.DenyProperties = GetDenyProperties(targetObject);
                modifyObjectMetaInfo.DenyNavigationProperties = GetDenyNavigationProperties(targetObject, denyObjects);
                modifyObjectMetaInfo.DenyObjectsInListProperty = GetDenyObjectsInListProperty(targetObject, denyObjects);
            }
            foreach(SecurityObjectBuilder modyficationsObject in modyficationsObjects) {
                modyficationsObject.ModifyObjectsInListProperty = GetModifyObjectsInListProperty(modyficationsObject, modyficationsObjects);
            }
            return modyficationsObjects;
        }

        private Dictionary<string, List<SecurityObjectBuilder>> GetModifyObjectsInListProperty(SecurityObjectBuilder modyficationsObject, List<SecurityObjectBuilder> modyficationsObjects) {
            Dictionary<string, List<SecurityObjectBuilder>> denyObjectsInListProperty = new Dictionary<string, List<SecurityObjectBuilder>>();
            Type targetType = modyficationsObject.RealObject.GetType();
            IEntityType entityType = entityInfoСache[targetType];
            IEnumerable<INavigation> properties = entityInfoNavigationsСache[entityType];
            IEnumerable<PropertyInfo> propertiesInfo = entityPropertyInfoCache[targetType];
            foreach(INavigation propertyNavigation in properties) {
                PropertyInfo navigationListPropertyInfo = propertiesInfo.First(p => p.Name == propertyNavigation.Name);
                if(propertyNavigation.IsCollection()) {
                    IEnumerable listObject = (IEnumerable)navigationListPropertyInfo.GetValue(modyficationsObject.RealObject);
                    foreach(object objectInList in listObject) {
                        List<object> denyObject;
                        modyficationsObject.DenyObjectsInListProperty.TryGetValue(propertyNavigation.Name, out denyObject);
                        if(denyObject != null && denyObject.Contains(objectInList)) {
                            continue;
                        }
                        SecurityObjectBuilder securityObjectMetaData = modyficationsObjects.FirstOrDefault(p => p.RealObject == objectInList);

                        if(securityObjectMetaData != null && securityObjectMetaData.NeedModify()) {
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
            return denyObjectsInListProperty;
        }
        private Dictionary<string, List<object>> GetDenyObjectsInListProperty(object targetObject, List<object> denyObjects) {
            Dictionary<string, List<object>> denyObjectsInListProperty = new Dictionary<string, List<object>>();
            Type targetType = targetObject.GetType();
            IEntityType entityType = entityInfoСache[targetType];
            IEnumerable<INavigation> properties = entityInfoNavigationsСache[entityType];
            IEnumerable<PropertyInfo> propertiesInfo = entityPropertyInfoCache[targetType];
            foreach(INavigation propertyNavigation in properties) {
                PropertyInfo navigationListPropertyInfo = propertiesInfo.First(p => p.Name == propertyNavigation.Name);
                if(propertyNavigation.IsCollection()) {
                    IEnumerable listObject = (IEnumerable)navigationListPropertyInfo.GetValue(targetObject);
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
            return denyObjectsInListProperty;
        }
        private List<string> GetDenyNavigationProperties(object targetObject, IEnumerable<object> denyObjects) {
            List<string> denyNavigationProperties = new List<string>();
            Type targetType = targetObject.GetType();
            IEntityType entityType = entityInfoСache[targetType];
            IEnumerable<INavigation> properties = entityInfoNavigationsСache[entityType];
            IEnumerable<PropertyInfo> propertiesInfo = entityPropertyInfoCache[targetType];
            foreach(INavigation propertyNavigation in properties) {
                bool isGranted = permissionProcessor.IsGranted(targetObject.GetType(), SecurityOperation.Read, targetObject, propertyNavigation.Name);
                if(!isGranted) {
                    denyNavigationProperties.Add(propertyNavigation.Name);
                }
                else {
                    PropertyInfo navigationPropertyInfo = propertiesInfo.First(p => p.Name == propertyNavigation.Name);
                    if(!propertyNavigation.IsCollection()) {
                        object valueNavigationProperty = navigationPropertyInfo.GetValue(targetObject);

                        if(valueNavigationProperty != null) {
                            bool isDenyNavigationObject = denyObjects.Contains(valueNavigationProperty);
                            if(isDenyNavigationObject) {
                                denyNavigationProperties.Add(propertyNavigation.Name);
                            }
                        }
                    }
                }
            }
            return denyNavigationProperties;
        }
        private List<string> GetDenyProperties(object targetObject) {
            List<string> denyMembers = new List<string>();
            Type targetType = targetObject.GetType();
            IEntityType entityType = entityInfoСache[targetType];
            IEnumerable<IProperty> properties = entityInfoPropertyСache[entityType];
            IEnumerable<PropertyInfo> propertiesInfo = entityPropertyInfoCache[targetType];
            foreach(IProperty property in properties) {
                PropertyInfo propertyInfo = propertiesInfo.FirstOrDefault(p => p.Name == property.Name);
                if(property.IsKey()) {
                    continue;
                }
                if(property.FindContainingForeignKeys().Count() > 0) {
                    continue;
                }
                if(property.FindContainingKeys().Count() > 0) {
                    continue;
                }
                if(propertyInfo != null && propertyInfo.GetGetMethod().IsStatic) {
                    continue;
                }
                bool isGranted = permissionProcessor.IsGranted(targetType, SecurityOperation.Read, targetObject, property.Name);
                if(!isGranted) {
                    denyMembers.Add(property.Name);
                }
            }
            return denyMembers;
        }
        private List<object> GetDenyObjects(List<object> allObject) {
            List<object> objectsToDelete = new List<object>();
            foreach(object targetObject in allObject) {
                bool result = permissionProcessor.IsGranted(targetObject.GetType(), SecurityOperation.Read, targetObject);
                if(!result) {
                    objectsToDelete.Add(targetObject);
                }
            }
            return objectsToDelete;
        }
        private List<object> GetNotEntityObjects(IEnumerable<object> objects) {
            List<object> notEntityObjects = new List<object>();
            foreach(object targetObject in objects) {
                IEntityType entityType = nativeDbContext.Model.FindEntityType(targetObject.GetType());
                if(entityType == null) {
                    notEntityObjects.Add(targetObject);
                }
            }
            return notEntityObjects;
        }
        private List<object> GetAllEntity(IEnumerable<object> objects) {
            List<object> allObject = new List<object>();
            foreach(object targetObject in objects) {
                RecursionGetEntity(targetObject, allObject);
            }
            return allObject;
        }
        private void RecursionGetEntity(object targetObject, List<object> allObject) {
            if(allObject.Contains(targetObject) || targetObject == null) {
                return;
            }
            Type targetType = targetObject.GetType();
            IEntityType entityInfo;
            if(!entityInfoСache.TryGetValue(targetType, out entityInfo)) {
                entityInfo = nativeDbContext.Model.FindEntityType(targetType);
                if(entityInfo != null) {
                    entityInfoСache.Add(targetType, entityInfo);
                    entityInfoNavigationsСache.Add(entityInfo, entityInfo.GetNavigations());
                    entityInfoPropertyСache.Add(entityInfo, entityInfo.GetProperties());
                    entityPropertyInfoCache.Add(targetType, targetType.GetRuntimeProperties());
                }
            }

            if(entityInfo != null) {
                allObject.Add(targetObject);
                IEnumerable<INavigation> navigations = entityInfoNavigationsСache[entityInfo];
                entityInfoNavigationsСache[entityInfo] = navigations;
                foreach(INavigation navigationProperty in navigations) {
                    PropertyInfo navigationPropertyInfo = entityPropertyInfoCache[targetType].First(p => p.Name == navigationProperty.Name);
                    object valueNavigationProperty = navigationPropertyInfo.GetValue(targetObject);
                    if(valueNavigationProperty != null) {
                        if(navigationProperty.IsCollection()) {
                            foreach(object collectionObject in (IEnumerable)valueNavigationProperty) {
                                RecursionGetEntity(collectionObject, allObject);
                            }
                        }
                        else {
                            RecursionGetEntity(valueNavigationProperty, allObject);
                        }
                    }
                }
            }
        }
    }
}
