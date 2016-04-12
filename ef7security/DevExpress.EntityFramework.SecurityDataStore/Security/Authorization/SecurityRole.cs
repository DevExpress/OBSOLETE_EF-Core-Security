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
        public ICollection<UserRole> UserRoleCollection { get; set; }
        = new Collection<UserRole>();
        public ICollection<SecurityTypePermission> TypePermissionCollection { get; set; } 
            = new Collection<SecurityTypePermission>();
        public ICollection<SecurityPolicyPermission> OperationPermissionCollection { get; set; }
           = new Collection<SecurityPolicyPermission>();
        public ICollection<SecurityMemberPermission> MemberPermissionCollection { get; set; }
           = new Collection<SecurityMemberPermission>();
        public ICollection<SecurityObjectPermission> ObjectPermissionCollection { get; set; }
           = new Collection<SecurityObjectPermission>();
    }
}
