using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Security {
    public interface IPermissionsProvider {
        IEnumerable<IPermission> GetPermissions();
    }
    public interface IPermissionsContainer {     
        ITypePermission SetTypePermission(Type type, SecurityOperation operation, OperationState state);
        IObjectPermission AddObjectPermission<TSource, TargetType>(SecurityOperation operation, OperationState state, Expression<Func<TSource, TargetType, bool>> criteria) where TSource : SecurityDbContext;
        IMemberPermission AddMemberPermission<TSource, TargetType>(SecurityOperation operation, OperationState state, string memberName, Expression<Func<TSource, TargetType, bool>> criteria) where TSource : SecurityDbContext;
        bool RemovePermission(IPermission permission);        
        void AddPermission(IPermission permission);
        void ClearPermissions();
        IEnumerable<IPermission> GetPermissions();
        IPermission SetPermissionPolicy(PermissionPolicy policy);
    }
}
