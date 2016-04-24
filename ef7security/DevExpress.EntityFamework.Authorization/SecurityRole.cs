using DevExpress.EntityFramework.SecurityDataStore.Security.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Authorization {
    public class SecurityRole : BaseSecurityObject, ISecurityRole {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public ICollection<UserRole> Roles { get; set; }
        = new Collection<UserRole>();
        public ICollection<SecurityTypePermission> TypePermissions { get; set; } 
            = new Collection<SecurityTypePermission>();
        public ICollection<SecurityPolicyPermission> OperationPermissions { get; set; }
           = new Collection<SecurityPolicyPermission>();
        public ICollection<SecurityMemberPermission> MemberPermissions { get; set; }
           = new Collection<SecurityMemberPermission>();
        public ICollection<SecurityObjectPermission> ObjectPermissions { get; set; }
           = new Collection<SecurityObjectPermission>();
    }
}
