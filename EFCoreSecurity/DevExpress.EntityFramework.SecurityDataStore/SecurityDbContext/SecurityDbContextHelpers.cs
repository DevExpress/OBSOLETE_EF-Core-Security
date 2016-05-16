using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.EntityFramework.SecurityDataStore.Security;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public struct BlockedObjectInfo {
        public enum OperationType { Create, Read, Write, Delete };
        public OperationType operationType;
        public Type objectType;
        public string memberName;
    }    

    public class SecurityAccessException : Exception {
        private List<BlockedObjectInfo> blockedObjectInfoList;
        public SecurityAccessException() {
            blockedObjectInfoList = new List<BlockedObjectInfo>();
        }
        public void AddBlockedObjectInfo(BlockedObjectInfo blockedObjectInfo) {
            blockedObjectInfoList.Add(blockedObjectInfo);
        }
        public void AddBlockedObjectInfoRange(IEnumerable<BlockedObjectInfo> blockedObjectInfo) {
            blockedObjectInfoList.AddRange(blockedObjectInfo);
        }
        public IList<BlockedObjectInfo> GetBlockedInfo() {
            return blockedObjectInfoList;
        }
        public bool IsEmpty() {
            return blockedObjectInfoList.Count == 0;
        }
    }



    public static class EntityStateExtensions {
        public static IList<string> GetBlockedMembers(this EntityEntry entityEntry) {
            SecurityDbContext securityDbContext = entityEntry.Context as SecurityDbContext;
            ISecurityObjectRepository objectRepository = securityDbContext.Security./*SecurityServicesProvider.*/SecurityObjectRepository;
            object securityObject = objectRepository.GetSecurityObject(entityEntry.Entity);
            IList<string> blockedMembers = objectRepository.GetBlockedMembers(securityObject);
            if(blockedMembers == null)
                blockedMembers = new List<string> { };

            return blockedMembers;
        }
    }
}

