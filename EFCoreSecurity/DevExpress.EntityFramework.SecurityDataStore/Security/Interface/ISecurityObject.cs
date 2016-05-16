using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public interface ISecurityObject {
        string ReadOnlyMembersOnLoad { get; set; }
        string ReadOnlyMembers { get; set; }
        string BlockedMembers { get; set; }
    }
}
