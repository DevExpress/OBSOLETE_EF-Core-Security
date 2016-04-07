using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevExpress.EntityFramework.SecurityDataStore.Security.BaseSecurityEntity {
    public interface ISecurityEntity {
        [NotMapped]
        IEnumerable<string> BlockedMembers { get; set; }
    }
    public class BaseSecurityEntity : ISecurityEntity {
        [NotMapped]
        public IEnumerable<string> BlockedMembers { get; set; }
    }
}
