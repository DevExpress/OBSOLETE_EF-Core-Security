using DevExpress.EntityFramework.SecurityDataStore.Security.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Authorization {
    public class SecurityUser : BaseSecurityObject, ISecurityUser {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public ICollection<UserRole> UserRoleCollection { get; set; }
        = new Collection<UserRole>();

        IEnumerable<IUserRole> ISecurityUser.UserRoleCollection {
            get {
                return UserRoleCollection.OfType<IUserRole>();
            }
        }
        public void AddRole(SecurityRole role) {
            UserRoleCollection.Add(new UserRole() { User = this, Role = role });
        }
        public IEnumerable<IPermission> GetPermissions() {
            List<IPermission> permissions = new List<IPermission>();
            foreach(var userRole in UserRoleCollection) {
                permissions.AddRange(userRole.Role.OperationPermissions);
                permissions.AddRange(userRole.Role.TypePermissions);
                permissions.AddRange(userRole.Role.ObjectPermissions);
                permissions.AddRange(userRole.Role.MemberPermissions);
            }
            return permissions;
        }
    }
}
