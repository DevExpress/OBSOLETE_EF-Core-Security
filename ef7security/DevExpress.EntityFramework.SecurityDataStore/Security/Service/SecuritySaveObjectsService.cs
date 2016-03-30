using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Update;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Security {
    public class SecuritySaveObjectsService {
        private SecurityDbContext securityDbContext;
        private SecurityDbContext realDbContext;
        private ISecurityStrategy security;
        private SecurityObjectRepository securityObjectRepository;
        private IStateManager nativeStateManager;
        private IStateManager securityStateManager;
        public static bool EvaluateInRealDbData { get; set; } = false;
        public SecuritySaveObjectsService(        
            DbContext securityDbContext,
            SecurityObjectRepository securityObjectRepository) {
            this.securityObjectRepository = securityObjectRepository;
            this.securityDbContext = (SecurityDbContext)securityDbContext;
            realDbContext = this.securityDbContext.realDbContext;
            nativeStateManager = realDbContext.GetService<IStateManager>();
            securityStateManager = securityDbContext.GetService<IStateManager>();
            security = this.securityDbContext.Security;
        }
        public int ProcessObject(IEnumerable<EntityEntry> updateEntities) {
            int rowsAffected = 0;
            Dictionary<EntityEntry, Dictionary<string, object>> modifyObjectsProperties = new Dictionary<EntityEntry, Dictionary<string, object>>();
            try {
                foreach(EntityEntry entityEntry in updateEntities) {
                    switch(entityEntry.State) {
                        case EntityState.Deleted:
                            RemoveEntityInRealDbContext(entityEntry);
                            rowsAffected++;
                            break;
                        case EntityState.Modified:
                            modifyObjectsProperties.Add(entityEntry, ModifyEntityInRealDbContext(entityEntry));
                            break;
                        case EntityState.Added:
                           AddingEntityInRealDbContext(entityEntry);
                            rowsAffected++;
                            break;
                    }
                }

                foreach(EntityEntry updateEntry in modifyObjectsProperties.Keys) {
                    switch(updateEntry.State) {
                        case EntityState.Modified:
                            ModifyEntityInRealDbContext(updateEntry, modifyObjectsProperties[updateEntry]);
                            break;
                    }
                }
                foreach(EntityEntry updateEntry in modifyObjectsProperties.Keys) {
                    switch(updateEntry.State) {
                        case EntityState.Modified:
                            CheckEntityInRealDbContext(updateEntry, modifyObjectsProperties[updateEntry]);
                            rowsAffected++;
                            break;
                    }
                }
            }
            catch(Exception e) {
                RollBackChanges(updateEntities);
                throw e;

            }
            return updateEntities.Count();
        }
        #region RollBack
        private void RollBackChanges(IEnumerable<EntityEntry> updateEntities) {
            RollBackModifyObjects();
            RollBackAddedObjects();
            RollBackDeleteObjects();
        }
        private void RollBackDeleteObjects() {
            RollBackRemoveObjects(nativeStateManager);
            RollBackRemoveObjects(securityStateManager);
        }
        private void RollBackRemoveObjects(IStateManager stateManager) {
            foreach(InternalEntityEntry internalEntityEntry in stateManager.Entries.Where(p => p.EntityState == EntityState.Deleted).ToList()) {
                internalEntityEntry.ResetObject();
            }
        }
        private void RollBackAddedObjects() {
            RoollBackAddedObjects(nativeStateManager);
            RoollBackAddedObjects(securityStateManager);
        }
        private void RoollBackAddedObjects(IStateManager stateManager) {
            foreach(InternalEntityEntry internalEntityEntry in stateManager.Entries.Where(p => p.EntityState == EntityState.Added).ToList()) {
                stateManager.StopTracking(internalEntityEntry);
            }
        }             
        private void RollBackModifyObjects() {
            RollBackModifyObjects(nativeStateManager);
            RollBackModifyObjects(securityStateManager);
        }      
        private void RollBackModifyObjects(IStateManager stateManager) {
            foreach(InternalEntityEntry internalEntityEntry in stateManager.Entries.Where(p => p.EntityState == EntityState.Modified).ToList()) {
                internalEntityEntry.ResetObject();
            }
        }
        #endregion

        private void CheckEntityInRealDbContext(EntityEntry entityEntry, Dictionary<string, object> dictionary) {
            Dictionary<string, object> modifyProperties = new Dictionary<string, object>();           
            Type targetType = entityEntry.Metadata.ClrType;
            SecurityObjectMetaData securityObjectMetaData = securityObjectRepository.GetSecurityObjectMetaData(entityEntry.Entity);
            object realObject = securityObjectMetaData.RealObject;
            object securityObject = securityObjectMetaData.SecurityObject;
            bool isGrantedModifyRealObject = security.IsGranted(targetType, SecurityOperation.Write, realObject, null);
            if(!isGrantedModifyRealObject) {
                throw new Exception("Write Deny " + targetType.ToString());
            }
            foreach(PropertyInfo propertyInfo in targetType.GetRuntimeProperties()) {
                string propertyName = propertyInfo.Name;
                object modifiedValue;
                if(dictionary.TryGetValue(propertyName, out modifiedValue)) {
                    if(entityEntry.Metadata.IsSystemProperty(propertyName)) {
                        continue;
                    }
                    bool isGrantedModify = security.IsGranted(targetType, SecurityOperation.Write, realObject, propertyName);
                    if(!isGrantedModify) {
                        throw new Exception("Write Deny: forbidden to write property '" + propertyName + "' was changed. " + targetType.ToString());
                    }
                }
            }
        }

        private Dictionary<string, object> ModifyEntityInRealDbContext(EntityEntry entityEntry, Dictionary<string, object> modifyProperties) {
            Dictionary<string, object> originalsProperties = new Dictionary<string, object>();
            Type targetType = entityEntry.Metadata.ClrType;
            SecurityObjectMetaData securityObjectMetaData = securityObjectRepository.GetSecurityObjectMetaData(entityEntry.Entity);
            object realObject = securityObjectMetaData.RealObject;
            IEnumerable<PropertyInfo> propertiesInfo = targetType.GetRuntimeProperties();
            foreach(string memberName in modifyProperties.Keys) {
                PropertyInfo propertyInfo = propertiesInfo.First(p => p.Name == memberName);
                object setValue = modifyProperties[memberName];
                object realProperyValue = propertyInfo.GetValue(realObject);
                originalsProperties.Add(memberName, realProperyValue);
                propertyInfo.SetValue(realObject, setValue);
            }
            return originalsProperties;
        }

        private Dictionary<string, object> ModifyEntityInRealDbContext(EntityEntry entityEntry) {
            Dictionary<string, object> modifyProperties = new Dictionary<string, object>();            
            Type targetType = entityEntry.Metadata.ClrType;
            SecurityObjectMetaData securityObjectMetaData = securityObjectRepository.GetSecurityObjectMetaData(entityEntry.Entity);
            if(securityObjectMetaData == null) {// update
                securityObjectMetaData = new SecurityObjectMetaData(securityObjectRepository, securityDbContext);
                securityObjectMetaData.RealObject = realDbContext.GetObject(entityEntry.Entity);
                securityObjectMetaData.SecurityObject = entityEntry.Entity;
                securityObjectRepository.RegisterObjects(securityObjectMetaData);
            }

            IEnumerable<INavigation> navigations = entityEntry.Metadata.GetNavigations();
            object realObject = securityObjectMetaData.RealObject;
            object securityObject = securityObjectMetaData.SecurityObject;
            IEnumerable<PropertyInfo> runtimeProperties = targetType.GetRuntimeProperties();
            //IEnumerable<IProperty> shadowProperties = entityEntry.Metadata.GetProperties().Where(d => !runtimeProperties.Any(p => p.Name == d.Name));
            bool isGrantedModifyRealObject = true;
            if(EvaluateInRealDbData) {
                isGrantedModifyRealObject = security.IsGranted(targetType, SecurityOperation.Write, realObject, null);
            }
            if(!isGrantedModifyRealObject) {
                throw new Exception("Write Deny " + targetType.ToString());
            }

            foreach(PropertyInfo propertyInfo in runtimeProperties) {
                string propertyName = propertyInfo.Name;
                object securityPropertyValue = propertyInfo.GetValue(securityObject);
                object realPropertyValue = propertyInfo.GetValue(realObject);

                if(Equals(realPropertyValue, securityPropertyValue)) {
                    continue;
                }
                if(securityObjectMetaData.IsPropertyDeny(propertyName)) {
                    object defaultSecurityValue = securityObjectMetaData.GetDefaultValueForProperty(propertyName);
                    if(securityPropertyValue != defaultSecurityValue && securityPropertyValue != null) {
                        throw new Exception("Write Deny: forbidden to read property '" + propertyName + "' was changed. " + targetType.ToString());
                    }
                    else {
                        continue;
                    }
                }
                if(entityEntry.Metadata.IsSystemProperty(propertyName)) {
                    continue;
                }
                if(navigations.Any(p => p.Name == propertyName)) {
                    INavigation navigation = navigations.First(p => p.Name == propertyName);
                    if(!navigation.IsCollection()) {
                        object defaultNavObjectValue;

                        if(securityObjectMetaData.originalValueSecurityObjectDictionary.TryGetValue(propertyName, out defaultNavObjectValue)) {
                            if(Equals(defaultNavObjectValue, securityPropertyValue)) {
                                continue;
                            }
                        }
                    }
                }
                bool isGrantedModify = true;
                if(EvaluateInRealDbData) {
                    isGrantedModify = security.IsGranted(targetType, SecurityOperation.Write, realObject, propertyName);
                }
                if(!isGrantedModify) {

                    throw new Exception("Write Deny: forbidden to write property '" + propertyName + "' was changed. " + targetType.ToString());
                }
                else {
                    modifyProperties.Add(propertyName, securityPropertyValue);
                }
            }
            return modifyProperties;
        }

        private void RemoveEntityInRealDbContext(EntityEntry entityEntry) {           
            Type targetType = entityEntry.Metadata.ClrType;
            SecurityObjectMetaData securityObjectMetaData = securityObjectRepository.GetSecurityObjectMetaData(entityEntry.Entity);
            if(securityObjectMetaData == null) {
                securityObjectMetaData = new SecurityObjectMetaData(securityObjectRepository, securityDbContext);
                securityObjectMetaData.RealObject = realDbContext.GetObject(entityEntry.Entity);
                securityObjectMetaData.SecurityObject = entityEntry.Entity;
                securityObjectRepository.RegisterObjects(securityObjectMetaData);
            }
            bool isGrantedresult = security.IsGranted(targetType, SecurityOperation.Delete, securityObjectMetaData.RealObject, null);
            if(!isGrantedresult) {
                throw new Exception("Delete Deny " + targetType.ToString());
            }
            else {
                realDbContext.Remove(securityObjectMetaData.RealObject);
            }

        }

        private SecurityObjectMetaData AddingEntityInRealDbContext(EntityEntry entityEntry) {            
            Type targetType = entityEntry.Metadata.ClrType;
            object objectToCreate = entityEntry.Entity;
            bool isGrantedresult = security.IsGranted(targetType, SecurityOperation.Create, objectToCreate, null);
            if(!isGrantedresult) {
                throw new Exception("Create Deny " + targetType.ToString());
            }
            else {
                SecurityObjectMetaData securityObjectMetaData = securityObjectRepository.GetSecurityObjectMetaData(objectToCreate);
                if(securityObjectMetaData == null) {
                    securityObjectMetaData = new SecurityObjectMetaData(securityObjectRepository, securityDbContext);
                    securityObjectMetaData.SecurityObject = objectToCreate;
                    securityObjectRepository.RegisterObjects(securityObjectMetaData);
                    securityObjectMetaData.CreateRealObject();
                }
                realDbContext.Add(securityObjectMetaData.RealObject);
                return securityObjectMetaData;
            }

        }
    }
}
