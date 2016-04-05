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
    public class SecuritySaveObjectsService : ISecuritySaveObjects {
        private SecurityDbContext securityDbContext;
        private DbContext realDbContext;
        private ISecurityObjectRepository securityObjectRepository;
        private SaveAddedObjectsService saveAddedObjectsService;
        private SaveRemovedObjectsService saveRemovedObjectsService;
        private SaveModifiedObjectsService saveModifyObjectsService;
        private TrackPrimaryKeyService trackPrimaryKeyService;
        public static bool EvaluateInRealDbData { get; set; } = false;
        public int ProcessObject(IEnumerable<EntityEntry> updateEntities) {
            try {
                saveAddedObjectsService.ProcessObjects(updateEntities.Where(p => p.State == EntityState.Added));
                saveRemovedObjectsService.ProcessObjects(updateEntities.Where(p => p.State == EntityState.Deleted));
                saveModifyObjectsService.ProcessObjects(updateEntities.Where(p => p.State == EntityState.Modified));
                securityDbContext.realDbContext.SaveChanges();
                trackPrimaryKeyService.ApplyChanges(updateEntities);
            }
            catch(Exception e) {
                RollBackChanges(updateEntities);
                throw e;

            }
            return updateEntities.Count();
        }
        private void RollBackChanges(IEnumerable<EntityEntry> updateEntities) {
            RollBackBuilder(securityDbContext.ChangeTracker.GetInfrastructure());
            RollBackBuilder(realDbContext.ChangeTracker.GetInfrastructure());
            realDbContext.ChangeTracker.RollBackChanges();
            securityDbContext.ChangeTracker.RollBackChanges();
        }
        private void RollBackBuilder(IStateManager securityStateManager) {
            foreach(InternalEntityEntry internalEntityEntry in securityStateManager.Entries.Where(p => p.EntityState == EntityState.Added).ToList()) {
                securityObjectRepository.TryRemoveObject(internalEntityEntry.Entity);
            }
        }
        public SecuritySaveObjectsService(
          SecurityDbContext securityDbContext,
          ISecurityObjectRepository securityObjectRepository) {
            this.securityObjectRepository = securityObjectRepository;
            this.securityDbContext = securityDbContext;
            realDbContext = this.securityDbContext.realDbContext;            
            saveAddedObjectsService = new SaveAddedObjectsService(this.securityDbContext, securityObjectRepository);
            saveRemovedObjectsService = new SaveRemovedObjectsService(this.securityDbContext, securityObjectRepository);
            saveModifyObjectsService = new SaveModifiedObjectsService(this.securityDbContext, securityObjectRepository);
            trackPrimaryKeyService = new TrackPrimaryKeyService(this.securityDbContext, securityObjectRepository);
        }
    }
}
