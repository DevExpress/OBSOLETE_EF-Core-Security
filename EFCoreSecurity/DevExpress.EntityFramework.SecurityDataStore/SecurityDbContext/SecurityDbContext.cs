using DevExpress.EntityFramework.SecurityDataStore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using DevExpress.EntityFramework.SecurityDataStore.Security;
using Microsoft.EntityFrameworkCore.Internal;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public class SecurityDbContext : BaseSecurityDbContext {
        public IPermissionsContainer PermissionsContainer {
            get {
                return this.GetService<IPermissionsContainer>();
            }
        }
        protected override void SecurityRegistryServices(IServiceCollection services) {
            base.SecurityRegistryServices(services);
            RegistryPermissionsProvider(services);
            RegistryPermissionsContainer(services);
        }
        protected void RegistryPermissionsContainer(IServiceCollection services) {
            services.AddScoped<IPermissionsContainer, PermissionsContainer>();
        }
        protected void RegistryPermissionsProvider(IServiceCollection services) {
            services.AddScoped<IPermissionsProvider, PermissionsProvider>();
        }
    }
}
