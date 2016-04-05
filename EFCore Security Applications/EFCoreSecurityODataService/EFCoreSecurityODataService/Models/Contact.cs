using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCoreSecurityODataService.Models {
    public class Contact {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public Department Department { get; set; }
        //[ForeignKey("Position")]
        //public int? PositionId { get; set; }
        //public Position Position { get; set; }
        public virtual ICollection<ContactTask> ContactTasks { get; set; }
    }
}
