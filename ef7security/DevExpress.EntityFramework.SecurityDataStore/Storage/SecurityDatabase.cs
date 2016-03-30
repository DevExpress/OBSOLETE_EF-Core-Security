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
        private SecurityDbContext securityDbContext;
        private SecurityDbContext dbContext;
        private ISecurityStrategy securityStrategy;
        private readonly object _lock = new object();
        private SecurityObjectRepository securityObjectRepository;
        private SecuritySaveObjectsService securitySaveObjectsService;
        private SecuritySaveObjectsService SecuritySaveObjectsService {
            get {
                if(securitySaveObjectsService == null) {
                    securitySaveObjectsService = new SecuritySaveObjectsService(securityDbContext, securityObjectRepository);
                }
                return securitySaveObjectsService;
            }
        }

        public SecurityDatabase(IQueryCompilationContextFactory queryCompilationContextFactory,
            DbContext dbContext,
            SecurityObjectRepository securityObjectRepository
            )
            : base(queryCompilationContextFactory) {
            securityDbContext = (SecurityDbContext)dbContext;
            this.dbContext = securityDbContext.realDbContext;
            securityStrategy = securityDbContext.Security;
            this.securityObjectRepository = securityObjectRepository;
        }

        private SecurityQueryExecutor CreateQueryExecutor(QueryModel queryModel) {
            return new SecurityQueryExecutor(queryModel);
        }
        public override int SaveChanges(IReadOnlyList<IUpdateEntry> entries) {
            int rowsAffected;
            rowsAffected = SecuritySaveObjectsService.ProcessObject(entries.Select(p => p.ToEntityEntry()));
            dbContext.SaveChanges();
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
