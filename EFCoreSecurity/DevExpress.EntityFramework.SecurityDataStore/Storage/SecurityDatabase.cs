using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using Remotion.Linq;
using DevExpress.EntityFramework.SecurityDataStore.Utility;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using DevExpress.EntityFramework.SecurityDataStore.Security;
using System.Collections;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DevExpress.EntityFramework.SecurityDataStore.Storage {
    public class SecurityDatabase : Database {
        private BaseSecurityDbContext securityDbContext;
        private DbContext realDbContext;
        private ISecurityStrategy securityStrategy;
        private SecuritySaveObjects SecuritySaveObjectsService { get; }

        public SecurityDatabase(IQueryCompilationContextFactory queryCompilationContextFactory,
            DbContext dbContext
            )
            : base(queryCompilationContextFactory) {
            securityDbContext = (BaseSecurityDbContext)dbContext;
            realDbContext = securityDbContext.RealDbContext;
            securityStrategy = securityDbContext.Security;
            SecuritySaveObjectsService = new SecuritySaveObjects(securityDbContext, securityDbContext.Security./*SecurityServicesProvider.*/SecurityObjectRepository);

        }

        private SecurityQueryExecutor CreateQueryExecutor(QueryModel queryModel) {
            return new SecurityQueryExecutor(queryModel);
        }
        public override int SaveChanges(IReadOnlyList<IUpdateEntry> entries) {
            int rowsAffected;
            IEnumerable<EntityEntry> entities = securityDbContext.ChangeTracker.Entries().Where(p =>
            p.State == EntityState.Added ||
             p.State == EntityState.Deleted ||
              p.State == EntityState.Modified);
            rowsAffected = SecuritySaveObjectsService.ProcessObject(entities);
            return rowsAffected;
        }

        public override Task<int> SaveChangesAsync(IReadOnlyList<IUpdateEntry> entries, CancellationToken cancellationToken = default(CancellationToken)) {
            return Task.Run(() => SaveChanges(entries), cancellationToken);
        }
        public override Func<QueryContext, IEnumerable<TResult>> CompileQuery<TResult>(QueryModel queryModel) {
            SecurityQueryExecutor queryExecutor = CreateQueryExecutor(queryModel);
            return p => queryExecutor.Execute<TResult>(p);
        }
        public override Func<QueryContext, IAsyncEnumerable<TResult>> CompileAsyncQuery<TResult>(QueryModel queryModel) {
            SecurityQueryExecutor queryExecutor = CreateQueryExecutor(queryModel);
            return (qc) => queryExecutor.Execute<TResult>(qc).ToAsyncEnumerable();
        }
    }
}
