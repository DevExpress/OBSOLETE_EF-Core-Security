using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Security {
    public class SaveRemovedObjectsService {
        private BaseSecurityDbContext securityDbContext;
        private ISecurityObjectRepository securityObjectRepository;        
        private void AddRemovedObjectsInRealContext(IEnumerable<SecurityObjectBuilder> securityObjectBuilders) {
            foreach(SecurityObjectBuilder SecurityObjectBuilder in securityObjectBuilders) {
                securityDbContext.RealDbContext.Remove(SecurityObjectBuilder.RealObject);
            }
        }
        private IList<BlockedObjectInfo> CheckRemovedObjects(IEnumerable<SecurityObjectBuilder> securityObjectBuilders) {
            List<BlockedObjectInfo> blockedList = new List<BlockedObjectInfo>();
            foreach(SecurityObjectBuilder SecurityObjectBuilder in securityObjectBuilders) {
                Type targetType = SecurityObjectBuilder.SecurityObject.GetType();
                object targetObject = SecurityObjectBuilder.RealObject;
                bool isGranted = securityDbContext.Security.IsGranted(targetType, SecurityOperation.Delete, targetObject);
                if(!isGranted) {
                    // throw new SecurityAccessException("Delete Deny " + targetType.ToString());
                    blockedList.Add(new BlockedObjectInfo {
                        operationType = BlockedObjectInfo.OperationType.Delete,
                        objectType = targetType
                    });
                }
            }
            return blockedList;
        }
        private IEnumerable<SecurityObjectBuilder> PrepareRemovedObjects(IEnumerable<EntityEntry> entitiesEntry) {
            List<SecurityObjectBuilder> securityObjectBuilders = new List<SecurityObjectBuilder>();
            foreach(EntityEntry entityEntry in entitiesEntry) {
                SecurityObjectBuilder securityObjectMetaData = securityObjectRepository.GetObjectMetaData(entityEntry.Entity);
                if(securityObjectMetaData == null) {
                    securityObjectMetaData = new SecurityObjectBuilder();
                    securityObjectMetaData.RealObject = securityDbContext.RealDbContext.GetObject(entityEntry.Entity);
                    securityObjectMetaData.SecurityObject = entityEntry.Entity;
                    securityObjectRepository.RegisterBuilder(securityObjectMetaData);
                }
                securityObjectBuilders.Add(securityObjectMetaData);
            }
            return securityObjectBuilders;
        }
        public IList<BlockedObjectInfo> ProcessObjects(IEnumerable<EntityEntry> entitiesEntry) {
            IEnumerable<SecurityObjectBuilder> securityObjectBuilders = PrepareRemovedObjects(entitiesEntry);
            IList<BlockedObjectInfo> blockedList = CheckRemovedObjects(securityObjectBuilders);
            if(blockedList.Count == 0)
                AddRemovedObjectsInRealContext(securityObjectBuilders);

            return blockedList;
        }
        public SaveRemovedObjectsService(BaseSecurityDbContext securityDbContext,
         ISecurityObjectRepository securityObjectRepository) {
            this.securityDbContext = securityDbContext;
            this.securityObjectRepository = securityObjectRepository;
        }
    }
}
