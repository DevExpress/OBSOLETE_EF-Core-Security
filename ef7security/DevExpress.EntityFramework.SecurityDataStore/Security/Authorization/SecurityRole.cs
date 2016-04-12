using DevExpress.EntityFramework.SecurityDataStore.Security.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Security {
    public class SecurityRole : BaseSecurityObject, ISecurityRole {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public ICollection<IUserRole> UserRoleCollection { get; set; }
        = new Collection<IUserRole>();
        public ICollection<ISecurityTypePermission> TypePermissionCollection { get; set; } 
            = new Collection<ISecurityTypePermission>();
        public ICollection<ISecurityPermission> OperationPermissionCollection { get; set; }
           = new Collection<ISecurityPermission>();
        public ICollection<ISecurityMemberPermission> MemberPermissionCollection { get; set; }
           = new Collection<ISecurityMemberPermission>();
        public ICollection<ISecurityObjectPermission> ObjectPermissionCollection { get; set; }
           = new Collection<ISecurityObjectPermission>();
    }
}
