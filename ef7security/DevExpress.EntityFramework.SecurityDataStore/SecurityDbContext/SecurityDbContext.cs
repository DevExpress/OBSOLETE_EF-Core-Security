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

namespace DevExpress.EntityFramework.SecurityDataStore {
    public class SecurityDbContext : DbContext, IDisposable {
        private DbContextOptions options;
        private IServiceProvider serviceProvider;
        public SecurityDbContext realDbContext { get; private set; }
        private bool isDisposed;
        internal bool UseRealProvider = false;
        public ISecurityStrategy Security {
            get {
                return this.GetService<ISecurityStrategy>();
            }
        }
        protected virtual void OnSecuredConfiguring(DbContextOptionsBuilder optionsBuilder) {
        }
        sealed protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if(UseRealProvider) {
                OnSecuredConfiguring(optionsBuilder);
                return;
            }
            realDbContext = CreateDbContext();
            DbContextOptionsBuilder dbContextOptionsBuilderNative = new DbContextOptionsBuilder();
            OnSecuredConfiguring(dbContextOptionsBuilderNative);
            realDbContext.UseRealProvider = true;

            Type securityOptionExtensionType = typeof(SecurityOptionsExtension<>).MakeGenericType(GetType());
            var securityOptionsExtension = Activator.CreateInstance(securityOptionExtensionType, this, dbContextOptionsBuilderNative);

            var builder = ((IDbContextOptionsBuilderInfrastructure)optionsBuilder);
            var methods = typeof(IDbContextOptionsBuilderInfrastructure).GetMethods();

            MethodInfo methodInfoDbSet = methods.First(m => m.Name == "AddOrUpdateExtension").MakeGenericMethod(securityOptionExtensionType);
            methodInfoDbSet.Invoke(builder, new object[] { securityOptionsExtension });
        }
        private SecurityDbContext CreateDbContext() {
            if(options == null && serviceProvider == null) {
                return (SecurityDbContext)Activator.CreateInstance(GetType());
            }

            if(options != null && serviceProvider == null) {
                if(GetType().GetConstructor(new[] { typeof(DbContextOptions) }) != null) {
                    return (SecurityDbContext)Activator.CreateInstance(GetType(), options);
                }
                if(GetType().GetConstructor(new[] { typeof(IServiceProvider), typeof(DbContextOptions) }) != null) {
                    return (SecurityDbContext)Activator.CreateInstance(GetType(), null, options);
                }
                throw new NotSupportedException();
            }

            if(options == null && serviceProvider != null) {
                if(GetType().GetConstructor(new[] { typeof(IServiceProvider) }) != null) {
                    return (SecurityDbContext)Activator.CreateInstance(GetType(), serviceProvider);
                }
                if(GetType().GetConstructor(new[] { typeof(IServiceProvider), typeof(DbContextOptions) }) != null) {
                    return (SecurityDbContext)Activator.CreateInstance(GetType(), serviceProvider, null);
                }
                throw new NotSupportedException();
            }

            if(options != null && serviceProvider != null) {
                if(GetType().GetConstructor(new[] { typeof(IServiceProvider), typeof(DbContextOptions) }) != null) {
                    return (SecurityDbContext)Activator.CreateInstance(GetType(), serviceProvider, options);
                }
                throw new NotSupportedException();
            }

            throw new NotSupportedException();
        }
        public SecurityDbContext(DbContextOptions options) : base(options) {
            this.options = options;
        }
        public SecurityDbContext(IServiceProvider serviceProvider) : base(serviceProvider) {
            this.serviceProvider = serviceProvider;
        }
        public SecurityDbContext(IServiceProvider serviceProvider, DbContextOptions options) : base(serviceProvider, options) {
            this.serviceProvider = serviceProvider;
            this.options = options;
        }
        public SecurityDbContext() : base() { }
        public SecurityObjectRepository GetSecurityObjectRepository() {
            return this.GetService<SecurityObjectRepository>();
        } 
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
