using DevExpress.EntityFramework.SecurityDataStore;
using EFCoreSecurityODataService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace EFCoreSecurityODataService.Controllers {
    public class ContactTasksController : ODataController {
        EFCoreDemoDbContext dbContext;
        public ContactTasksController() {
            dbContext = new EFCoreDemoDbContext();
            //dbContext.Security.AddObjectPermission<EFCoreDemoDbContext, Contact>(SecurityOperation.Read, OperationState.Deny, (db, obj) => obj.Name == "John");
            //dbContext.Security.AddObjectPermission<EFCoreDemoDbContext, ContactTask>(SecurityOperation.Read, OperationState.Deny, (db, obj) => obj.Contact.Name == "John");
        }
        private bool ContactTaskExists(int key) {
            return dbContext.ContactTasks.Any(p => p.Id == key);
        }
        protected override void Dispose(bool disposing) {
            dbContext.Dispose();
            base.Dispose(disposing);
        }
        [EnableQuery]
        public IQueryable<ContactTask> Get() {
            IQueryable<ContactTask> result = dbContext.ContactTasks;
            return result;
        }
        [EnableQuery]
        public SingleResult<ContactTask> Get([FromODataUri] int key) {
            IQueryable<ContactTask> result = dbContext.ContactTasks.Where(p => p.Id == key).Include(p => p.Task).Include(p => p.Contact);
            return SingleResult.Create(result);
        }
        public async Task<IHttpActionResult> Post(ContactTask contactTask) {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            dbContext.ContactTasks.Add(contactTask);
            await dbContext.SaveChangesAsync();
            return Created(contactTask);
        }
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<ContactTask> contactTask) {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            var entity = await dbContext.ContactTasks.FirstOrDefaultAsync(p => p.Id == key);
            if(entity == null) {
                return NotFound();
            }
            contactTask.Patch(entity);
            try {
                await dbContext.SaveChangesAsync();
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
            dbContext.Entry(contactTask).State = EntityState.Modified;
            try {
                await dbContext.SaveChangesAsync();
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
            var contactTask = await dbContext.ContactTasks.FirstOrDefaultAsync(p => p.Id == key);
            if(contactTask == null) {
                return NotFound();
            }
            dbContext.ContactTasks.Remove(contactTask);
            await dbContext.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }
        [EnableQuery]
        public SingleResult<Contact> GetContact([FromODataUri] int key) {
            IQueryable<Contact> result = dbContext.ContactTasks.Where(p => p.Id == key).Select(m => m.Contact);
            return SingleResult.Create(result);
        }
        [EnableQuery]
        public SingleResult<DemoTask> GetTask([FromODataUri] int key) {
            IQueryable<DemoTask> result = dbContext.ContactTasks.Where(p => p.Id == key).Select(m => m.Task);
            return SingleResult.Create(result);
        }
        [AcceptVerbs("POST", "PUT")]
        public async Task<IHttpActionResult> CreateRef([FromODataUri] int key, string navigationProperty, [FromBody] Uri link) {
            ContactTask contactTask = await dbContext.ContactTasks.SingleOrDefaultAsync(p => p.Id == key);
            if(contactTask == null) {
                return NotFound();
            }
            switch(navigationProperty) {
                case "Contact":
                    // Note: The code for GetKeyFromUri is shown later in this topic.
                    int relatedKey = Helpers.GetKeyFromUri<int>(Request, link);
                    Contact contact = await dbContext.Contacts.SingleOrDefaultAsync(f => f.Id == relatedKey);
                    if(contact == null) {
                        return NotFound();
                    }
                    contactTask.Contact = contact;
                    break;
                case "Task":
                    // Note: The code for GetKeyFromUri is shown later in this topic.
                    relatedKey = Helpers.GetKeyFromUri<int>(Request, link);
                    DemoTask task = await dbContext.Tasks.SingleOrDefaultAsync(f => f.Id == relatedKey);
                    if(task == null) {
                        return NotFound();
                    }
                    contactTask.Task = task;
                    break;
                default:
                    return StatusCode(HttpStatusCode.NotImplemented);
            }
            await dbContext.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }
        public async Task<IHttpActionResult> DeleteRef([FromODataUri] int key, [FromODataUri] int relatedKey, string navigationProperty) {
            ContactTask contactTask = await dbContext.ContactTasks.SingleOrDefaultAsync(p => p.Id == key);
            if(contactTask == null) {
                return StatusCode(HttpStatusCode.NotFound);
            }
            switch(navigationProperty) {
                case "Contact":
                    int contactId = Convert.ToInt32(relatedKey);
                    Contact contact = await dbContext.Contacts.SingleOrDefaultAsync(p => p.Id == contactId);
                    if(contact == null) {
                        return NotFound();
                    }
                    contactTask.Contact = null;
                    break;
                case "Task":
                    int taskId = Convert.ToInt32(relatedKey);
                    DemoTask task = await dbContext.Tasks.SingleOrDefaultAsync(p => p.Id == taskId);
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