using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DevExpress.EntityFramework.SecurityDataStore.Security.BaseSecurityEntity;

namespace EFCoreSecurityODataService.Models {
    public class Department : BaseSecurityEntity {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Office { get; set; }
        public List<Contact> Contacts { get; set; }
    }
}