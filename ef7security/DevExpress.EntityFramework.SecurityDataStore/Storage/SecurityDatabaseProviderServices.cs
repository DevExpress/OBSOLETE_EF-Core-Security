using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.EntityFrameworkCore;

namespace DevExpress.EntityFramework.SecurityDataStore.Storage {
    public class SecurityDatabaseProviderServices : DatabaseProviderServices {
        DbContext dbContextNative;
        SecurityDbContext dbContextSecurity;
        IServiceProvider services;
        public SecurityDatabaseProviderServices([NotNull] IServiceProvider services, [NotNull] DbContext dbContext) : base(services) {
            this.services = services;
            this.dbContextNative = ((SecurityDbContext)dbContext).realDbContext;
            this.dbContextSecurity = ((SecurityDbContext)dbContext);
        }
        public override IDatabaseCreator Creator => GetService<SecurityDatabaseCreator>();

        public override IDatabase Database => GetService<SecurityDatabase>();

        public override IEntityQueryableExpressionVisitorFactory EntityQueryableExpressionVisitorFactory => (IEntityQueryableExpressionVisitorFactory)services.GetService(dbContextNative.GetService<IEntityQueryableExpressionVisitorFactory>().GetType());

        public override IEntityQueryModelVisitorFactory EntityQueryModelVisitorFactory => (IEntityQueryModelVisitorFactory)services.GetService(dbContextNative.GetService<IEntityQueryModelVisitorFactory>().GetType());

        public override string InvariantName => GetType().GetTypeInfo().Assembly.GetName().Name;

        public override IModelSource ModelSource => (IModelSource)services.GetService(dbContextNative.GetService<IModelSource>().GetType());

        public override IQueryContextFactory QueryContextFactory => GetService<SecurityQueryContextFactory>();

        public override IDbContextTransactionManager TransactionManager => (IDbContextTransactionManager)services.GetService(dbContextNative.GetService<IDbContextTransactionManager>().GetType());

        public override IValueGeneratorCache ValueGeneratorCache => (IValueGeneratorCache)services.GetService(dbContextNative.GetService<IValueGeneratorCache>().GetType());
        public override IValueGeneratorSelector ValueGeneratorSelector => (IValueGeneratorSelector)services.GetService(dbContextNative.GetService<IValueGeneratorSelector>().GetType());
       
    }
}
