using DevExpress.EntityFramework.SecurityDataStore.Security;
using System.Collections.Generic;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public interface ISecurityRole {
         string Name { get; set; }
         ICollection<UserRole> UserRoleCollection { get; set; }
         ICollection<SecurityTypePermission> TypePermissionCollection { get; set; }
         ICollection<SecurityPolicyPermission> OperationPermissionCollection { get; set; }
         ICollection<SecurityMemberPermission> MemberPermissionCollection { get; set; }
         ICollection<SecurityObjectPermission> ObjectPermissionCollection { get; set; }
    }
}