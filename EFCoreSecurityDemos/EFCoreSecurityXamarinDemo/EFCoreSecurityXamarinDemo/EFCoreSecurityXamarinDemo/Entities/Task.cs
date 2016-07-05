using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreSecurityXamarinDemo.Entities {
    public class Task : BaseSecurityEntity {
        public string Description { get; set; }
        public string Note { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DateCompleted { get; set; }
        public int PercentCompleted { get; set; }
    }
}
