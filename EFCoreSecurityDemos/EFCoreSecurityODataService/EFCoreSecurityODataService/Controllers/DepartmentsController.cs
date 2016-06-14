using DevExpress.EntityFramework.SecurityDataStore.Authorization;
using DevExpress.EntityFramework.SecurityDataStore.Security;
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
    public class DepartmentsController : ODataController {
        private EFCoreDemoDbContext departmentContext = new EFCoreDemoDbContext(PermissionsProviderContext.GetPermissionsProvider());
        private bool DepartmentExists(int key) {
            return departmentContext.Departments.Any(p => p.Id == key);
        }
        protected override void Dispose(bool disposing) {
            departmentContext.Dispose();
            base.Dispose(disposing);
        }
        [EnableQuery]
        public IQueryable<Department> Get() {
            IQueryable<Department> result = departmentContext.Departments.
                Include(p => p.Contacts).
                ThenInclude(c => c.ContactTasks).
                ThenInclude(ct => ct.Task);
            return result;
        }
        [EnableQuery]
        public IQueryable<Department> Get([FromODataUri] int key) {
            IQueryable<Department> result = departmentContext.Departments.
                Where(p => p.Id == key).
                Include(d => d.Contacts).
                ThenInclude(c => c.ContactTasks).
                ThenInclude(ct => ct.Task).
                ToArray().
                AsQueryable();
            return result;
        }
        public async Task<IHttpActionResult> Post(Department department) {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            departmentContext.Departments.Add(department);
            await departmentContext.SaveChangesAsync();
            return Created(department);
        }
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Department> department) {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            var entity = await departmentContext.Departments.FirstOrDefaultAsync(p => p.Id == key);
            if(entity == null) {
                return NotFound();
            }
            department.Patch(entity);
            try {
                await departmentContext.SaveChangesAsync();
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
            departmentContext.Entry(department).State = EntityState.Modified;
            try {
                await departmentContext.SaveChangesAsync();
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
            var task = await departmentContext.Departments.FirstOrDefaultAsync(p => p.Id == key);
            if(task == null) {
                return NotFound();
            }
            departmentContext.Departments.Remove(task);
            await departmentContext.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }
        [EnableQuery]
        public IQueryable<Contact> GetContacts([FromODataUri] int key) {
            IQueryable<Contact> result = departmentContext.Contacts
                .Include(c => c.Department)
                .Include(c => c.ContactTasks)
                .ThenInclude(ct => ct.Task)
                .Where(p => p.Department.Id == key);
            return result;
        }
        [AcceptVerbs("POST", "PUT")]
        public async Task<IHttpActionResult> CreateRef([FromODataUri] int key, string navigationProperty, [FromBody] Uri link) {
            Department department = await departmentContext.Departments.SingleOrDefaultAsync(p => p.Id == key);
            if(department == null) {
                return NotFound();
            }
            switch(navigationProperty) {
                case "Contacts":
                    int relatedKey = Helpers.GetKeyFromUri<int>(Request, link);
                    Contact contact = await departmentContext.Contacts.SingleOrDefaultAsync(f => f.Id == relatedKey);
                    if(contact == null) {
                        return NotFound();
                    }
                    department.Contacts.Add(contact);
                    break;

                default:
                    return StatusCode(HttpStatusCode.NotImplemented);
            }
            await departmentContext.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }
        public async Task<IHttpActionResult> DeleteRef([FromODataUri] int key, [FromODataUri] int relatedKey, string navigationProperty) {
            Department department = await departmentContext.Departments.SingleOrDefaultAsync(p => p.Id == key);
            if(department == null) {
                return StatusCode(HttpStatusCode.NotFound);
            }
            switch(navigationProperty) {
                case "Contacts":
                    int contactId = Convert.ToInt32(relatedKey);
                    Contact contact = await departmentContext.Contacts.SingleOrDefaultAsync(p => p.Id == contactId);
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