using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Internal;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public class SecurityQueryContext : QueryContext {
        public SecurityDbContext dbContext;
        // public StateManager stateManager;
        public SecurityQueryContext([NotNull] SecurityDbContext dbContext, [NotNull] Func<IQueryBuffer> queryBufferFactory, [NotNull] IStateManager stateManager, [NotNull] IConcurrencyDetector concurrencyDetector) 
            : base(queryBufferFactory, stateManager, concurrencyDetector) {
            this.dbContext = dbContext;
        }
    }
}
