using DevExpress.EntityFramework.SecurityDataStore.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.EntityFramework.SecurityDataStore;
using DevExpress.EntityFramework.SecurityDataStore.Authorization;

namespace DevExpress.EntityFamework.Authorization.Permissions {
    public interface IAuthorization {
        ISecurityUser SecurityUser { get; }
        void Logon(ISecurityUser securityUser);
        void Logoff();
    }
    public class AuthorizationPermissionsProvider : IPermissionsProvider, IAuthorization {
        public ISecurityUser SecurityUser { get; private set; }
        public IEnumerable<IPermission> GetPermissions() {
            IEnumerable<IPermission> resultPermissions;
            if(SecurityUser == null) {
                resultPermissions = new IPermission[0];
            }
            else {
                resultPermissions = GetAllPermissions(SecurityUser);
            }
            return resultPermissions;
        }
        public virtual void Logon(ISecurityUser securityUser) {
            this.SecurityUser = securityUser;
        }
        public virtual void Logoff() {
            SecurityUser = null;
        }
        protected virtual IEnumerable<IPermission> GetAllPermissions(ISecurityUser securityUser) {
            return securityUser.GetPermissions();
        }
    }
}
