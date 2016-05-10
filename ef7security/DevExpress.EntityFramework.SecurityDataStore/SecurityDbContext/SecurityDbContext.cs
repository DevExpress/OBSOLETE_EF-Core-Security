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
    public class SecurityDbContext : DbContext, IDisposable {
        private DbContextOptions options;
        protected IServiceCollection services;
        private bool isDisposed;
        internal bool UseRealProvider = false;
        public DbContext realDbContext { get; private set; }
        public virtual ISecurityStrategy Security {
            get {
                return this.GetService<ISecurityStrategy>();
            }
        }
        protected virtual void SecurityRegistrationServices(IServiceCollection services) {
            this.services = services;
            services.AddScoped<ISecurityStrategy, SecurityStrategy>();
            services.AddScoped<IPermissionsRepository, PermissionsRepository>();
            services.AddScoped<ISecurityProcessLoadObjects, SecurityProcessLoadObjects>();
            services.AddScoped<ISecuritySaveObjects, SecuritySaveObjects>();
            services.AddScoped<ISecurityObjectRepository, SecurityObjectRepository>();
            services.AddScoped<IModificationСriterion, ModificationСriterion>();
            services.AddScoped<IPermissionProcessor, PermissionProcessor>();
        }
        internal void InternalSecurityRegistrationServices(IServiceCollection service) {
            SecurityRegistrationServices(service);
        }    
        protected virtual void OnSecuredConfiguring(DbContextOptionsBuilder optionsBuilder) {
        }
        sealed protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if(UseRealProvider) {
                OnSecuredConfiguring(optionsBuilder);
                return;
            }
            SecurityDbContext realContext = CreateDbContext();
            realDbContext = realContext;
            DbContextOptionsBuilder dbContextOptionsBuilderNative = new DbContextOptionsBuilder();
            OnSecuredConfiguring(dbContextOptionsBuilderNative);
            realContext.UseRealProvider = true;


            Type securityOptionExtensionType = typeof(SecurityOptionsExtension<>).MakeGenericType(GetType());
            var securityOptionsExtension = Activator.CreateInstance(securityOptionExtensionType, this, dbContextOptionsBuilderNative);

            var builder = ((IDbContextOptionsBuilderInfrastructure)optionsBuilder);
            var methods = typeof(IDbContextOptionsBuilderInfrastructure).GetMethods();

            MethodInfo methodInfoDbSet = methods.First(m => m.Name == "AddOrUpdateExtension").MakeGenericMethod(securityOptionExtensionType);
            methodInfoDbSet.Invoke(builder, new object[] { securityOptionsExtension });
        }
        private SecurityDbContext CreateDbContext() {
            if(options == null) {
                return (SecurityDbContext)Activator.CreateInstance(GetType());
            }

            if(options != null) {
                if(GetType().GetConstructor(new[] { typeof(DbContextOptions) }) != null) {
                    return (SecurityDbContext)Activator.CreateInstance(GetType(), options);
                }
            }
            throw new NotSupportedException();
        }
        public SecurityDbContext(DbContextOptions options) : base(options) {
            this.options = options;
        }
        public SecurityDbContext() : base() { }     
        public override void Dispose() {
            if(!isDisposed) {
                isDisposed = true;
                base.Dispose();
                if(realDbContext != null) {
                    realDbContext.Dispose();
                    realDbContext = null;
                }
            }
        }
    }
}
