using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;

namespace DevExpress.EntityFramework.SecurityDataStore.Security {
    public interface ISecuritySaveObjects {
        int ProcessObject(IEnumerable<EntityEntry> updateEntities);
    }
}