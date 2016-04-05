using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public static class RollBackExtension {
        public static void RollBackChanges(this ChangeTracker changeTracker) {
            IEnumerable<EntityEntry> entities = changeTracker.Entries().Where(p =>
      p.State == EntityState.Added ||
       p.State == EntityState.Deleted ||
        p.State == EntityState.Modified);
            RollBackChanges(changeTracker, entities);
        }

        private static void RollBackChanges(ChangeTracker changeTracker, IEnumerable<EntityEntry> updateEntities) {
            IStateManager stateManager = ((IInfrastructure<IStateManager>)changeTracker).Instance;
            RollBackModifyObjects(stateManager);
            RollBackAddedObjects(stateManager);
            RollBackRemoveObjects(stateManager);
        }

        private static void RollBackRemoveObjects(IStateManager stateManager) {
            foreach(InternalEntityEntry internalEntityEntry in stateManager.Entries.Where(p => p.EntityState == EntityState.Deleted).ToList()) {
                internalEntityEntry.ResetObject();
            }
        }

        private static void RollBackAddedObjects(IStateManager stateManager) {
            foreach(InternalEntityEntry internalEntityEntry in stateManager.Entries.Where(p => p.EntityState == EntityState.Added).ToList()) {
                stateManager.StopTracking(internalEntityEntry);
            }
        }

        private static void RollBackModifyObjects(IStateManager stateManager) {
            foreach(InternalEntityEntry internalEntityEntry in stateManager.Entries.Where(p => p.EntityState == EntityState.Modified).ToList()) {
                internalEntityEntry.ResetObject();
                stateManager.StopTracking(internalEntityEntry);
                InternalEntityEntry entry = stateManager.GetOrCreateEntry(internalEntityEntry.Entity);
                stateManager.StartTracking(entry);
                entry.SetEntityState(EntityState.Unchanged);
            }
        }
    }
}
