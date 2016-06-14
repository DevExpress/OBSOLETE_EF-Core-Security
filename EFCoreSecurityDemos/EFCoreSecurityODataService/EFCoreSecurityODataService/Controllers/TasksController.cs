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
    public class TasksController : ODataController {
        EFCoreDemoDbContext taskContext = new EFCoreDemoDbContext(PermissionsProviderContext.GetPermissionsProvider());
        private bool TaskExists(int key) {
            return taskContext.Tasks.Any(p => p.Id == key);
        }
        protected override void Dispose(bool disposing) {
            taskContext.Dispose();
            base.Dispose(disposing);
        }
        [EnableQuery]
        public IQueryable<DemoTask> Get() {
            IQueryable<DemoTask> result = taskContext.Tasks.
                Include(p => p.ContactTasks).
                ThenInclude(o => o.Contact).
                ThenInclude(c => c.Department);
            return result;
        }
        [EnableQuery]
        public IQueryable<DemoTask> Get([FromODataUri] int key) {
            IQueryable<DemoTask> result = taskContext.Tasks.Include(p => p.ContactTasks).
                ThenInclude(o => o.Contact).
                Where(p => p.Id == key).
                ToArray().
                AsQueryable();
            return result;
        }
        public async Task<IHttpActionResult> Post(DemoTask task) {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            taskContext.Tasks.Add(task);
            await taskContext.SaveChangesAsync();
            return Created(task);
        }
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<DemoTask> task) {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            var entity = await taskContext.Tasks.FirstOrDefaultAsync(p => p.Id == key);
            if(entity == null) {
                return NotFound();
            }
            task.Patch(entity);
            try {
                await taskContext.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException) {
                if(!TaskExists(key)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }
            return Updated(task);
        }
        public async Task<IHttpActionResult> Put([FromODataUri] int key, DemoTask task) {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            if(key != task.Id) {
                return BadRequest();
            }
            taskContext.Entry(task).State = EntityState.Modified;
            try {
                await taskContext.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException) {
                if(!TaskExists(key)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }
            return Updated(task);
        }
        public async Task<IHttpActionResult> Delete([FromODataUri] int key) {
            var task = await taskContext.Tasks.FirstOrDefaultAsync(p => p.Id == key);
            if(task == null) {
                return NotFound();
            }
            taskContext.Tasks.Remove(task);
            await taskContext.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }
        [EnableQuery]
        public IQueryable<ContactTask> GetContactTasks([FromODataUri] int key) {
            IQueryable<ContactTask> result = taskContext.ContactTasks
                .Include(ct => ct.Task)
                .Include(ct => ct.Contact)
                .ThenInclude(c => c.Department)
                .Where(ct => ct.Task.Id == key); 
            return result;
        }
        [AcceptVerbs("POST", "PUT")]
        public async Task<IHttpActionResult> CreateRef([FromODataUri] int key, string navigationProperty, [FromBody] Uri link) {
            DemoTask task = await taskContext.Tasks.SingleOrDefaultAsync(p => p.Id == key);
            if(task == null) {
                return NotFound();
            }
            switch(navigationProperty) {
                case "ContactTasks":
                    // Note: The code for GetKeyFromUri is shown later in this topic.
                    int relatedKey = Helpers.GetKeyFromUri<int>(Request, link);
                    ContactTask contactTask = await taskContext.ContactTasks.SingleOrDefaultAsync(f => f.Id == relatedKey);
                    if(contactTask == null) {
                        return NotFound();
                    }
                    task.ContactTasks.Add(contactTask);
                    break;
                default:
                    return StatusCode(HttpStatusCode.NotImplemented);
            }
            await taskContext.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }
        public async Task<IHttpActionResult> DeleteRef([FromODataUri] int key, [FromODataUri] int relatedKey, string navigationProperty) {
            DemoTask task = await taskContext.Tasks.SingleOrDefaultAsync(p => p.Id == key);
            if(task == null) {
                return StatusCode(HttpStatusCode.NotFound);
            }
            switch(navigationProperty) {
                case "ContactTasks":
                    int contactTaskId = Convert.ToInt32(relatedKey);
                    ContactTask contactTask = await taskContext.ContactTasks.SingleOrDefaultAsync(p => p.Id == contactTaskId);
                    if(contactTask == null) {
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