using System.Collections.Generic;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public interface ISecurityRole {
         string Name { get; set; }
         ICollection<IUserRole> UserRoleCollection { get; set; }
         ICollection<ISecurityTypePermission> TypePermissionCollection { get; set; }
         ICollection<ISecurityPermission> OperationPermissionCollection { get; set; }
         ICollection<ISecurityMemberPermission> MemberPermissionCollection { get; set; }
         ICollection<ISecurityObjectPermission> ObjectPermissionCollection { get; set; }
    }
}