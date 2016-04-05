using DevExpress.EntityFramework.SecurityDataStore.Security;
using DevExpress.EntityFramework.SecurityDataStore.Security.Services;
using DevExpress.EntityFramework.SecurityDataStore.Utility;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public class SecurityStrategy : ISecurityStrategy {
        private SecurityDbContext securityDbContext;
        public virtual ISecurityServicesProvider SecurityServicesProvider { get; }
        public IList<IPermission> SecurityPermissions { get; } = new List<IPermission>();
#if DebugTest
        public SecurityDbContext GetDbContext() {
            return securityDbContext;
        }
#endif      
        public bool IsGranted(Type type, SecurityOperation operation, object targetObject, string memberName) {
            return SecurityServicesProvider.PermissionProcessor.IsGranted(type, operation, targetObject, memberName);
        }       
        public SecurityStrategy(DbContext dbContext) {
            securityDbContext = ((SecurityDbContext)dbContext);
            SecurityServicesProvider = new SecurityServicesProvider(securityDbContext, SecurityPermissions);
        }
    }
}
