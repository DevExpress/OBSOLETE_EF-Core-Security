using System;

namespace DevExpress.EntityFramework.SecurityDataStore.Authorization {
    public class UserRole : IUserRole {
        public Guid ID { get; set; }
        public Guid UserID { get; set; }
        public SecurityUser User { get; set; }
        public Guid RoleID { get; set; }
        public SecurityRole Role { get; set; }
        ISecurityUser IUserRole.User {
            get {
                return User;
            }
        }
        ISecurityRole IUserRole.Role {
            get {
                return Role;
            }
        }
    }
}
