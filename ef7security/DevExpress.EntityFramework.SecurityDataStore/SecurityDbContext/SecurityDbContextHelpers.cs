using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.EntityFramework.SecurityDataStore.Security;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public class SecurityAccessException : Exception {
        public SecurityAccessException(string message) : base(message) { }
    }
    //public class SecurityCreateException : SecurityException { }
    //public class SecurityReadException : SecurityException { }
    //public class SecurityWriteException : SecurityException { }
    //public class SecurityDeleteException : SecurityException { }
    public static class EntityStateExtensions {
        public static IList<string> GetBlockedMembers(this EntityEntry entityEntry) {
            SecurityDbContext securityDbContext = entityEntry.Context as SecurityDbContext;
            SecurityObjectRepository objectRepository = securityDbContext.GetSecurityObjectRepository();
            object securityObject = objectRepository.GetSecurityObject(entityEntry.Entity);
            IList<string> blockedMembers = objectRepository.GetBlockedMembers(securityObject);
            if(blockedMembers == null)
                blockedMembers = new List<string> { };

            return blockedMembers;
        }
    }
}

