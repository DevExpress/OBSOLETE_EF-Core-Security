using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Security {
    public class SecurityObjectRepository : ISecurityObjectRepository {
        public List<SecurityObjectBuilder> resource = new List<SecurityObjectBuilder>();
        public void RegisterBuilder(SecurityObjectBuilder securityObjectMetaData) {
            if(resource.Any(p => p.RealObject == securityObjectMetaData.RealObject)) {
                resource.Remove(resource.First(p => p.RealObject == securityObjectMetaData.RealObject));
            }
            resource.Add(securityObjectMetaData);
        }
        public bool RemoveBuilder(SecurityObjectBuilder builder) {
            return resource.Remove(builder);
        }
        public IEnumerable<SecurityObjectBuilder> GetAllBuilders() {
            return resource.ToArray();
        }
    }
    
}
