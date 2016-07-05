using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreSecurityXamarinDemo.Entities {
    public class Department : BaseSecurityEntity {
        public string Title { get; set; }
        public string Office { get; set; }
    }
}
