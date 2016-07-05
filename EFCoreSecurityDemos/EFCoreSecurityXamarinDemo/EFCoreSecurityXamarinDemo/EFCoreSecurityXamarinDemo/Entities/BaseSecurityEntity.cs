using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreSecurityXamarinDemo.Entities {
    public class BaseSecurityEntity {
        public List<string> BlockedMembers { get; set; }
        public int Id { get; set; }
    }
}
