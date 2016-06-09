using DevExpress.EntityFramework.SecurityDataStore.Infrastructure;
using DevExpress.EntityFramework.SecurityDataStore.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public abstract class BaseSecurityDbContext : DbContext, IDisposable{
        private DbContextOptions options;
        private bool isDisposed;
        internal bool useRealProvider = false;
        public BaseSecurityDbContext RealDbContext { get; private set; }
        public virtual ISecurityStrategy Security {
            get {
                return this.GetService<ISecurityStrategy>();
            }
        }
        internal void InternalSecurityRegistryServices(IServiceCollection services) {
            SecurityRegistryServices(services);
        }
        protected virtual void SecurityRegistryServices(IServiceCollection services) {
            RegistrySecurityStrategy(services);
            RegistrySecurityProcessLoadObjects(services);
            RegistrySecuritySaveObjects(services);
            RegistrySecurityObjectRepository(services);
            RegistrySecurityExpressionBuilder(services);
            RegistryPermissionProcessor(services);
        }
        protected virtual void RegistryPermissionProcessor(IServiceCollection services) {
            services.AddScoped<IPermissionProcessor, PermissionProcessor>();
        }
        protected virtual void RegistrySecurityExpressionBuilder(IServiceCollection services) {
            services.AddScoped<ISecurityExpressionBuilder, SecurityExpressionBuilder>();
        }
        protected virtual void RegistrySecurityObjectRepository(IServiceCollection services) {
            services.AddScoped<ISecurityObjectRepository, SecurityObjectRepository>();
        }
        protected virtual void RegistrySecuritySaveObjects(IServiceCollection services) {
            services.AddScoped<ISecuritySaveObjects, SecuritySaveObjects>();
        }
        protected virtual void RegistrySecurityProcessLoadObjects(IServiceCollection services) {
            services.AddScoped<ISecurityProcessLoadObjects, SecurityProcessLoadObjects>();
        }
        
        protected virtual void RegistrySecurityStrategy(IServiceCollection services) {
            services.AddScoped<ISecurityStrategy, SecurityStrategy>();
        }
        protected virtual void OnSecuredConfiguring(DbContextOptionsBuilder optionsBuilder) {
        }
        sealed protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if(useRealProvider) {
                OnSecuredConfiguring(optionsBuilder);
                return;
            }
            BaseSecurityDbContext realContext = CreateDbContext();
            RealDbContext = realContext;
            DbContextOptionsBuilder dbContextOptionsBuilderNative = new DbContextOptionsBuilder();
            OnSecuredConfiguring(dbContextOptionsBuilderNative);
            realContext.useRealProvider = true;

            realContext.ChangeTracker.AutoDetectChangesEnabled = false;

            Type securityOptionExtensionType = typeof(SecurityOptionsExtension<>).MakeGenericType(GetType());
            var securityOptionsExtension = Activator.CreateInstance(securityOptionExtensionType, this, dbContextOptionsBuilderNative);

            var builder = ((IDbContextOptionsBuilderInfrastructure)optionsBuilder);
            var methods = typeof(IDbContextOptionsBuilderInfrastructure).GetMethods();

            MethodInfo methodInfoDbSet = methods.First(m => m.Name == "AddOrUpdateExtension").MakeGenericMethod(securityOptionExtensionType);
            methodInfoDbSet.Invoke(builder, new object[] { securityOptionsExtension });
        }
        private BaseSecurityDbContext CreateDbContext() {
            if(options == null) {
                return (BaseSecurityDbContext)Activator.CreateInstance(GetType());
            }

            if(options != null) {
                if(GetType().GetConstructor(new[] { typeof(DbContextOptions) }) != null) {
                    return (BaseSecurityDbContext)Activator.CreateInstance(GetType(), options);
                }
            }
            throw new NotSupportedException();
        }
        public BaseSecurityDbContext(DbContextOptions options) : base(options) {
            this.options = options;
        }
        public BaseSecurityDbContext() : base() { }
        public override void Dispose() {
            if(!isDisposed) {
                isDisposed = true;
                base.Dispose();
                if(RealDbContext != null) {
                    RealDbContext.Dispose();
                    RealDbContext = null;
                }
            }
        }
    }
}
