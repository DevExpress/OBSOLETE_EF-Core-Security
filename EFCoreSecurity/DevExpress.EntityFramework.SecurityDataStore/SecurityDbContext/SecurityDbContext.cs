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
        private bool isDisposed;
        internal bool useRealProvider = false;
        public DbContext realDbContext { get; private set; }
        public virtual ISecurityStrategy Security {
            get {
                return this.GetService<ISecurityStrategy>();
            }
        }
        protected virtual void SecurityRegistryServices(IServiceCollection services) {
            RegistrySecurityStrategy(services);
            RegistryPermissionsProvider(services);
            RegistryPermissionsContainer(services);
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
        protected virtual void RegistryPermissionsContainer(IServiceCollection services) {
            services.AddScoped<IPermissionsContainer, PermissionsContainer>();
        }
        protected virtual void RegistryPermissionsProvider(IServiceCollection services) {
            services.AddScoped<IPermissionsProvider, PermissionsProvider>();
        }
        protected virtual void RegistrySecurityStrategy(IServiceCollection services) {
            services.AddScoped<ISecurityStrategy, SecurityStrategy>();
        }
        internal void InternalSecurityRegistryServices(IServiceCollection services) {
            SecurityRegistryServices(services);            
        }    
        protected virtual void OnSecuredConfiguring(DbContextOptionsBuilder optionsBuilder) {
        }
        sealed protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if(useRealProvider) {
                OnSecuredConfiguring(optionsBuilder);
                return;
            }
            SecurityDbContext realContext = CreateDbContext();
            realDbContext = realContext;
            DbContextOptionsBuilder dbContextOptionsBuilderNative = new DbContextOptionsBuilder();
            OnSecuredConfiguring(dbContextOptionsBuilderNative);
            realContext.useRealProvider = true;

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
        /*
        public virtual TEntity DatabaseEntity<TEntity>([NotNull] TEntity entity) where TEntity : class {
            ISecurityObjectRepository securityObjectRepository = this.GetService<ISecurityObjectRepository>();
            IEnumerable<SecurityObjectBuilder> resource = securityObjectRepository.GetAllBuilders();
            SecurityObjectBuilder securityObjectMetaData = resource.FirstOrDefault(p => p.SecurityObject == entity);
            return securityObjectMetaData.RealObject as TEntity;
        }
        */
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
