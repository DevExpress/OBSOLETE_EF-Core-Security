using DevExpress.EntityFramework.SecurityDataStore.Security.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Security {
    public class SecurityUser : BaseSecurityObject, ISecurityUser {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public ICollection<UserRole> UserRoleCollection { get; set; }
        = new Collection<UserRole>();

    }
}
