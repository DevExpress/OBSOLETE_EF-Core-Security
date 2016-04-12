using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DevExpress.EntityFramework.SecurityDataStore.Security;
using DevExpress.EntityFramework.SecurityDataStore.Utility;

namespace DevExpress.EntityFramework.SecurityDataStore.Security {
    public class UserSecurityStrategy : SecurityStrategy {
        private DbContextUsersBase DbContextUser;
        public ISecurityUser CurrentUser { get; set; }
        public virtual void Logon(string user, string password) {
            var currentUser = DbContextUser.Users.
                Include(p => p.UserRoleCollection).ThenInclude(p => p.Role).ThenInclude(p => p.MemberPermissionCollection).
                 Include(p => p.UserRoleCollection).ThenInclude(p => p.Role).ThenInclude(p => p.OperationPermissionCollection).
                 Include(p => p.UserRoleCollection).ThenInclude(p => p.Role).ThenInclude(p => p.ObjectPermissionCollection).
                 Include(p => p.UserRoleCollection).ThenInclude(p => p.Role).ThenInclude(p => p.TypePermissionCollection).
                First(p => p.Name == user && p.Password == password);
            Logon(currentUser);
        }
        public virtual void Logon(ISecurityUser user) {
            CurrentUser = user;
            InitSecurity(user);
        }

        protected virtual void InitSecurity(ISecurityUser user) {
            SecurityPermissions.Clear();
            var allPermissions = user.GetAllPermissions();
            foreach(var permission in allPermissions) {
                SecurityPermissions.Add(permission);
            }
        }       
        public UserSecurityStrategy(DbContext dbContext) : base(dbContext) {
            DbContextUser = (DbContextUsersBase)dbContext;
        }
    }
}
