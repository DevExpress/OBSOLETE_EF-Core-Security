using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public class SecurityQueryContextFactory : QueryContextFactory {
        private IStateManager stateManager;
        private IConcurrencyDetector concurrencyDetector;
        private BaseSecurityDbContext dbContext;
        public SecurityQueryContextFactory([NotNull] DbContext dbContext, [NotNull] IStateManager stateManager, [NotNull] IConcurrencyDetector concurrencyDetector, [NotNull] IChangeDetector changeDetector) : base(stateManager, concurrencyDetector, changeDetector) {
            this.dbContext = (BaseSecurityDbContext)dbContext;
            this.stateManager = stateManager;
            this.concurrencyDetector = concurrencyDetector;
        }

        public override QueryContext Create() {
            return new SecurityQueryContext(dbContext, CreateQueryBuffer, stateManager, concurrencyDetector);
        }
    }
}
