using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public static class SecurityStrategyExtensions {
        public static TypePermission SetTypePermission<T>(this ISecurityStrategy securityStrateg, SecurityOperation operation, OperationState state) where T : class {
            return securityStrateg.SetTypePermission(typeof(T), operation, state);
        }
    }
}
