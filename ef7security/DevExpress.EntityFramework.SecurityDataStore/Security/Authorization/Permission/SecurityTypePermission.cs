using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Security {
    public class SecurityTypePermission : ITypePermission, ISecurityTypePermission {
        public Guid ID { get; set; }
        [NotMapped]
        private CriteriaSerializer criteriaSerializer = new CriteriaSerializer();
        public SecurityOperation Operations { get; set; }
        public OperationState OperationState { get; set; }
        public string StringType { get; set; }
        [NotMapped]
        public Type Type {
            get {

                return ((ParameterExpression)criteriaSerializer.Deserialize(StringType)).Type;
            }
            set {
                StringType = criteriaSerializer.Serialize(Expression.Parameter(value));
            }
        }
        public SecurityRole SecurityRole { get; set; }
        public Guid SecurityRoleID { get; set; }
    }
}
