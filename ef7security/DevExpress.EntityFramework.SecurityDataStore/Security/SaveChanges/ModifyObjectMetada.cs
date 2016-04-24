using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Security {
    public class ModifiedObjectMetada {
        public ModifiedObjectMetada(object Object) {
            this.Object = Object;
            Properties = new Dictionary<string, object>();
            NavigationProperties = new List<string>();
            ForeignKeys = new Dictionary<string, object>();
        }
        public object Object { get; }
        public Dictionary<string, object> Properties { get; }
        public Dictionary<string, object> ForeignKeys { get; }
        public List<string> NavigationProperties { get; }
    }
}
