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
            RollBackBuilder(securityStateManager);
            RollBackAddedObjects(nativeStateManager);
            RollBackAddedObjects(securityStateManager);

        }

        private void RollBackBuilder(IStateManager securityStateManager) {
            foreach(InternalEntityEntry internalEntityEntry in securityStateManager.Entries.Where(p => p.EntityState == EntityState.Added).ToList()) {
                securityObjectRepository.TryRemoveObject(internalEntityEntry.Entity);
            }
        }

        private void RollBackAddedObjects(IStateManager stateManager) {
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
                stateManager.StopTracking(internalEntityEntry);
                InternalEntityEntry entry = stateManager.GetOrCreateEntry(internalEntityEntry.Entity);
                stateManager.StartTracking(entry);
                entry.SetEntityState(EntityState.Unchanged);
            }
        }
        #endregion
        public SecuritySaveObjectsService(
          DbContext securityDbContext,
          SecurityObjectRepository securityObjectRepository) {
            this.securityObjectRepository = securityObjectRepository;
            this.securityDbContext = (SecurityDbContext)securityDbContext;
            realDbContext = this.securityDbContext.realDbContext;
            nativeStateManager = realDbContext.GetService<IStateManager>();
            securityStateManager = securityDbContext.GetService<IStateManager>();
            security = this.securityDbContext.Security;
            saveAddedObjectsService = new SaveAddedObjectsService(this.securityDbContext, securityObjectRepository);
            saveRemovedObjectsService = new SaveRemovedObjectsService(this.securityDbContext, securityObjectRepository);
            saveModifyObjectsService = new SaveModifiedObjectsService(this.securityDbContext, securityObjectRepository);
            trackPrimaryKeyService = new TrackPrimaryKeyService(this.securityDbContext, securityObjectRepository);
        }
    }
}
