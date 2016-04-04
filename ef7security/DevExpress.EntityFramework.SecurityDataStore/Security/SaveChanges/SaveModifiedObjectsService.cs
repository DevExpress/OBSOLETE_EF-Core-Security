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
        private SecurityObjectRepository securityObjectRepository;
        public int ProcessObjects(IEnumerable<EntityEntry> entitiesEntry) {
            IEnumerable<ModifyObjectMetada> modifyObjectsMetada = securityDbContext.ChangeTracker.GetModifyObjectMetada();
            ApplyModyfication(entitiesEntry, modifyObjectsMetada);
            CheckModyficationObjects(modifyObjectsMetada);
            return entitiesEntry.Count();
        }

        private void CheckModyficationObjects(IEnumerable<ModifyObjectMetada> modifyObjectsMetada) {
            foreach(ModifyObjectMetada modifyObjectMetada in modifyObjectsMetada) {
                SecurityObjectBuilder securityObjectMetaData = securityObjectRepository.GetSecurityObjectMetaData(modifyObjectMetada.Object);
                Type targetType = securityObjectMetaData.RealObject.GetType();
                foreach(string memberName in modifyObjectMetada.ModifiedProperties.Keys) {
                    CheckModyficationObjects(securityObjectMetaData, targetType, memberName);
                }
                foreach(string memberName in modifyObjectMetada.NavigationProperty) {
                    CheckModyficationObjects(securityObjectMetaData, targetType, memberName);
                }
            }
        }

        private void CheckModyficationObjects(SecurityObjectBuilder securityObjectMetaData, Type targetType, string memberName) {
            bool isGranted = securityDbContext.Security.IsGranted(targetType, SecurityOperation.Write, securityObjectMetaData.RealObject, memberName);
            if(!isGranted) {
                throw new Exception("Write Deny " + targetType.ToString() + "Member name " + memberName);
            }
        }

        private void ApplyModyfication(IEnumerable<EntityEntry> entitiesEntry, IEnumerable<ModifyObjectMetada> modifyObjectsMetada) {
            foreach(var entityEntry in entitiesEntry) {
                ModifyObjectMetada modifyObjectMetada = modifyObjectsMetada.First(p => p.Object == entityEntry.Entity);
                SecurityObjectBuilder securityObjectMetaData = securityObjectRepository.GetSecurityObjectMetaData(entityEntry.Entity);
                if(securityObjectMetaData == null) {
                    securityObjectMetaData = new SecurityObjectBuilder(securityObjectRepository, securityDbContext);
                    securityObjectMetaData.RealObject = securityDbContext.realDbContext.GetObject(entityEntry.Entity);
                    securityObjectMetaData.SecurityObject = entityEntry.Entity;
                    securityObjectRepository.RegisterObjects(securityObjectMetaData);
                }
                ApplyModyfication(securityObjectMetaData.RealObject, modifyObjectMetada);
            }
        }

        private void ApplyModyfication(object realObject, ModifyObjectMetada modifyObjectMetada) {
            ApplyModyfication(realObject, modifyObjectMetada.ModifiedProperties);
            ApplyModyfication(realObject, modifyObjectMetada.ModifiedForeignKey);
        }

        private void ApplyModyfication(object realObject, Dictionary<string, object> modifiedProperties) {
            foreach(string propertyName in modifiedProperties.Keys) {
                EntityEntry realEntity = securityDbContext.realDbContext.ChangeTracker.Entries().First(p => p.Entity == realObject);
                PropertyEntry propertyEntry = realEntity.Property(propertyName);
                propertyEntry.CurrentValue = modifiedProperties[propertyName];
                propertyEntry.IsModified = true;
            }
        }

        public SaveModifiedObjectsService(SecurityDbContext securityDbContext,
            SecurityObjectRepository securityObjectRepository) {
            this.securityDbContext = securityDbContext;
            this.securityObjectRepository = securityObjectRepository;
        }
    }
}
