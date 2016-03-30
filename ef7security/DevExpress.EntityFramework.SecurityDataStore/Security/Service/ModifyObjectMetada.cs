using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Security.Service {
    public class ModifyObjectMetada {
        public object SecurityObject { get; set; }
        public object RealObject { get; set; }
        public List<string> ModifyMembers { get; } 
            = new List<string>();
    }
}
