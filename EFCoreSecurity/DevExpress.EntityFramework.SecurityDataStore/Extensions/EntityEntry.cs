using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public static class EntityEntryExtensions {
        public static void RollbackObject(this InternalEntityEntry internalEntityEntry) {
            IEnumerable<IProperty> properties = internalEntityEntry.EntityType.GetProperties().OfType<Property>();
            foreach(var property in properties) {
                PropertyEntry propertyEntry = internalEntityEntry.ToEntityEntry().Property(property.Name);
                if(propertyEntry.IsModified) {
                    internalEntityEntry[property] = propertyEntry.OriginalValue;
                    propertyEntry.CurrentValue = propertyEntry.OriginalValue;
                    propertyEntry.IsModified = false;
                }
            }
            internalEntityEntry.SetEntityState(EntityState.Unchanged, true);
        }
      
        public static IEnumerable<PropertyEntry> GetProperties(this EntityEntry entityEntry) {
           return entityEntry.Metadata.GetProperties().Select(p => entityEntry.Property(p.Name));            
        }
        
    }
}
