using DevExpress.EntityFramework.SecurityDataStore.Security;
using System.Collections.Generic;

namespace DevExpress.EntityFramework.SecurityDataStore.Authorization {
    public interface ISecurityRole : IPermissionsContainer {
        string Name { get; set; }
        IEnumerable<IUserRole> Roles { get; }
        IEnumerable<ISecurityTypePermission> TypePermissions { get; }
        IEnumerable<ISecurityPolicyPermission> OperationPermissions { get; }
        IEnumerable<ISecurityMemberPermission> MemberPermissions { get; }
        IEnumerable<ISecurityObjectPermission> ObjectPermissions { get; }
    }
}