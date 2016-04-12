using DevExpress.EntityFramework.SecurityDataStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Security {
    public class SecurityOperationPermission : IPermission, ISecurityPermission {
        public Guid ID { get; set; }
        public SecurityOperation Operations { get; set; }
        public SecurityRole SecurityRole { get; set; }
        public Guid SecurityRoleID { get; set; }
    }
}
