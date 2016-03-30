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

namespace DevExpress.EntityFramework.SecurityDataStore.Infrastructure {
    public class SecurityOptionsExtension<TSource> : IDbContextOptionsExtension where TSource : SecurityDbContext{
        private DbContext dbContext;
        private DbContext dbContextSecurity;
        private IServiceCollection service;
        private DbContextOptionsBuilder dbContextOptionsBuilderNative;
        private void AddNativeServices() {
            if(dbContextOptionsBuilderNative.Options.Extensions.Count() == 1) {
                IServiceCollection serviceCollection = new ServiceCollection();
                EntityFrameworkServicesBuilder builderNative = new EntityFrameworkServicesBuilder(serviceCollection);
                IDbContextOptionsExtension dbContextOptionsExtension = dbContextOptionsBuilderNative.Options.Extensions.First();
                dbContextOptionsExtension.ApplyServices(builderNative);
                service.TryAdd(serviceCollection);
            }
            else {
                throw new NotSupportedException();
            }
        }
        public SecurityOptionsExtension(DbContext dbContext, DbContextOptionsBuilder dbContextOptionsBuilderNative) {
            dbContextSecurity = dbContext;
            this.dbContext = ((SecurityDbContext)dbContext).realDbContext;
            this.dbContextOptionsBuilderNative = dbContextOptionsBuilderNative;
        }
        public void ApplyServices(EntityFrameworkServicesBuilder builder) {
            service = builder.GetInfrastructure();
            AddNativeServices();
            service.TryAddEnumerable(ServiceDescriptor.Singleton<IDatabaseProvider, DatabaseProvider<SecurityDatabaseProviderServices, SecurityOptionsExtension<TSource>>>());           
            service.AddScoped<ISecurityStrategy, SecurityStrategy>();
            service.AddScoped<SecurityDatabaseProviderServices>();
            service.AddScoped<SecurityDatabase>();
            service.AddScoped<SecurityQueryExecutor>();
            service.AddScoped<SecurityQueryContextFactory>();
            service.AddScoped<SecurityDatabaseCreator>();

            service.AddScoped<SecurityObjectRepository>();
            service.AddScoped<SecurityObjectsBuilder>();
            service.AddScoped<PermissionProcessor>();
            


        }
    }
}
