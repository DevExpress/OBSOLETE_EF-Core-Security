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
            RollbackObject(internalEntityEntry.ToEntityEntry());
        }
        public static void RollbackObject(this EntityEntry entityEntry) {
            IEnumerable<IProperty> properties = entityEntry.Metadata.GetProperties().OfType<Property>();
            foreach(var property in properties) {
                PropertyEntry propertyEntry = entityEntry.Property(property.Name);                
                if(propertyEntry.IsModified) {
                    propertyEntry.CurrentValue = propertyEntry.OriginalValue;
                    propertyEntry.IsModified = false;
                }
            }
            entityEntry.State = EntityState.Unchanged;
        }
        public static IEnumerable<PropertyEntry> GetProperties(this EntityEntry entityEntry) {
           return entityEntry.Metadata.GetProperties().Select(p => entityEntry.Property(p.Name));            
        }
        
    }
}
