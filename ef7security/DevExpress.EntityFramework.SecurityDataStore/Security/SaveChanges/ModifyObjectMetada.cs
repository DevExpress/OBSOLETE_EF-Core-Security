using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Security {
    public class ModifyObjectMetada {
        public ModifyObjectMetada(object Object) {
            this.Object = Object;
            ModifiedProperties = new Dictionary<string, object>();
            NavigationProperty = new List<string>();
            ModifiedForeignKey = new Dictionary<string, object>();
        }
        public object Object { get; }
        public Dictionary<string, object> ModifiedProperties { get; }
        public Dictionary<string, object> ModifiedForeignKey { get; }
        public List<string> NavigationProperty { get; }
    }
}
