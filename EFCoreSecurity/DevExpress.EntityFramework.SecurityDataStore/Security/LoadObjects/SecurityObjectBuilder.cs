using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DevExpress.EntityFramework.SecurityDataStore.Security.BaseSecurityEntity;

namespace DevExpress.EntityFramework.SecurityDataStore.Security {
    public class SecurityObjectBuilder {
        public object RealObject { get; set; } 
        public object SecurityObject { get; set; } 
        public List<string> BlockedProperties { get; set; }
            = new List<string>();
        public List<string> BlockedNavigationProperties { get; set; }
            = new List<string>();
        public Dictionary<string, List<object>> BlockedObjectsInListProperty { get; set; }
            = new Dictionary<string, List<object>>();
        public Dictionary<string, List<SecurityObjectBuilder>> ModifyObjectsInListProperty { get; set; }
            = new Dictionary<string, List<SecurityObjectBuilder>>();
        public Dictionary<string, object> defaultValueDictionary = new Dictionary<string, object>();
        public Dictionary<string, object> originalValueSecurityObjectDictionary = new Dictionary<string, object>();

        public object CreateRealObject(IModel model, ISecurityObjectRepository securityObjectRepository) {
            Type targetType = SecurityObject.GetType();
            RealObject = Activator.CreateInstance(SecurityObject.GetType());
            IEntityType entityType = model.FindEntityType(targetType);
            IEnumerable<PropertyInfo> properiesInfo = targetType.GetRuntimeProperties();
            IEnumerable<INavigation> navigations = entityType.GetNavigations();

            IReadOnlyList<IProperty> primaryKeyProperties = entityType.FindPrimaryKey().Properties;

