using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public class MemberPermission<TSource, TargetType> : IMemberPermission where TSource : SecurityDbContext {
        public MemberPermission(string memberName, Expression<Func<TSource, TargetType, bool>> criteria) {
            Type = typeof(TargetType);
            Criteria = criteria;
            MemberName = memberName;
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
