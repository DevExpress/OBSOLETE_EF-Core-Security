using DevExpress.EntityFramework.SecurityDataStore.Security;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public static class SecurityObjectRepositoryExtensions {
        public static bool TryRemoveObject(this ISecurityObjectRepository securityObjectRepository, object targetObject) {
            bool result;
            IEnumerable<SecurityObjectBuilder> resource = securityObjectRepository.GetAllBuilders();
            SecurityObjectBuilder objToRemove = resource.FirstOrDefault(p => p.SecurityObject == targetObject || p.RealObject == targetObject);
            if(objToRemove != null) {
                securityObjectRepository.RemoveBuilder(objToRemove);
                result = true;
            }
            else {
                result = false;
            }
            return result;
        }
        public static List<string> GetBlockedMembers(this ISecurityObjectRepository securityObjectRepository, object securityObject) {
            IEnumerable<SecurityObjectBuilder> resource = securityObjectRepository.GetAllBuilders();
            SecurityObjectBuilder securityObjectMetaData = resource.FirstOrDefault(p => p.SecurityObject == securityObject);
            List<string> blockedMembers = new List<string>();
            if(securityObjectMetaData != null) {
                blockedMembers.AddRange(securityObjectMetaData.BlockedProperties);
                blockedMembers.AddRange(securityObjectMetaData.BlockedNavigationProperties);
            }
            return blockedMembers;
        }
        public static object GetRealObject(this ISecurityObjectRepository securityObjectRepository, object obj) {
            IEnumerable<SecurityObjectBuilder> resource = securityObjectRepository.GetAllBuilders();
            object realObject = resource.FirstOrDefault(p => p.SecurityObject == obj)?.RealObject;
            if(realObject == null)
                realObject = resource.FirstOrDefault(p => p.RealObject == obj)?.RealObject;
            return realObject;
        }
        public static object GetSecurityObject(this ISecurityObjectRepository securityObjectRepository, object obj) {
            IEnumerable<SecurityObjectBuilder> resource = securityObjectRepository.GetAllBuilders();
            object securityObject = resource.FirstOrDefault(p => p.RealObject == obj)?.SecurityObject;
            if(securityObject == null)
                securityObject = resource.FirstOrDefault(p => p.SecurityObject == obj)?.SecurityObject;
            return securityObject;
        }
        public static SecurityObjectBuilder GetObjectMetaData(this ISecurityObjectRepository securityObjectRepository, object targetObject) {
            IEnumerable<SecurityObjectBuilder> resource = securityObjectRepository.GetAllBuilders();
            return resource.FirstOrDefault(p => p.SecurityObject == targetObject || p.RealObject == targetObject);
        }     
        public static void RegisterBuilders(this ISecurityObjectRepository securityObjectRepository, IEnumerable<SecurityObjectBuilder> builders) {
            foreach(SecurityObjectBuilder builder in builders) {
                securityObjectRepository.RegisterBuilder(builder);
            }
        }
        public static void RemoveBuilders(this ISecurityObjectRepository securityObjectRepository, IEnumerable<SecurityObjectBuilder> builders) {
            foreach(SecurityObjectBuilder builder in builders) {
                securityObjectRepository.RemoveBuilder(builder);
            }
        }
        public static IEnumerable<SecurityObjectBuilder> GetDuplicateBuilders(this ISecurityObjectRepository securityObjectRepository, IEnumerable<object> objects) {
            List<SecurityObjectBuilder> dublicateBuilders = new List<SecurityObjectBuilder>();
            IEnumerable<SecurityObjectBuilder> allBuilders = securityObjectRepository.GetAllBuilders();
            foreach(object obj in objects) {
                SecurityObjectBuilder builder = allBuilders.FirstOrDefault(p => Equals(p.RealObject, obj) || Equals(p.SecurityObject, obj));
                if(builder != null) {
                    dublicateBuilders.Add(builder);
                }
            }
            return dublicateBuilders;
        }
    }
}
