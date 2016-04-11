using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Security.BusinessEntities {
    public class BaseSecurityObject : ISecurityObject {
        [NotMapped]
        public string BlockedMembers { get; set; }
        [NotMapped]
        public string ReadOnlyMembers { get; set; }
        [NotMapped]
        public string ReadOnlyMembersOnLoad { get; set; }
    }
}
