using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Security {

    public class PermissionsProvider : IPermissionsProvider {       
        protected virtual IPermissionsContainer permissionsContainer { get; private set; }

        public virtual IEnumerable<IPermission> GetPermissions() {
            return permissionsContainer.GetPermissions();
        }       
        public PermissionsProvider(IPermissionsContainer permissionsContainer) {
            this.permissionsContainer = permissionsContainer;
        }
    }
}
