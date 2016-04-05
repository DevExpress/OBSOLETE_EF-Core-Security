using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Security {
    public class SaveAddedObjectsService {
        private SecurityDbContext securityDbContext;
        private ISecurityObjectRepository securityObjectRepository;
        private void AddingInRealContext(IEnumerable<SecurityObjectBuilder> securityObjectBuilders) {
            foreach(SecurityObjectBuilder SecurityObjectBuilder in securityObjectBuilders) {
                securityDbContext.realDbContext.Add(SecurityObjectBuilder.RealObject);
            }
        }
        private void CheckAddedObjects(IEnumerable<SecurityObjectBuilder> securityObjectBuilders) {
            foreach(SecurityObjectBuilder SecurityObjectBuilder in securityObjectBuilders) {
                Type targetType = SecurityObjectBuilder.SecurityObject.GetType();
                object targetObject = SecurityObjectBuilder.SecurityObject;
                bool isGranted = securityDbContext.Security.IsGranted(targetType, SecurityOperation.Create, targetObject, null);
                if(!isGranted) {
                    throw new Exception("Create Deny " + targetType.ToString());
                }
            }
        }
        private IEnumerable<SecurityObjectBuilder> PrepareAddedObjects(IEnumerable<EntityEntry> entitiesEntry) {
            List<SecurityObjectBuilder> securityObjectBuilders = new List<SecurityObjectBuilder>();
            foreach(EntityEntry entityEntry in entitiesEntry) {
                SecurityObjectBuilder securityObjectMetaData = securityObjectRepository.GetSecurityObjectMetaData(entityEntry.Entity);              
                if(securityObjectMetaData == null) {
                    securityObjectMetaData = new SecurityObjectBuilder();
                    securityObjectMetaData.SecurityObject = entityEntry.Entity;
                    securityObjectRepository.RegisterBuilder(securityObjectMetaData);
                    securityObjectMetaData.CreateRealObject(securityDbContext.Model, securityObjectRepository);
                }
                securityObjectBuilders.Add(securityObjectMetaData);
            }
            return securityObjectBuilders;
        }
        private void CheckModifiedNavigations(IEnumerable<ModifyObjectMetada> modifyObjectMetadaForAddedObjects) {
            foreach(ModifyObjectMetada modifyObjectMetada in modifyObjectMetadaForAddedObjects) {
                SecurityObjectBuilder securityObjectMetaData = GetOrCreateBuilde(modifyObjectMetada);
                Type targetType = securityObjectMetaData.RealObject.GetType();
                foreach(var denyNavigation in modifyObjectMetada.NavigationProperty) {
                    bool isGranted = securityDbContext.Security.IsGranted(targetType, SecurityOperation.Write, securityObjectMetaData.RealObject, denyNavigation);
                    if(!isGranted) {
                        throw new Exception("Write Deny " + targetType.ToString() + "Member name " + denyNavigation);
                    }
                }
            }
        }

        private SecurityObjectBuilder GetOrCreateBuilde(ModifyObjectMetada modifyObjectMetada) {
            SecurityObjectBuilder securityObjectMetaData = securityObjectRepository.GetSecurityObjectMetaData(modifyObjectMetada.Object);
            if(securityObjectMetaData == null) {
                securityObjectMetaData = new SecurityObjectBuilder();
                securityObjectMetaData.SecurityObject = modifyObjectMetada.Object;
                securityObjectMetaData.RealObject = securityDbContext.realDbContext.GetObject(modifyObjectMetada.Object);
                securityObjectRepository.RegisterBuilder(securityObjectMetaData);
            }

            return securityObjectMetaData;
        }

        public int ProcessObjects(IEnumerable<EntityEntry> entitiesEntry) {
            IEnumerable<SecurityObjectBuilder> SecurityObjectBuilders = PrepareAddedObjects(entitiesEntry);
            CheckAddedObjects(SecurityObjectBuilders);
            IEnumerable<ModifyObjectMetada> modifyObjectMetadaForAddedObjects = securityDbContext.ChangeTracker.GetModifyObjectMetadaForAddedObjects();
            CheckModifiedNavigations(modifyObjectMetadaForAddedObjects);
            AddingInRealContext(SecurityObjectBuilders);
            return SecurityObjectBuilders.Count();
        }


        public SaveAddedObjectsService(SecurityDbContext securityDbContext,
            ISecurityObjectRepository securityObjectRepository) {
            this.securityDbContext = securityDbContext;
            this.securityObjectRepository = securityObjectRepository;
        }
    }
}
