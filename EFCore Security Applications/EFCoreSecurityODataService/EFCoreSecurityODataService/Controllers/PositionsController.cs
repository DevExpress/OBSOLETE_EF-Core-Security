//using EFCoreSecurityODataService.Models;
//using Microsoft.EntityFrameworkCore;
//using System.Linq;
//using System.Net;
//using System.Threading.Tasks;
//using System.Web.Http;
//using System.Web.OData;

//namespace EFCoreSecurityODataService.Controllers {
//    public class PositionsController : ODataController {
//        EFCoreDemoDbContext dbContext = new EFCoreDemoDbContext();
//        private bool PositionExists(int key) {
//            return dbContext.Positions.Any(p => p.Id == key);
//        }
//        protected override void Dispose(bool disposing) {
//            dbContext.Dispose();
//            base.Dispose(disposing);
//        }
//        [EnableQuery]
//        public IQueryable<Position> Get() {
//            return dbContext.Positions;
//        }
//        [EnableQuery]
//        public SingleResult<Position> Get([FromODataUri] int key) {
//            IQueryable<Position> result = dbContext.Positions.Where(p => p.Id == key);
//            return SingleResult.Create(result);
//        }
//        public async Task<IHttpActionResult> Post(Position position) {
//            if(!ModelState.IsValid) {
//                return BadRequest(ModelState);
//            }
//            dbContext.Positions.Add(position);
//            await dbContext.SaveChangesAsync();
//            return Created(position);
//        }
//        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Position> position) {
//            if(!ModelState.IsValid) {
//                return BadRequest(ModelState);
//            }
//            var entity = await dbContext.Positions.FirstOrDefaultAsync(p => p.Id == key);
//            if(entity == null) {
//                return NotFound();
//            }
//            position.Patch(entity);
//            try {
//                await dbContext.SaveChangesAsync();
//            }
//            catch(DbUpdateConcurrencyException) {
//                if(!PositionExists(key)) {
//                    return NotFound();
//                }
//                else {
//                    throw;
//                }
//            }
//            return Updated(position);
//        }
//        public async Task<IHttpActionResult> Put([FromODataUri] int key, Position position) {
//            if(!ModelState.IsValid) {
//                return BadRequest(ModelState);
//            }
//            if(key != position.Id) {
//                return BadRequest();
//            }
//            dbContext.Entry(position).State = EntityState.Modified;
//            try {
//                await dbContext.SaveChangesAsync();
//            }
//            catch(DbUpdateConcurrencyException) {
//                if(!PositionExists(key)) {
//                    return NotFound();
//                }
//                else {
//                    throw;
//                }
//            }
//            return Updated(position);
//        }
//        public async Task<IHttpActionResult> Delete([FromODataUri] int key) {
//            var task = await dbContext.Positions.FirstOrDefaultAsync(p => p.Id == key);
//            if(task == null) {
//                return NotFound();
//            }
//            dbContext.Positions.Remove(task);
//            await dbContext.SaveChangesAsync();
//            return StatusCode(HttpStatusCode.NoContent);
//        }
//        [EnableQuery]
//        public IQueryable<Department> GetDepartments([FromODataUri] int key) {
//            return dbContext.Positions.Where(p => p.Id.Equals(key)).SelectMany(m => m.Departments);
//        }
//    }
//}