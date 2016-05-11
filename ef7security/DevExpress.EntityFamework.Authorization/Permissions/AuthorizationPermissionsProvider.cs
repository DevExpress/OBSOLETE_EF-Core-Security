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
        void Logon(ISecurityUser securityUser);
        void Logoff();
    }
    public class AuthorizationPermissionsProvider : IPermissionsProvider, IAuthorization {
        private ISecurityUser securityUser;
        public IEnumerable<IPermission> GetPermissions() {
            IEnumerable<IPermission> resultPermissions;
            if(securityUser == null) {
                resultPermissions = new IPermission[0];
            }
            else {
                resultPermissions = GetAllPermissions(securityUser);
            }
            return resultPermissions;
        }
        public virtual void Logon(ISecurityUser securityUser) {
            this.securityUser = securityUser;
        }
        public virtual void Logoff() {
            securityUser = null;
        }
        protected virtual IEnumerable<IPermission> GetAllPermissions(ISecurityUser securityUser) {
            return securityUser.GetAllPermissions();
        }
    }
}
