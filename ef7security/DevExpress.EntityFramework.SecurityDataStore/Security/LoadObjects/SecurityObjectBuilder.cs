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
        public Dictionary<string, List<object>> DenyObjectsInListProperty { get; set; }
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
            foreach(PropertyInfo propertyInfo in properiesInfo) {
                object defaultValue = propertyInfo.GetValue(RealObject);
                defaultValueDictionary[propertyInfo.Name] = defaultValue;
                if(navigations.Any(p => p.Name == propertyInfo.Name)) {
                    INavigation navigation = navigations.First(p => p.Name == propertyInfo.Name);
                    if(navigation.IsCollection()) {
                        IClrCollectionAccessor collectionAccessor = navigation.GetCollectionAccessor();
                        IEnumerable objectRealListProperty = (IEnumerable)propertyInfo.GetValue(RealObject);
                        IEnumerable objectSecurityListProperty = (IEnumerable)propertyInfo.GetValue(SecurityObject);
                        if(objectSecurityListProperty != null && objectRealListProperty != null) {
                            foreach(object objInList in objectSecurityListProperty) {
                                SecurityObjectBuilder securityObjectMetaDataObj = securityObjectRepository.GetSecurityObjectMetaData(objInList);
                                if(securityObjectMetaDataObj == null) {
                                    securityObjectMetaDataObj = new SecurityObjectBuilder();
                                    securityObjectRepository.RegisterBuilder(securityObjectMetaDataObj);
                                    securityObjectMetaDataObj.SecurityObject = objInList;
                                    securityObjectMetaDataObj.CreateRealObject(model, securityObjectRepository);
                                }
                                collectionAccessor.Add(RealObject, securityObjectMetaDataObj.RealObject);
                            }
                        }
                    }
                    else {
                        object realValue = propertyInfo.GetValue(SecurityObject);
                        if(!Equals(realValue, null)) {
                            SecurityObjectBuilder securityObjectMetaDataObj = securityObjectRepository.GetSecurityObjectMetaData(realValue);
                            if(securityObjectMetaDataObj == null) {
                                securityObjectMetaDataObj = new SecurityObjectBuilder();
                                securityObjectRepository.RegisterBuilder(securityObjectMetaDataObj);
                                securityObjectMetaDataObj.SecurityObject = realValue;

                                securityObjectMetaDataObj.CreateRealObject(model, securityObjectRepository);
                            }
                            if(propertyInfo.SetMethod != null) {
                                propertyInfo.SetValue(RealObject, securityObjectMetaDataObj.RealObject);
                            }
                        }
                    }
                }
                else {
                    if(propertyInfo.SetMethod != null) {
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
            IEnumerable<PropertyInfo> properiesInfo = targetType.GetRuntimeProperties();
            IEnumerable<INavigation> navigations = entityType.GetNavigations();
            foreach(PropertyInfo propertyInfo in properiesInfo) {
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
                        DenyObjectsInListProperty.TryGetValue(propertyInfo.Name, out denyObject);
                        if(objectRealListProperty != null) {
                            foreach(object objInList in objectRealListProperty) {
                                if(denyObject != null && denyObject.Contains(objInList)) {
                                    continue;
                                }
                                object objectToAdding;
                                SecurityObjectBuilder ModifyObjectInListMetaInfo = securityObjectRepository.GetSecurityObjectMetaData(objInList);
                                if(ModifyObjectInListMetaInfo != null) {
                                    if(ModifyObjectInListMetaInfo.SecurityObject != null) {
                                        objectToAdding = ModifyObjectInListMetaInfo.SecurityObject;
                                    }
                                    else {
                                        objectToAdding = ModifyObjectInListMetaInfo.CreateSecurityObject(model, securityObjectRepository);
                                    }
                                }
                                else {
                                    throw new Exception();
                                }
                                collectionAccessor.Add(SecurityObject, objectToAdding);
                            } 
                        }
                    }
                    else {
                        object realValue = propertyInfo.GetValue(RealObject);
                        SecurityObjectBuilder securityObjectMetaDataObj = securityObjectRepository.GetSecurityObjectMetaData(realValue);
                        if(securityObjectMetaDataObj != null && realValue != null) {
                            if(securityObjectMetaDataObj.SecurityObject == null) {
                                securityObjectMetaDataObj.SecurityObject = securityObjectMetaDataObj.CreateSecurityObject(model, securityObjectRepository);
                            }
                            if(propertyInfo.SetMethod != null) {
                                propertyInfo.SetValue(SecurityObject, securityObjectMetaDataObj.SecurityObject);
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
            foreach(PropertyInfo propertyInfo in properiesInfo) {
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
