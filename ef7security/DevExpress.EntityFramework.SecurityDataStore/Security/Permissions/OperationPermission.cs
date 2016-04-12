using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public class PolicyPermission : IPolicyPermission {
        public PolicyPermission([NotNull] SecurityOperation operations) {
            Operations = operations;
        }
        public SecurityOperation Operations { get; set; }
        public override int GetHashCode() {
            return Operations.GetHashCode();
        }
    }
}
