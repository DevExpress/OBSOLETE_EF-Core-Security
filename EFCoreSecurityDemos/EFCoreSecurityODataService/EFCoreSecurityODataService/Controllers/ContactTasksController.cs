using DevExpress.EntityFramework.SecurityDataStore.Authorization;
using EFCoreSecurityODataService.DataModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.OData;

namespace EFCoreSecurityODataService.Controllers {
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ContactTasksController : ODataController {
        EFCoreDemoDbContext contactTaskContext = new EFCoreDemoDbContext();
        public ContactTasksController() {
            ISecurityApplication application = HttpContext.Current.ApplicationInstance as ISecurityApplication;
            if(application != null) {
                ISecurityUser user = application.CurrentUser;
                if(user != null) {
                    contactTaskContext.Logon(user);
                }
            }
        }
        private bool ContactTaskExists(int key) {
            return contactTaskContext.ContactTasks.Any(p => p.Id == key);
        }
        protected override void Dispose(bool disposing) {
            contactTaskContext.Dispose();
            base.Dispose(disposing);
        }
        [EnableQuery]
        public IQueryable<ContactTask> Get() {
            IQueryable<ContactTask> result = contactTaskContext.ContactTasks.Include(ct => ct.Task).Include(ct => ct.Contact).ThenInclude(c => c.Department);
            return result;
        }
        [EnableQuery]
        public IQueryable<ContactTask> Get([FromODataUri] int key) {
            IQueryable<ContactTask> result = contactTaskContext.ContactTasks.Include(ct => ct.Task).Include(ct => ct.Contact).ThenInclude(c => c.Department).Where(p => p.Id == key);
            return result;
        }
        public async Task<IHttpActionResult> Post(ContactTask contactTask) {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            contactTaskContext.ContactTasks.Add(contactTask);
            await contactTaskContext.SaveChangesAsync();
            return Created(contactTask);
        }
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<ContactTask> contactTask) {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            var entity = await contactTaskContext.ContactTasks.FirstOrDefaultAsync(p => p.Id == key);
            if(entity == null) {
                return NotFound();
            }
            contactTask.Patch(entity);
            try {
                await contactTaskContext.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException) {
                if(!ContactTaskExists(key)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }
            return Updated(contactTask);
        }
        public async Task<IHttpActionResult> Put([FromODataUri] int key, ContactTask contactTask) {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            if(key != contactTask.Id) {
                return BadRequest();
            }
            contactTaskContext.Entry(contactTask).State = EntityState.Modified;
            try {
                await contactTaskContext.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException) {
                if(!ContactTaskExists(key)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }
            return Updated(contactTask);
        }
        public async Task<IHttpActionResult> Delete([FromODataUri] int key) {
            var contactTask = await contactTaskContext.ContactTasks.FirstOrDefaultAsync(p => p.Id == key);
            if(contactTask == null) {
                return NotFound();
            }
            contactTaskContext.ContactTasks.Remove(contactTask);
            await contactTaskContext.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }
        [EnableQuery]
        public IQueryable<Contact> GetContact([FromODataUri] int key) {
            IQueryable<Contact> result = Enumerable.Empty<Contact>().AsQueryable();
            IQueryable<ContactTask> contactTasks = contactTaskContext.ContactTasks
                .Include(ct => ct.Task).Include(ct => ct.Contact).ThenInclude(c => c.Department).Where(ct => ct.Id == key);
            if(contactTasks.Count() > 0) {
                ContactTask contactTask = contactTasks.First();
                if(contactTask.Contact != null) {
                    result = contactTaskContext.Contacts
                            .Include(c => c.ContactTasks)
                            .Where(d => d.Id == contactTask.Contact.Id);
                } 
            }
            return result;
        }
        [EnableQuery]
        public IQueryable<DemoTask> GetTask([FromODataUri] int key) {
            IQueryable<DemoTask> result = Enumerable.Empty<DemoTask>().AsQueryable();
            IQueryable<ContactTask> contactTasks = contactTaskContext.ContactTasks
                .Include(ct => ct.Task).Include(ct => ct.Contact).ThenInclude(c => c.Department).Where(ct => ct.Id == key);
            if(contactTasks.Count() > 0) {
                ContactTask contactTask = contactTasks.First();
                if(contactTask.Task != null) {
                    result = contactTaskContext.Tasks
                            .Include(c => c.ContactTasks)
                            .Where(d => d.Id == contactTask.Task.Id);
                } 
            }
            return result;
        }
        [AcceptVerbs("POST", "PUT")]
        public async Task<IHttpActionResult> CreateRef([FromODataUri] int key, string navigationProperty, [FromBody] Uri link) {
            ContactTask contactTask = await contactTaskContext.ContactTasks.SingleOrDefaultAsync(p => p.Id == key);
            if(contactTask == null) {
                return NotFound();
            }
            switch(navigationProperty) {
                case "Contact":
                    // Note: The code for GetKeyFromUri is shown later in this topic.
                    int relatedKey = Helpers.GetKeyFromUri<int>(Request, link);
                    Contact contact = await contactTaskContext.Contacts.SingleOrDefaultAsync(f => f.Id == relatedKey);
                    if(contact == null) {
                        return NotFound();
                    }
                    contactTask.Contact = contact;
                    break;
                case "Task":
                    // Note: The code for GetKeyFromUri is shown later in this topic.
                    relatedKey = Helpers.GetKeyFromUri<int>(Request, link);
                    DemoTask task = await contactTaskContext.Tasks.SingleOrDefaultAsync(f => f.Id == relatedKey);
                    if(task == null) {
                        return NotFound();
                    }
                    contactTask.Task = task;
                    break;
                default:
                    return StatusCode(HttpStatusCode.NotImplemented);
            }
            await contactTaskContext.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }
        public async Task<IHttpActionResult> DeleteRef([FromODataUri] int key, [FromODataUri] int relatedKey, string navigationProperty) {
            ContactTask contactTask = await contactTaskContext.ContactTasks.SingleOrDefaultAsync(p => p.Id == key);
            if(contactTask == null) {
                return StatusCode(HttpStatusCode.NotFound);
            }
            switch(navigationProperty) {
                case "Contact":
                    int contactId = Convert.ToInt32(relatedKey);
                    Contact contact = await contactTaskContext.Contacts.SingleOrDefaultAsync(p => p.Id == contactId);
                    if(contact == null) {
                        return NotFound();
                    }
                    contactTask.Contact = null;
                    break;
                case "Task":
                    int taskId = Convert.ToInt32(relatedKey);
                    DemoTask task = await contactTaskContext.Tasks.SingleOrDefaultAsync(p => p.Id == taskId);
                    if(task == null) {
                        return NotFound();
                    }
                    contactTask.Task = null;
                    break;
                default:
                    return StatusCode(HttpStatusCode.NotImplemented);
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}