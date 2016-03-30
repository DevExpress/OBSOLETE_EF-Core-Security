using DevExpress.EntityFramework.SecurityDataStore.Security;
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
        private PermissionProcessor permissionProcessor;
        private SecurityDbContext securityDbContext;
        public IList<IPermission> SecurityPermissions { get; } = new List<IPermission>();
        public SecurityObjectsBuilder SecurityObjectsBuilder { get; }
        public SecurityDbContext GetDbContext() {
            return securityDbContext;
        }
        public bool IsGranted(Type type, SecurityOperation operation) {
            return permissionProcessor.IsGranted(type, operation, null);
        }
        public bool IsGranted(Type type, SecurityOperation operation, object targetObject) {
            return permissionProcessor.IsGranted(type, operation, targetObject);
        }
        public bool IsGranted(Type type, SecurityOperation operation, object targetObject, string memberName) {
            return permissionProcessor.IsGranted(type, operation, targetObject, memberName);
        }
        public Expression SetExpressionCriteriaFromType(Expression sourceExpression, Type type) {
            return permissionProcessor.SetExpressionReadCriteriaFromSecurity(sourceExpression, type);
        }
        public SecurityStrategy(DbContext securityDbContext, SecurityObjectsBuilder securityObjectsBuilder, PermissionProcessor permissionProcessor) {
            this.securityDbContext = ((SecurityDbContext)securityDbContext);
            this.SecurityObjectsBuilder = securityObjectsBuilder;
            this.permissionProcessor = permissionProcessor;
            permissionProcessor.SetPermissions(SecurityPermissions);
        }
    }
}
