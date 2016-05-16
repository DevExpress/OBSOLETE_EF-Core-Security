using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace DevExpress.EntityFramework.SecurityDataStore.Security {
    public class ObjectPermission<TSource, TargetType> : IObjectPermission where TSource : SecurityDbContext {
            public ObjectPermission(Expression<Func<TSource, TargetType, bool>> criteria) {
            Type = typeof(TargetType);
            Criteria = criteria;
        }        
        public Type Type { get; set; }
        public OperationState OperationState { get; set; }
        public SecurityOperation Operations { get; set; }
        public Expression<Func<TSource, TargetType, bool>> Criteria { get; set; }
        LambdaExpression IObjectPermission.Criteria {
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