            foreach(PropertyInfo propertyInfo in properiesInfo) {
                object defaultValue = propertyInfo.GetValue(RealObject);
                defaultValueDictionary[propertyInfo.Name] = defaultValue;
                if(navigations.Any(p => p.Name == propertyInfo.Name)) {
                    INavigation navigation = navigations.First(p => p.Name == propertyInfo.Name);
                    if(navigation.IsCollection()) {
                        IClrCollectionAccessor collectionAccessor = navigation.GetCollectionAccessor();
                        IEnumerable realObjectListPropertyValue = (IEnumerable)propertyInfo.GetValue(RealObject);
                        IEnumerable securityObjectListPropertyValue = (IEnumerable)propertyInfo.GetValue(SecurityObject);
                        if(securityObjectListPropertyValue != null && realObjectListPropertyValue != null) {
                            foreach(object objectInListProperty in securityObjectListPropertyValue) {
                                SecurityObjectBuilder metadata = securityObjectRepository.GetObjectMetaData(objectInListProperty);
                                if(metadata == null) {
                                    metadata = new SecurityObjectBuilder();
                                    securityObjectRepository.RegisterBuilder(metadata);
                                    metadata.SecurityObject = objectInListProperty;
                                    metadata.CreateRealObject(model, securityObjectRepository);
                                }
                                collectionAccessor.Add(RealObject, metadata.RealObject);
                            }
                        }
                    }
                    else {
                        object realValue = propertyInfo.GetValue(SecurityObject);
                        if(!Equals(realValue, null)) {
                            SecurityObjectBuilder metadata = securityObjectRepository.GetObjectMetaData(realValue);
                            if(metadata == null) {
                                metadata = new SecurityObjectBuilder();
                                securityObjectRepository.RegisterBuilder(metadata);
                                metadata.SecurityObject = realValue;

                                metadata.CreateRealObject(model, securityObjectRepository);
                            }
                            if(propertyInfo.SetMethod != null) {
                                propertyInfo.SetValue(RealObject, metadata.RealObject);
                            }
                        }
                    }
                }
                else {
                    bool isGeneratedPrimaryKey = false;
                    foreach(IProperty primaryKeyProperty in primaryKeyProperties) {
                        if((propertyInfo.Name == primaryKeyProperty.Name) && primaryKeyProperty.RequiresValueGenerator)
                            isGeneratedPrimaryKey = true;
                    }
                    if(propertyInfo.SetMethod != null && !isGeneratedPrimaryKey) {
                        object securityValue = propertyInfo.GetValue(SecurityObject);
                        propertyInfo.SetValue(RealObject, securityValue);
                    }
                }
            }
            return RealObject;
        }
        public object CreateSecurityObject(IModel model, ISecurityObjectRepository securityObjectRepository) {
            Type targetType = RealObject.GetType();
            SecurityObject = Activator.CreateInstance(RealObject.GetType());
            IEntityType entityType = model.FindEntityType(targetType);
            IEnumerable<PropertyInfo> propertiesInfo = targetType.GetRuntimeProperties();
            IEnumerable<INavigation> navigations = entityType.GetNavigations();
            foreach(PropertyInfo propertyInfo in propertiesInfo) {
                object defaultValue = propertyInfo.GetValue(SecurityObject);
                defaultValueDictionary[propertyInfo.Name] = defaultValue;
                if(this.IsPropertyBlocked(propertyInfo.Name)) {
                    if(navigations.Any(p => p.Name == propertyInfo.Name)) {
                        INavigation navigation = navigations.First(p => p.Name == propertyInfo.Name);
                        if(navigation.IsCollection()) {
                            if(propertyInfo.SetMethod != null) {
                                propertyInfo.SetValue(SecurityObject, null);
                            }
                        }
                    }
                    continue;
                }
                if(navigations.Any(p => p.Name == propertyInfo.Name)) {
                    INavigation navigation = navigations.First(p => p.Name == propertyInfo.Name);
                    if(navigation.IsCollection()) {
                        IClrCollectionAccessor collectionAccessor = navigation.GetCollectionAccessor();
                        IEnumerable objectRealListProperty = (IEnumerable)propertyInfo.GetValue(RealObject);
                        IEnumerable objectSecurityListProperty = (IEnumerable)propertyInfo.GetValue(SecurityObject);
                        List<object> denyObject;
                        BlockedObjectsInListProperty.TryGetValue(propertyInfo.Name, out denyObject);
                        if(objectRealListProperty != null) {
                            foreach(object objInList in objectRealListProperty) {
                                if(denyObject != null && denyObject.Contains(objInList)) {
                                    continue;
                                }
                                object objectToAdd;
                                SecurityObjectBuilder metadata = securityObjectRepository.GetObjectMetaData(objInList);
                                if(metadata != null) {
                                    if(metadata.SecurityObject != null) {
                                        objectToAdd = metadata.SecurityObject;
                                    }
                                    else {
                                        objectToAdd = metadata.CreateSecurityObject(model, securityObjectRepository);
                                    }
                                }
                                else {
                                    throw new Exception();
                                }
                                collectionAccessor.Add(SecurityObject, objectToAdd);
                            } 
                        }
                    }
                    else {
                        object realValue = propertyInfo.GetValue(RealObject);
                        SecurityObjectBuilder metadata = securityObjectRepository.GetObjectMetaData(realValue);
                        if(metadata != null && realValue != null) {
                            if(metadata.SecurityObject == null) {
                                metadata.SecurityObject = metadata.CreateSecurityObject(model, securityObjectRepository);
                            }
                            if(propertyInfo.SetMethod != null) {
                                propertyInfo.SetValue(SecurityObject, metadata.SecurityObject);
                            }
                        }
                        else {
                            if(propertyInfo.SetMethod != null) {
                                propertyInfo.SetValue(SecurityObject, realValue);
                            }
                        }
                    }
                }
                else {
                    if(propertyInfo.SetMethod != null) {
                        object realValue = propertyInfo.GetValue(RealObject);
                        propertyInfo.SetValue(SecurityObject, realValue);
                    }
                }
            }
            foreach(PropertyInfo propertyInfo in propertiesInfo) {
                object originalValue = propertyInfo.GetValue(SecurityObject);
                originalValueSecurityObjectDictionary.Add(propertyInfo.Name, originalValue);
            }

            if(SecurityObject is ISecurityEntity) {
                ISecurityEntity securityEntity = (ISecurityEntity)SecurityObject;

                List<string> blockedMembers = new List<string>();
                blockedMembers.AddRange(BlockedProperties);
                blockedMembers.AddRange(BlockedNavigationProperties);

                securityEntity.BlockedMembers = blockedMembers;
            }

            return SecurityObject;
        }
        public SecurityObjectBuilder() {
        }
    }
}
