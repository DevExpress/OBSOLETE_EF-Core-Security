using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Security {
    public class SaveAddedObjectsService {
        private BaseSecurityDbContext securityDbContext;
        private ISecurityObjectRepository securityObjectRepository;
        private void AddInRealContext(IEnumerable<SecurityObjectBuilder> securityObjectBuilders) {
            foreach(SecurityObjectBuilder SecurityObjectBuilder in securityObjectBuilders) {
                securityDbContext.RealDbContext.Add(SecurityObjectBuilder.RealObject);
            }
        }
        private IList<BlockedObjectInfo> CheckAddedObjects(IEnumerable<SecurityObjectBuilder> securityObjectBuilders) {
            List<BlockedObjectInfo> blockedList = new List<BlockedObjectInfo>();
            foreach(SecurityObjectBuilder SecurityObjectBuilder in securityObjectBuilders) {
                Type targetType = SecurityObjectBuilder.SecurityObject.GetType();
                object targetObject = SecurityObjectBuilder.SecurityObject;
                bool isGranted = securityDbContext.Security.IsGranted(targetType, SecurityOperation.Create, targetObject);
                if(!isGranted) {
                    blockedList.Add(new BlockedObjectInfo {
                        operationType = BlockedObjectInfo.OperationType.Create,
                        objectType = targetType
                    });
                }
            }
            return blockedList;
        }
        private IEnumerable<SecurityObjectBuilder> PrepareAddedObjects(IEnumerable<EntityEntry> entitiesEntry) {
            List<SecurityObjectBuilder> securityObjectBuilders = new List<SecurityObjectBuilder>();
            foreach(EntityEntry entityEntry in entitiesEntry) {
                SecurityObjectBuilder securityObjectMetaData = securityObjectRepository.GetObjectMetaData(entityEntry.Entity);              
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
        private IEnumerable<BlockedObjectInfo> CheckModifiedNavigations(IEnumerable<ModifiedObjectMetadata> modifyObjectMetadaForAddedObjects) {
            List<BlockedObjectInfo> blockedList = new List<BlockedObjectInfo>();
            foreach(ModifiedObjectMetadata modifyObjectMetada in modifyObjectMetadaForAddedObjects) {
                SecurityObjectBuilder securityObjectMetaData = GetOrCreateBuilder(modifyObjectMetada);
                Type targetType = securityObjectMetaData.RealObject.GetType();
                foreach(var navigationProperty in modifyObjectMetada.NavigationProperties) {
                    bool isGranted = securityDbContext.Security.IsGranted(targetType, SecurityOperation.Write, securityObjectMetaData.RealObject, navigationProperty);
                    if(!isGranted) {
                        blockedList.Add(new BlockedObjectInfo {
                            operationType = BlockedObjectInfo.OperationType.Write,
                            objectType = targetType,
                            memberName = navigationProperty
                        });
                    }
                }
            }
            return blockedList;
        }
        private SecurityObjectBuilder GetOrCreateBuilder(ModifiedObjectMetadata modifyObjectMetada) {
            SecurityObjectBuilder securityObjectMetaData = securityObjectRepository.GetObjectMetaData(modifyObjectMetada.Object);
            if(securityObjectMetaData == null) {
                securityObjectMetaData = new SecurityObjectBuilder();
                securityObjectMetaData.SecurityObject = modifyObjectMetada.Object;
                securityObjectMetaData.RealObject = securityDbContext.RealDbContext.GetObject(modifyObjectMetada.Object);
                securityObjectRepository.RegisterBuilder(securityObjectMetaData);
            }
            return securityObjectMetaData;
        }
        public IList<BlockedObjectInfo> ProcessObjects(IEnumerable<EntityEntry> entitiesEntry) {
            List<BlockedObjectInfo> blockedList = new List<BlockedObjectInfo>();
            IEnumerable<SecurityObjectBuilder> securityObjectBuilders = PrepareAddedObjects(entitiesEntry);
            blockedList.AddRange(CheckAddedObjects(securityObjectBuilders));
            IEnumerable<ModifiedObjectMetadata> modifyObjectMetadaForAddedObjects = securityDbContext.ChangeTracker.GetModifiedObjectMetadaForAddedObjects();
            blockedList.AddRange(CheckModifiedNavigations(modifyObjectMetadaForAddedObjects));
            if(blockedList.Count == 0)
                AddInRealContext(securityObjectBuilders);
            return blockedList;
        }
        public SaveAddedObjectsService(BaseSecurityDbContext securityDbContext,
            ISecurityObjectRepository securityObjectRepository) {
            this.securityDbContext = securityDbContext;
            this.securityObjectRepository = securityObjectRepository;
        }
    }
}
