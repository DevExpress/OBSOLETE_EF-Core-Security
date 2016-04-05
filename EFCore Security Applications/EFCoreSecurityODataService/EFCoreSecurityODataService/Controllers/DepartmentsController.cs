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
    public class DepartmentsController : ODataController {
        EFCoreDemoDbContext dbContext;
        public DepartmentsController() {
            dbContext = new EFCoreDemoDbContext();
            dbContext.Security.AddObjectPermission<EFCoreDemoDbContext, Department>(SecurityOperation.Read, OperationState.Deny, (db, obj) => obj.Contacts.Any(p => p.Address == "LA"));
        }
        private bool DepartmentExists(int key) {
            return dbContext.Departments.Any(p => p.Id == key);
        }
        protected override void Dispose(bool disposing) {
            dbContext.Dispose();
            base.Dispose(disposing);
        }
        [EnableQuery]
        public IQueryable<Department> Get() {
            IQueryable<Department> result = dbContext.Departments.Include(p => p.Contacts);
            return result;
        }
        [EnableQuery]
        public SingleResult<Department> Get([FromODataUri] int key) {
            IQueryable<Department> result = dbContext.Departments.Where(p => p.Id == key);
            return SingleResult.Create(result);
        }
        public async Task<IHttpActionResult> Post(Department department) {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            dbContext.Departments.Add(department);
            await dbContext.SaveChangesAsync();
            return Created(department);
        }
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Department> department) {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            var entity = await dbContext.Departments.FirstOrDefaultAsync(p => p.Id == key);
            if(entity == null) {
                return NotFound();
            }
            department.Patch(entity);
            try {
                await dbContext.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException) {
                if(!DepartmentExists(key)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }
            return Updated(department);
        }
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Department department) {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            if(key != department.Id) {
                return BadRequest();
            }
            dbContext.Entry(department).State = EntityState.Modified;
            try {
                await dbContext.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException) {
                if(!DepartmentExists(key)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }
            return Updated(department);
        }
        public async Task<IHttpActionResult> Delete([FromODataUri] int key) {
            var task = await dbContext.Departments.FirstOrDefaultAsync(p => p.Id == key);
            if(task == null) {
                return NotFound();
            }
            dbContext.Departments.Remove(task);
            await dbContext.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }
        [EnableQuery]
        public IQueryable<Contact> GetContacts([FromODataUri] int key) {
            return dbContext.Departments.Where(p => p.Id.Equals(key)).SelectMany(m => m.Contacts);
        }
        //[EnableQuery]
        //public IQueryable<Position> GetPositions([FromODataUri] int key) {
        //    return dbContext.Departments.Where(p => p.Id.Equals(key)).SelectMany(m => m.Positions);
        //}
        [AcceptVerbs("POST", "PUT")]
        public async Task<IHttpActionResult> CreateRef([FromODataUri] int key, string navigationProperty, [FromBody] Uri link) {
            Department department = await dbContext.Departments.SingleOrDefaultAsync(p => p.Id == key);
            if(department == null) {
                return NotFound();
            }
            switch(navigationProperty) {
                case "Contacts":
                    // Note: The code for GetKeyFromUri is shown later in this topic.
                    int relatedKey = Helpers.GetKeyFromUri<int>(Request, link);
                    Contact contact = await dbContext.Contacts.SingleOrDefaultAsync(f => f.Id == relatedKey);
                    if(contact == null) {
                        return NotFound();
                    }
                    department.Contacts.Add(contact);
                    break;

                default:
                    return StatusCode(HttpStatusCode.NotImplemented);
            }
            await dbContext.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }
        public async Task<IHttpActionResult> DeleteRef([FromODataUri] int key, [FromODataUri] int relatedKey, string navigationProperty) {
            Department department = await dbContext.Departments.SingleOrDefaultAsync(p => p.Id == key);
            if(department == null) {
                return StatusCode(HttpStatusCode.NotFound);
            }
            switch(navigationProperty) {
                case "Contacts":
                    int contactId = Convert.ToInt32(relatedKey);
                    Contact contact = await dbContext.Contacts.SingleOrDefaultAsync(p => p.Id == contactId);
                    if(contact == null) {
                        return NotFound();
                    }
                    contact.Department = null;
                    break;
                default:
                    return StatusCode(HttpStatusCode.NotImplemented);
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}