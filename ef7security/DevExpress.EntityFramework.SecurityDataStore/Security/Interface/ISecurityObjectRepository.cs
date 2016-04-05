using DevExpress.EntityFramework.SecurityDataStore.Security;
using System.Collections.Generic;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public interface ISecurityObjectRepository {
        bool RemoveBuilder(SecurityObjectBuilder targetObject);
        void RegisterBuilder(SecurityObjectBuilder securityObjectMetaData);
        IEnumerable<SecurityObjectBuilder> GetAllBuilders();
    }
}