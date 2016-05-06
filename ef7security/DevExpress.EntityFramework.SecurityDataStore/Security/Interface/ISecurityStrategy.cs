using DevExpress.EntityFramework.SecurityDataStore.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public interface ISecurityStrategy {
        TypePermission FindFirstTypePermission<T>() where T : class;
        TypePermission FindFirstTypePermission(Type type);        
        TypePermission SetTypePermission(Type type, SecurityOperation operation, OperationState state);
        ObjectPermission<TSource, TargetType> AddObjectPermission<TSource, TargetType>(SecurityOperation operation, OperationState state, Expression<Func<TSource, TargetType, bool>> criteria) where TSource : SecurityDbContext;
        MemberPermission<TSource, TargetType> AddMemberPermission<TSource, TargetType>(SecurityOperation operation, OperationState state, string memberName, Expression<Func<TSource, TargetType, bool>> criteria) where TSource : SecurityDbContext;
        bool RemovePermission(IPermission permission);
        IEnumerable<IPermission> GetAllPermissions();
        void AddPermission(IPermission permission);
        void ClearPermissions();
        IPermission SetPermissionPolicy(PermissionPolicy policy);
        bool IsGranted(Type type, SecurityOperation operation);
        bool IsGranted(Type type, SecurityOperation operation, object targetObject);
        bool IsGranted(Type type, SecurityOperation operation, object targetObject, string memberName);
        ISecurityServicesProvider SecurityServicesProvider { get; }
    }
}
