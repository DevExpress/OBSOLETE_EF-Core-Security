using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Security {
    public class UserRole : IUserRole {
        public Guid ID { get; set; }
        public Guid UserID { get; set; }
        public SecurityUser User { get; set; }
        public Guid RoleID { get; set; }
        public SecurityRole Role { get; set; }
    }
}
