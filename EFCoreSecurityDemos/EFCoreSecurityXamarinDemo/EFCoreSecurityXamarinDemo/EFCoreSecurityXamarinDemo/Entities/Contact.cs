using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreSecurityXamarinDemo.Entities {
    public class Contact : BaseSecurityEntity {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Department { get; set; }
    }
}
