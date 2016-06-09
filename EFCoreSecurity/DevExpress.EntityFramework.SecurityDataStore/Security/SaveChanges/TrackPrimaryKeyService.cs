using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DevExpress.EntityFramework.SecurityDataStore.Security {
    public class TrackPrimaryKeyService {
        private readonly BaseSecurityDbContext securityDbContext;
        private readonly ISecurityObjectRepository securityObjectRepository;
        public void ApplyChanges(IEnumerable<EntityEntry> updateEntities) {            
            foreach (EntityEntry securityEntityEntry in updateEntities) {
                object realObject =  securityObjectRepository.GetRealObject(securityEntityEntry.Entity);
                EntityEntry realEntityEntry = securityDbContext.RealDbContext.ChangeTracker.GetEntity(realObject);
                if (realEntityEntry == null) {
                    continue;
                }
                ApplyChanges(securityEntityEntry, realEntityEntry);
            }
        }

        private void ApplyChanges(EntityEntry securityEntityEntry, EntityEntry realEntityEntry) {
            IEnumerable<PropertyEntry> securityProperties = securityEntityEntry.GetProperties();
            IEnumerable<PropertyEntry> realProperties = realEntityEntry.GetProperties();
            foreach(PropertyEntry propertyEntry in securityProperties) {
                if(!propertyEntry.Metadata.IsKey()) {
                    continue;
                }
                PropertyEntry securityPropertyEntry = propertyEntry;
                PropertyEntry realPropertyEntry = realProperties.First(p => p.Metadata.Name == securityPropertyEntry.Metadata.Name);
                ApplyChanges(securityPropertyEntry, realPropertyEntry);
            }
        }

        private void ApplyChanges(PropertyEntry securityPropertyEntry, PropertyEntry realPropertyEntry) {
            if(!Equals(securityPropertyEntry.CurrentValue, realPropertyEntry.CurrentValue)) {
                securityPropertyEntry.CurrentValue = realPropertyEntry.CurrentValue;
                securityPropertyEntry.OriginalValue = realPropertyEntry.CurrentValue;
                securityPropertyEntry.IsModified = false;
            }
        }

        public TrackPrimaryKeyService(BaseSecurityDbContext securityDbContext, ISecurityObjectRepository securityObjectRepository) {
            this.securityDbContext = securityDbContext;
            this.securityObjectRepository = securityObjectRepository;
        }
    }
}
