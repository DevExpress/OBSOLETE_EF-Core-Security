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
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;
using Microsoft.EntityFrameworkCore.Query.Sql;

namespace DevExpress.EntityFramework.SecurityDataStore.Storage {
    // RelationalDatabaseProviderServices { 
    public class SecurityDatabaseProviderServices : DatabaseProviderServices {
        DbContext dbContextNative;
        BaseSecurityDbContext dbContextSecurity;
        IServiceProvider services;
        public SecurityDatabaseProviderServices([NotNull] IServiceProvider services, [NotNull] DbContext dbContext) : base(services) {
            this.services = services;
            this.dbContextNative = ((BaseSecurityDbContext)dbContext).RealDbContext;
            this.dbContextSecurity = ((BaseSecurityDbContext)dbContext);
        }
        public override IDatabaseCreator Creator => GetService<SecurityDatabaseCreator>();

        public override IDatabase Database => GetService<SecurityDatabase>();

        public override IEntityQueryableExpressionVisitorFactory EntityQueryableExpressionVisitorFactory => dbContextNative.GetService<IEntityQueryableExpressionVisitorFactory>();
        // public override IEntityQueryableExpressionVisitorFactory EntityQueryableExpressionVisitorFactory => (IEntityQueryableExpressionVisitorFactory)services.GetService(dbContextNative.GetService<IEntityQueryableExpressionVisitorFactory>().GetType());

        public override IEntityQueryModelVisitorFactory EntityQueryModelVisitorFactory => dbContextNative.GetService<IEntityQueryModelVisitorFactory>();
        // public override IEntityQueryModelVisitorFactory EntityQueryModelVisitorFactory => (IEntityQueryModelVisitorFactory)services.GetService(dbContextNative.GetService<IEntityQueryModelVisitorFactory>().GetType());

        public override string InvariantName => GetType().GetTypeInfo().Assembly.GetName().Name;

        public override IModelSource ModelSource => dbContextNative.GetService<IModelSource>();
        // public override IModelSource ModelSource => (IModelSource)services.GetService(dbContextNative.GetService<IModelSource>().GetType());

        public override IQueryContextFactory QueryContextFactory => GetService<SecurityQueryContextFactory>();

        public override IDbContextTransactionManager TransactionManager => dbContextNative.GetService<IDbContextTransactionManager>();
        // public override IDbContextTransactionManager TransactionManager => (IDbContextTransactionManager)services.GetService(dbContextNative.GetService<IDbContextTransactionManager>().GetType());

        public override IValueGeneratorSelector ValueGeneratorSelector => dbContextNative.GetService<IValueGeneratorSelector>();
        // public override IValueGeneratorSelector ValueGeneratorSelector => (IValueGeneratorSelector)services.GetService(dbContextNative.GetService<IValueGeneratorSelector>().GetType());

        public override IValueGeneratorCache ValueGeneratorCache => dbContextNative.GetService<IValueGeneratorCache>();
        // public override IValueGeneratorCache ValueGeneratorCache => (IValueGeneratorCache)services.GetService(dbContextNative.GetService<IValueGeneratorCache>().GetType());

        //public override IValueGeneratorSelector ValueGeneratorSelector { get {
        //        Type serviceType = dbContextNative.GetService<IValueGeneratorSelector>().GetType();
        //        IValueGeneratorSelector valueGeneratorSelector = (IValueGeneratorSelector)services.GetService(serviceType);
        //        return valueGeneratorSelector;

        //        // return dbContextNative.GetService<IValueGeneratorSelector>();
        //    }
        //}

        /*
        public override IRelationalTypeMapper TypeMapper => (IRelationalTypeMapper)services.GetService(dbContextNative.GetService<IRelationalTypeMapper>().GetType());

        public override IMethodCallTranslator CompositeMethodCallTranslator => (IMethodCallTranslator)services.GetService(dbContextNative.GetService<IMethodCallTranslator>().GetType());
        public override IMemberTranslator CompositeMemberTranslator => (IMemberTranslator)services.GetService(dbContextNative.GetService<IMemberTranslator>().GetType());
        public override IHistoryRepository HistoryRepository => (IHistoryRepository)services.GetService(dbContextNative.GetService<IHistoryRepository>().GetType());
        public override IRelationalConnection RelationalConnection => (IRelationalConnection)services.GetService(dbContextNative.GetService<IRelationalConnection>().GetType());
        public override ISqlGenerationHelper SqlGenerationHelper => (ISqlGenerationHelper)services.GetService(dbContextNative.GetService<ISqlGenerationHelper>().GetType());
        public override IUpdateSqlGenerator UpdateSqlGenerator => (IUpdateSqlGenerator)services.GetService(dbContextNative.GetService<IUpdateSqlGenerator>().GetType());
        public override IModificationCommandBatchFactory ModificationCommandBatchFactory => (IModificationCommandBatchFactory)services.GetService(dbContextNative.GetService<IModificationCommandBatchFactory>().GetType());
        public override IRelationalDatabaseCreator RelationalDatabaseCreator => (IRelationalDatabaseCreator)services.GetService(dbContextNative.GetService<IRelationalDatabaseCreator>().GetType());
        public override IRelationalAnnotationProvider AnnotationProvider => (IRelationalAnnotationProvider)services.GetService(dbContextNative.GetService<IRelationalAnnotationProvider>().GetType());
        public override IQuerySqlGeneratorFactory QuerySqlGeneratorFactory => (IQuerySqlGeneratorFactory)services.GetService(dbContextNative.GetService<IQuerySqlGeneratorFactory>().GetType());
        */
    }
}
