using DevExpress.EntityFramework.SecurityDataStore.Security;
using System.Collections.Generic;

namespace DevExpress.EntityFramework.SecurityDataStore.Authorization {
    public interface ISecurityRole {
         string Name { get; set; }
         ICollection<UserRole> Roles { get; set; }
         ICollection<SecurityTypePermission> TypePermissions { get; set; }
         ICollection<SecurityPolicyPermission> OperationPermissions { get; set; }
         ICollection<SecurityMemberPermission> MemberPermissions { get; set; }
         ICollection<SecurityObjectPermission> ObjectPermissions { get; set; }
    }
}