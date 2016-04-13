using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore {
    public interface ISecurityApplication {
        ISecurityUser CurrentUser { get; set; }
    }
}
