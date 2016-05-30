using DevExpress.EntityFramework.SecurityDataStore.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Security {
    public interface ISecurityStrategy {
        IPermissionsProvider PermissionsProvider { get; }
        ISecurityProcessLoadObjects SecurityProcessLoadObjects { get; }
        ISecuritySaveObjects SecuritySaveObjects { get; }
        ISecurityObjectRepository SecurityObjectRepository { get; }
        ISecurityExpressionBuilder SecurityExpressionBuilder { get; }
        IPermissionProcessor PermissionProcessor { get; }
        bool IsGranted(Type type, SecurityOperation operation);
        bool IsGranted(Type type, SecurityOperation operation, object targetObject);
        bool IsGranted(Type type, SecurityOperation operation, object targetObject, string memberName);
    }
}
