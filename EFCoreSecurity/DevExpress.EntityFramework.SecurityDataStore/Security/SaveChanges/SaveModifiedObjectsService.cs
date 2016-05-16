using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;

namespace DevExpress.EntityFramework.SecurityDataStore.Security {
    public class SaveModifiedObjectsService {
        private SecurityDbContext securityDbContext;
        private ISecurityObjectRepository securityObjectRepository;
        public IList<BlockedObjectInfo> ProcessObjects(IEnumerable<EntityEntry> entitiesEntry) {
            IEnumerable<ModifiedObjectMetada> modifyObjectsMetada = securityDbContext.ChangeTracker.GetModifyObjectMetada();
            ApplyModification(entitiesEntry, modifyObjectsMetada);
            IList<BlockedObjectInfo> blockedList = CheckModificationObjects(modifyObjectsMetada);
            // if(blockedList.Count == 0)
                
            
            return blockedList;
        }
        private IList<BlockedObjectInfo> CheckModificationObjects(IEnumerable<ModifiedObjectMetada> modifyObjectsMetada) {
            List<BlockedObjectInfo> blockedList = new List<BlockedObjectInfo>();
            foreach(ModifiedObjectMetada modifyObjectMetada in modifyObjectsMetada) {
                SecurityObjectBuilder securityObjectMetaData = securityObjectRepository.GetSecurityObjectMetaData(modifyObjectMetada.Object);
                Type targetType = securityObjectMetaData.RealObject.GetType();
                
                foreach(string memberName in modifyObjectMetada.Properties.Keys)
                    blockedList.AddRange(CheckModificationObjects(securityObjectMetaData, targetType, memberName));
                foreach(string memberName in modifyObjectMetada.NavigationProperties)
                    blockedList.AddRange(CheckModificationObjects(securityObjectMetaData, targetType, memberName));
            }

            return blockedList;
        }
        private IList<BlockedObjectInfo> CheckModificationObjects(SecurityObjectBuilder securityObjectMetaData, Type targetType, string memberName) {
            List<BlockedObjectInfo> blockedList = new List<BlockedObjectInfo>();
            bool isGranted = securityDbContext.Security.IsGranted(targetType, SecurityOperation.Write, securityObjectMetaData.RealObject, memberName);
            if(!isGranted) {
                // throw new SecurityAccessException("Write Deny " + targetType.ToString() + "Member name " + memberName);
                blockedList.Add(new BlockedObjectInfo {
                    operationType = BlockedObjectInfo.OperationType.Write,
                    objectType = targetType,
                    memberName = memberName
                });
            }
            return blockedList;
        }
        private void ApplyModification(IEnumerable<EntityEntry> entitiesEntry, IEnumerable<ModifiedObjectMetada> modifyObjectsMetada) {
            foreach(var entityEntry in entitiesEntry) {
                ModifiedObjectMetada modifyObjectMetada = modifyObjectsMetada.First(p => p.Object == entityEntry.Entity);
                SecurityObjectBuilder securityObjectMetaData = securityObjectRepository.GetSecurityObjectMetaData(entityEntry.Entity);
                if(securityObjectMetaData == null) {
                    securityObjectMetaData = new SecurityObjectBuilder();
                    securityObjectMetaData.RealObject = securityDbContext.realDbContext.GetObject(entityEntry.Entity);
                    securityObjectMetaData.SecurityObject = entityEntry.Entity;
                    securityObjectRepository.RegisterBuilder(securityObjectMetaData);
                }
                ApplyModyfication(securityObjectMetaData.RealObject, modifyObjectMetada);
            }
        }
        private void ApplyModyfication(object realObject, ModifiedObjectMetada modifyObjectMetada) {
            ApplyModification(realObject, modifyObjectMetada.Properties);
            ApplyModification(realObject, modifyObjectMetada.ForeignKeys);
        }
        private void ApplyModification(object realObject, Dictionary<string, object> modifiedProperties) {
            foreach(string propertyName in modifiedProperties.Keys) {
                EntityEntry realEntity = securityDbContext.realDbContext.ChangeTracker.Entries().First(p => p.Entity == realObject);
                PropertyEntry propertyEntry = realEntity.Property(propertyName);
                propertyEntry.CurrentValue = modifiedProperties[propertyName];
                propertyEntry.IsModified = true;
            }
        }
        public SaveModifiedObjectsService(SecurityDbContext securityDbContext,
            ISecurityObjectRepository securityObjectRepository) {
            this.securityDbContext = securityDbContext;
            this.securityObjectRepository = securityObjectRepository;
        }
    }
}
