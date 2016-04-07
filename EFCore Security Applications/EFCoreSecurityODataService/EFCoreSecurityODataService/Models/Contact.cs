using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using DevExpress.EntityFramework.SecurityDataStore.Security.BaseSecurityEntity;

namespace EFCoreSecurityODataService.Models {
    public class Contact : BaseSecurityEntity {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public Department Department { get; set; }
        //[ForeignKey("Position")]
        //public int? PositionId { get; set; }
        //public Position Position { get; set; }
        public List<ContactTask> ContactTasks { get; set; }
    }
}
