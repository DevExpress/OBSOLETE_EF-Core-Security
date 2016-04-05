using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EFCoreSecurityODataService.Models {
    public class Department {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Office { get; set; }
        public List<Contact> Contacts { get; set; }
        //public ICollection<Position> Positions { get; set; }
    }
}