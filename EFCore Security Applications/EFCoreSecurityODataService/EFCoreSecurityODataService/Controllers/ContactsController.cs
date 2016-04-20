using DevExpress.EntityFramework.SecurityDataStore;
using DevExpress.EntityFramework.SecurityDataStore.Security;
using EFCoreSecurityODataService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.OData;

namespace EFCoreSecurityODataService.Controllers {
    public class ContactsController : ODataController {
        EFCoreDemoDbContext dbContext = new EFCoreDemoDbContext();
        public ContactsController() {
            ISecurityApplication application = HttpContext.Current.ApplicationInstance as ISecurityApplication;
            if(application != null) {
                ISecurityUser user = application.CurrentUser;
                if(user != null) {
                    dbContext.Logon(user);
                }
            }
        }
        private bool ContactExists(int key) {
            return dbContext.Contacts.Any(p => p.Id == key);
        }
        protected override void Dispose(bool disposing) {
            dbContext.Dispose();
            base.Dispose(disposing);
        }
        public IQueryable<Contact> Get() {
            IQueryable<Contact> result = dbContext.Contacts
                .Include(c => c.Department)
                .Include(c => c.ContactTasks)
                .ThenInclude(ct => ct.Task);
            return result;
        }
        [EnableQuery]
        public SingleResult<Contact> Get([FromODataUri] int key) {
            IQueryable<Contact> result = dbContext.Contacts
                .Where(p => p.Id == key)
                .Include(p => p.Department)
                .Include(c => c.ContactTasks)
                .ThenInclude(ct => ct.Task);
            return SingleResult.Create(result);
        }
        public async Task<IHttpActionResult> Post(Contact contact) {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            dbContext.Contacts.Add(contact);
            await dbContext.SaveChangesAsync();
            return Created(contact);
        }
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Contact> contact) {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            var entity = await dbContext.Contacts.FirstOrDefaultAsync(p => p.Id == key);
            if(entity == null) {
                return NotFound();
            }
            contact.Patch(entity);
            try {
                await dbContext.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException) {
                if(!ContactExists(key)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }
            return Updated(contact);
        }
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Contact contact) {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            if(key != contact.Id) {
                return BadRequest();
            }
            dbContext.Entry(contact).State = EntityState.Modified;
            try {
                await dbContext.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException) {
                if(!ContactExists(key)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }
            return Updated(contact);
        }
        public async Task<IHttpActionResult> Delete([FromODataUri] int key) {
            var contact = await dbContext.Contacts.FirstOrDefaultAsync(p => p.Id == key);
            if(contact == null) {
                return NotFound();
            }
            dbContext.Contacts.Remove(contact);
            await dbContext.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }
        [EnableQuery]
        public SingleResult<Department> GetDepartment([FromODataUri] int key) {
            IQueryable<Contact> contacts = dbContext.Contacts
                .Include(c => c.Department).Include(c => c.ContactTasks).ThenInclude(ct => ct.Task).Where(c => c.Id == key);
            Contact contact = contacts.First();
            IQueryable<Department> result = dbContext.Departments
                .Include(p => p.Contacts)
                .ThenInclude(c => c.ContactTasks)
                .ThenInclude(ct => ct.Task)
                .Where(d => d.Id == contact.Department.Id);
            return SingleResult.Create(result);
        }
        [EnableQuery]
        public IQueryable<ContactTask> GetContactTasks([FromODataUri] int key) {
            IQueryable<ContactTask> result = dbContext.Contacts.Where(p => p.Id == key).SelectMany(p => p.ContactTasks);
            return result;
        }
        [AcceptVerbs("POST", "PUT")]
        public async Task<IHttpActionResult> CreateRef([FromODataUri] int key, string navigationProperty, [FromBody] Uri link) {
            Contact contact = await dbContext.Contacts.SingleOrDefaultAsync(p => p.Id == key);
            if(contact == null) {
                return NotFound();
            }
            switch(navigationProperty) {
                case "Department":
                    int relatedKey = Helpers.GetKeyFromUri<int>(Request, link);
                    Department department = await dbContext.Departments.SingleOrDefaultAsync(p => p.Id == relatedKey);
                    if(department == null) {
                        return NotFound();
                    }
                    contact.Department = department;
                    break;
                case "ContactTasks":
                    relatedKey = Helpers.GetKeyFromUri<int>(Request, link);
                    ContactTask task = await dbContext.ContactTasks.SingleOrDefaultAsync(p => p.Id == relatedKey);
                    if(task == null) {
                        return NotFound();
                    }
                    contact.ContactTasks.Add(task);
                    break;
                default:
                    return StatusCode(HttpStatusCode.NotImplemented);
            }
            await dbContext.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }
        public async Task<IHttpActionResult> DeleteRef([FromODataUri] int key, string navigationProperty, [FromBody] Uri link) {
            Contact contact = await dbContext.Contacts.SingleOrDefaultAsync(p => p.Id == key);
            if(contact == null) {
                return NotFound();
            }
            switch(navigationProperty) {
                case "Department":
                    contact.Department = null;
                    break;
                default:
                    return StatusCode(HttpStatusCode.NotImplemented);
            }
            await dbContext.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}