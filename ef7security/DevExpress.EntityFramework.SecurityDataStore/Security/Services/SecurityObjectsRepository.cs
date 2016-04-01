using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Security {
    public class SecurityObjectRepository {
        public List<SecurityObjectBuilder> resource = new List<SecurityObjectBuilder>();
        public bool TryRemoveObject(object targetObject) {
            bool result;
            SecurityObjectBuilder objToRemove = resource.FirstOrDefault(p => p.SecurityObject == targetObject || p.RealObject == targetObject);
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
            SecurityObjectBuilder securityObjectMetaData = resource.FirstOrDefault(p => p.SecurityObject == securityObject);
            List<string> blockedMembers = new List<string>();
            if(securityObjectMetaData != null) {
                blockedMembers.AddRange(securityObjectMetaData.DenyProperties);
                blockedMembers.AddRange(securityObjectMetaData.DenyNavigationProperties);
            }
            return blockedMembers;
        }
        public SecurityObjectBuilder GetSecurityObjectMetaData(object targetObject) {
            return resource.FirstOrDefault(p => p.SecurityObject == targetObject || p.RealObject == targetObject);
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
        public void RegisterObjects(SecurityObjectBuilder securityObjectMetaData) {
            if(resource.Any(p => p.RealObject == securityObjectMetaData.RealObject)) {
                resource.Remove(resource.First(p => p.RealObject == securityObjectMetaData.RealObject));
            }
            resource.Add(securityObjectMetaData);
        }
    }
}
