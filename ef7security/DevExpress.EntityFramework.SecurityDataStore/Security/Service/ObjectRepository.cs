using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Security {
    public class SecurityObjectRepository {
        private List<SecurityModifyObject> resource = new List<SecurityModifyObject>();

        public bool TryRemoveObject(object targetObject) {
            bool result;
            SecurityModifyObject objToRemove = resource.FirstOrDefault(p => p.SecurityObject == targetObject || p.RealObject == targetObject);
            if(objToRemove != null) {
                resource.Remove(objToRemove);
                result = true;
            }
            else {
                result = false;
            }
            return result;
        }
        public List<string> GetBlockedMembers(object securityObject) {
            return resource.FirstOrDefault(p => p.SecurityObject == securityObject)?.BlockedMembers;
        }
        public object GetRealObject(object obj) {
            object realObject = resource.FirstOrDefault(p => p.SecurityObject == obj)?.RealObject;
            if(realObject == null)
                realObject = resource.FirstOrDefault(p => p.RealObject == obj)?.RealObject;
            return realObject;
        }
        public object GetSecurityObject(object obj) {
            object securityObject = resource.FirstOrDefault(p => p.RealObject == obj)?.SecurityObject;
            if(securityObject == null)
                securityObject = resource.FirstOrDefault(p => p.SecurityObject == obj)?.SecurityObject;
            return securityObject;
        }
        public void RegisterObjects(object realObject, object securityObject, List<string> blockedMembers) {
            if(resource.Any(p => p.RealObject == realObject)) {
                resource.Remove(resource.First(p => p.RealObject == realObject));
            }
            SecurityModifyObject securityModifyObject = new SecurityModifyObject();
            securityModifyObject.RealObject = realObject;
            securityModifyObject.SecurityObject = securityObject;
            securityModifyObject.BlockedMembers = blockedMembers;
            resource.Add(securityModifyObject);
        }
        private class SecurityModifyObject {
            public object RealObject { get; set; }
            public object SecurityObject { get; set; }
            public List<string> BlockedMembers { get; set; }
        }
    }
}
