using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using DevExpress.EntityFramework.SecurityDataStore.Security;
using DevExpress.EntityFamework.Authorization.Permissions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using DevExpress.EntityFramework.SecurityDataStore;
using DevExpress.EntityFramework.SecurityDataStore.Authorization;

namespace DevExpress.EntityFramework.Authorization {
    public class AuthorizationDbContext : BaseSecurityDbContext {
        protected override void SecurityRegistryServices(IServiceCollection services) {
            base.SecurityRegistryServices(services);
            RegistryPermissionsProvider(services);
        }
        protected virtual void RegistryPermissionsProvider(IServiceCollection services) {
            services.AddScoped<IPermissionsProvider, AuthorizationPermissionsProvider>();
        }
        public ISecurityUser CurrentUser {
            get {
                IAuthorization authorization = (IAuthorization)this.GetService<IPermissionsProvider>();
                return authorization.SecurityUser;
            }
        }
        public virtual void Logon(ISecurityUser user) {
            IAuthorization authorization = (IAuthorization)this.GetService<IPermissionsProvider>();
            authorization.Logon(user);        
        }
        public virtual void Logoff() {
            IAuthorization authorization = (IAuthorization)this.GetService<IPermissionsProvider>();
            authorization.Logoff();
        }
    }
}
