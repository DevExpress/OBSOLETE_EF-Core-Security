using EFCoreSecurityODataService.DataModel;
using System.Linq;
using System.Web;
using System.Web.Http.Cors;
using System.Web.OData;
using DevExpress.EntityFramework.SecurityDataStore.Security;

namespace EFCoreSecurityODataService.Controllers {
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ContactsController : ODataController {
        private EFCoreDemoDbContext Context = new EFCoreDemoDbContext(PermissionsProviderContext.GetPermissionsProvider());
        [EnableQuery]
        public IQueryable<Contact> Get() {
            IQueryable<Contact> result = Context.Contacts;
            return result;
        }
        [EnableQuery]
        public IQueryable<Contact> Get([FromODataUri] int key) {
            IQueryable<Contact> result = Context.Contacts.
                    Where(p => p.Id == key).
                    ToArray().
                    AsQueryable();
            return result;
        }
    }
}