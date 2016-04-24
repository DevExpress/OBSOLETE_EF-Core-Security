using DevExpress.EntityFramework.SecurityDataStore.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public static class SecurityObjectBuilderExtensions {
        public static string GetBlockedMembers(this SecurityObjectBuilder securityObjectBuilder) {       
            List<string> blockedMembers = new List<string>();
            if(securityObjectBuilder != null) {
                blockedMembers.AddRange(securityObjectBuilder.DenyProperties);
                blockedMembers.AddRange(securityObjectBuilder.DenyNavigationProperties);
            }
            blockedMembers.RemoveAll(p => string.IsNullOrWhiteSpace(p));
            return string.Join(";", blockedMembers);
        }
        public static bool NeedToModify(this SecurityObjectBuilder securityObjectBuilder) {
            bool result = false;
            result = securityObjectBuilder.DenyProperties.Count > 0;
            if(result == false) {
                result = securityObjectBuilder.DenyNavigationProperties.Count > 0;
            }
            if(!result) {
                foreach(string propertyName in securityObjectBuilder.DenyObjectsInListProperty.Keys) {
                    List<object> denyObjectInList = securityObjectBuilder.DenyObjectsInListProperty[propertyName];
                    if(denyObjectInList.Count > 0) {
                        result = true;
                        break;
                    }
                }
            }
            if(!result) {
                foreach(string propertyName in securityObjectBuilder.ModifyObjectsInListProperty.Keys) {
                    List<SecurityObjectBuilder> modifyObjectInList = securityObjectBuilder.ModifyObjectsInListProperty[propertyName];
                    if(modifyObjectInList.Count > 0) {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }
        public static bool IsPropertyDenied(this SecurityObjectBuilder securityObjectBuilder, string propertyName) {
            bool result = true;
            result = securityObjectBuilder.DenyProperties.Contains(propertyName);
            if(!result) {
                result = securityObjectBuilder.DenyNavigationProperties.Contains(propertyName);
            }
            return result;
        }
    }
}
