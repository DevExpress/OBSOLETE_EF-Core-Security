using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using DevExpress.EntityFramework.SecurityDataStore.Security;

namespace DevExpress.EntityFramework.SecurityDataStore.Security {
    public class MemberPermission<TSource, TargetType> : IMemberPermission where TSource : BaseSecurityDbContext {
        public MemberPermission(SecurityOperation operations, OperationState operationState, string memberName, Expression<Func<TSource, TargetType, bool>> criteria) {
            Type = typeof(TargetType);
            Operations = operations;
            OperationState = operationState;
            MemberName = memberName;
            Criteria = criteria;
        }
        public string MemberName { get; set; }
        public Type Type { get; set; }
        public OperationState OperationState { get; set; }
        public SecurityOperation Operations { get; set; }
        public Expression<Func<TSource, TargetType, bool>> Criteria { get; set; }
        LambdaExpression IMemberPermission.Criteria {
            get {
                return Criteria;
            }
            set {
                Criteria = (Expression<Func<TSource, TargetType, bool>>)value;
            }
        }
        public override int GetHashCode() {
            return (Type.Name + Operations + Operations.ToString()).GetHashCode();
        }
    }
}
