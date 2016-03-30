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
        private SecurityDbContext dbContext;
        public SecurityDatabaseCreator(DbContext dbContext) {
            this.dbContext = ((SecurityDbContext)dbContext).realDbContext;
        }
        public bool EnsureCreated() => dbContext.Database.EnsureCreated();
        public Task<bool> EnsureCreatedAsync(CancellationToken cancellationToken = default(CancellationToken)) => dbContext.Database.EnsureCreatedAsync(cancellationToken);
        public bool EnsureDeleted() => dbContext.Database.EnsureDeleted();
        public Task<bool> EnsureDeletedAsync(CancellationToken cancellationToken = default(CancellationToken)) => dbContext.Database.EnsureDeletedAsync(cancellationToken);
        
    }
}
