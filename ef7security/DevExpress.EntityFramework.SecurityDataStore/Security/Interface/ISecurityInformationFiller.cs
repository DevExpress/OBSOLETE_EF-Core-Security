using DevExpress.EntityFramework.SecurityDataStore.Security;
using System.Collections.Generic;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public interface ISecurityInformationFiller {
        void FillSecurityInformation(IEnumerable<SecurityObjectBuilder> objectBuilders);
    }
}