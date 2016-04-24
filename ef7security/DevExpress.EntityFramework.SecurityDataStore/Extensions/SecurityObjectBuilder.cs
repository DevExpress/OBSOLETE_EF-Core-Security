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
                blockedMembers.AddRange(securityObjectBuilder.BlockedProperties);
                blockedMembers.AddRange(securityObjectBuilder.BlockedNavigationProperties);
            }
            blockedMembers.RemoveAll(p => string.IsNullOrWhiteSpace(p));
            return string.Join(";", blockedMembers);
        }
        public static bool NeedToModify(this SecurityObjectBuilder securityObjectBuilder) {
            bool result = false;
            result = securityObjectBuilder.BlockedProperties.Count > 0;
            if(result == false) {
                result = securityObjectBuilder.BlockedNavigationProperties.Count > 0;
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
        public static bool IsPropertyBlocked(this SecurityObjectBuilder securityObjectBuilder, string propertyName) {
            bool result = true;
            result = securityObjectBuilder.BlockedProperties.Contains(propertyName);
            if(!result) {
                result = securityObjectBuilder.BlockedNavigationProperties.Contains(propertyName);
            }
            return result;
        }
    }
}
