using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;

namespace DevExpress.EntityFramework.SecurityDataStore.Storage {
    public class SecurityDatabaseCreator : IDatabaseCreator {
        private DbContext realDbContext;
        public SecurityDatabaseCreator(DbContext dbContext) {
            this.realDbContext = ((BaseSecurityDbContext)dbContext).RealDbContext;
        }
        public bool EnsureCreated() => realDbContext.Database.EnsureCreated();
        public Task<bool> EnsureCreatedAsync(CancellationToken cancellationToken = default(CancellationToken)) => realDbContext.Database.EnsureCreatedAsync(cancellationToken);
        public bool EnsureDeleted() => realDbContext.Database.EnsureDeleted();
        public Task<bool> EnsureDeletedAsync(CancellationToken cancellationToken = default(CancellationToken)) => realDbContext.Database.EnsureDeletedAsync(cancellationToken);
        
    }
}
