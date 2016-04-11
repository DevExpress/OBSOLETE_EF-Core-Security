using DevExpress.EntityFramework.SecurityDataStore.Security;
using System.Collections.Generic;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public interface IFillSecurityObjects {
        void FillObjects(IEnumerable<SecurityObjectBuilder> SecurityObjectBuilders);
    }
}