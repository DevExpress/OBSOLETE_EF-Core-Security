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
        // private IStateManager stateManager;
        private IConcurrencyDetector concurrencyDetector;
        // private BaseSecurityDbContext dbContext;
        private ICurrentDbContext currentContext;
        
        public SecurityQueryContextFactory([NotNull] ICurrentDbContext currentContext, [NotNull] IConcurrencyDetector concurrencyDetector) : base(currentContext, concurrencyDetector) {
            this.currentContext = currentContext;
            this.stateManager = stateManager;
            this.concurrencyDetector = concurrencyDetector;
        }

        public override QueryContext Create() {
            return new SecurityQueryContext(dbContext, CreateQueryBuffer, stateManager, concurrencyDetector);
        }
    }
}
