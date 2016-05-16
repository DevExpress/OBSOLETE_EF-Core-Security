using DevExpress.EntityFramework.SecurityDataStore.Storage;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using DevExpress.EntityFramework.SecurityDataStore.Security;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Internal;

namespace DevExpress.EntityFramework.SecurityDataStore.Infrastructure {
    public class SecurityOptionsExtension<TSource> : IDbContextOptionsExtension where TSource : SecurityDbContext {
        private DbContext dbContext;
        private SecurityDbContext securityDbContext;
        private IServiceCollection service;
        private DbContextOptionsBuilder dbContextOptionsBuilderNative;
        private void AddNativeServices() {
            IServiceCollection serviceCollection = new ServiceCollection();
            foreach(IDbContextOptionsExtension dbContextOptionsExtension in dbContextOptionsBuilderNative.Options.Extensions) {               
                dbContextOptionsExtension.ApplyServices(serviceCollection);
                service.TryAdd(serviceCollection);
            }
        }

        public SecurityOptionsExtension(DbContext dbContext, DbContextOptionsBuilder dbContextOptionsBuilderNative) {
            securityDbContext = (SecurityDbContext)dbContext;
            this.dbContext = ((SecurityDbContext)dbContext).realDbContext;
            this.dbContextOptionsBuilderNative = dbContextOptionsBuilderNative;
        }
        public void ApplyServices([NotNull] IServiceCollection service) {
            this.service = service;

            AddNativeServices();
            service.TryAddEnumerable(ServiceDescriptor.Singleton<IDatabaseProvider, DatabaseProvider<SecurityDatabaseProviderServices, SecurityOptionsExtension<TSource>>>());          
            service.AddScoped<SecurityDatabaseProviderServices>();
            service.AddScoped<SecurityDatabase>();
            service.AddScoped<SecurityQueryExecutor>();
            service.AddScoped<SecurityQueryContextFactory>();
            service.AddScoped<SecurityDatabaseCreator>();
            service.AddScoped(p => p.GetService<IDbContextServices>().CurrentContext.Context);
            securityDbContext.InternalSecurityRegistryServices(service);
        }
    }
}
