using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Security {
    public class SaveRemovedObjectsService {
        private SecurityDbContext securityDbContext;
        private ISecurityObjectRepository securityObjectRepository;        
        private void AddRemovedObjectsInRealContext(IEnumerable<SecurityObjectBuilder> securityObjectBuilders) {
            foreach(SecurityObjectBuilder SecurityObjectBuilder in securityObjectBuilders) {
                securityDbContext.realDbContext.Remove(SecurityObjectBuilder.RealObject);
            }
        }
        private void CheckRemovedObjects(IEnumerable<SecurityObjectBuilder> securityObjectBuilders) {
            foreach(SecurityObjectBuilder SecurityObjectBuilder in securityObjectBuilders) {
                Type targetType = SecurityObjectBuilder.SecurityObject.GetType();
                object targetObject = SecurityObjectBuilder.RealObject;
                bool isGranted = securityDbContext.Security.IsGranted(targetType, SecurityOperation.Delete, targetObject, null);
                if(!isGranted) {
                    throw new Exception("Delete Deny " + targetType.ToString());
                }
            }
        }
        private IEnumerable<SecurityObjectBuilder> PrepareRemovedObjects(IEnumerable<EntityEntry> entitiesEntry) {
            List<SecurityObjectBuilder> securityObjectBuilders = new List<SecurityObjectBuilder>();
            foreach(EntityEntry entityEntry in entitiesEntry) {
                SecurityObjectBuilder securityObjectMetaData = securityObjectRepository.GetSecurityObjectMetaData(entityEntry.Entity);
                if(securityObjectMetaData == null) {
                    securityObjectMetaData = new SecurityObjectBuilder();
                    securityObjectMetaData.RealObject = securityDbContext.realDbContext.GetObject(entityEntry.Entity);
                    securityObjectMetaData.SecurityObject = entityEntry.Entity;
                    securityObjectRepository.RegisterBuilder(securityObjectMetaData);
                }
                securityObjectBuilders.Add(securityObjectMetaData);
            }
            return securityObjectBuilders;
        }
        public int ProcessObjects(IEnumerable<EntityEntry> entitiesEntry) {
            IEnumerable<SecurityObjectBuilder> SecurityObjectBuilders = PrepareRemovedObjects(entitiesEntry);
            CheckRemovedObjects(SecurityObjectBuilders);
            AddRemovedObjectsInRealContext(SecurityObjectBuilders);
            return SecurityObjectBuilders.Count();
        }
        public SaveRemovedObjectsService(SecurityDbContext securityDbContext,
         ISecurityObjectRepository securityObjectRepository) {
            this.securityDbContext = securityDbContext;
            this.securityObjectRepository = securityObjectRepository;

        }
    }
}
