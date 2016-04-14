using DevExpress.EntityFramework.SecurityDataStore;
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
    public class TasksController : ODataController {
        EFCoreDemoDbContext dbContext = new EFCoreDemoDbContext();
        public TasksController() {
            ISecurityApplication application = HttpContext.Current.ApplicationInstance as ISecurityApplication;
            if(application != null) {
                ISecurityUser user = application.CurrentUser;
                if(user != null) {
                    dbContext.Logon(user);
                }
            }
        }
        private bool TaskExists(int key) {
            return dbContext.Tasks.Any(p => p.Id == key);
        }
        protected override void Dispose(bool disposing) {
            dbContext.Dispose();
            base.Dispose(disposing);
        }
        [EnableQuery]
        public IQueryable<DemoTask> Get() {
            IQueryable<DemoTask> result = dbContext.Tasks.Include(p => p.ContactTasks).ThenInclude(o => o.Contact);
            return result;
        }
        [EnableQuery]
        public SingleResult<DemoTask> Get([FromODataUri] int key) {
            IQueryable<DemoTask> result = dbContext.Tasks.Where(p => p.Id == key);
            return SingleResult.Create(result);
        }
        public async Task<IHttpActionResult> Post(DemoTask task) {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            dbContext.Tasks.Add(task);
            await dbContext.SaveChangesAsync();
            return Created(task);
        }
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<DemoTask> task) {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            var entity = await dbContext.Tasks.FirstOrDefaultAsync(p => p.Id == key);
            if(entity == null) {
                return NotFound();
            }
            task.Patch(entity);
            try {
                await dbContext.SaveChangesAsync();
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
            dbContext.Entry(task).State = EntityState.Modified;
            try {
                await dbContext.SaveChangesAsync();
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
            var task = await dbContext.Tasks.FirstOrDefaultAsync(p => p.Id == key);
            if(task == null) {
                return NotFound();
            }
            dbContext.Tasks.Remove(task);
            await dbContext.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }
        [EnableQuery]
        public IQueryable<ContactTask> GetContactTasks([FromODataUri] int key) {
            return dbContext.Tasks.Where(p => p.Id.Equals(key)).SelectMany(m => m.ContactTasks);
        }
        [AcceptVerbs("POST", "PUT")]
        public async Task<IHttpActionResult> CreateRef([FromODataUri] int key, string navigationProperty, [FromBody] Uri link) {
            DemoTask task = await dbContext.Tasks.SingleOrDefaultAsync(p => p.Id == key);
            if(task == null) {
                return NotFound();
            }
            switch(navigationProperty) {
                case "ContactTasks":
                    // Note: The code for GetKeyFromUri is shown later in this topic.
                    int relatedKey = Helpers.GetKeyFromUri<int>(Request, link);
                    ContactTask contactTask = await dbContext.ContactTasks.SingleOrDefaultAsync(f => f.Id == relatedKey);
                    if(contactTask == null) {
                        return NotFound();
                    }
                    task.ContactTasks.Add(contactTask);
                    break;
                default:
                    return StatusCode(HttpStatusCode.NotImplemented);
            }
            await dbContext.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }
        public async Task<IHttpActionResult> DeleteRef([FromODataUri] int key, [FromODataUri] int relatedKey, string navigationProperty) {
            DemoTask task = await dbContext.Tasks.SingleOrDefaultAsync(p => p.Id == key);
            if(task == null) {
                return StatusCode(HttpStatusCode.NotFound);
            }
            switch(navigationProperty) {
                case "ContactTasks":
                    int contactTaskId = Convert.ToInt32(relatedKey);
                    ContactTask contactTask = await dbContext.ContactTasks.SingleOrDefaultAsync(p => p.Id == contactTaskId);
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