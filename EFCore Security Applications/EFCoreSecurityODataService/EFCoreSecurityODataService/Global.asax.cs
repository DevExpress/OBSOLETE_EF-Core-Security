using DevExpress.EntityFramework.SecurityDataStore;
using EFCoreSecurityODataService.Models;
using System.Web.Http;
using System;
using DevExpress.EntityFramework.SecurityDataStore.Security;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EFCoreSecurityODataService {
    public class WebApiApplication : System.Web.HttpApplication, ISecurityApplication
    {
        public ISecurityUser CurrentUser { get; set; }
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            DatabaseUpdate();
        }

        private void DatabaseUpdate() {
            EFCoreDemoDbContext dbContext = new EFCoreDemoDbContext();
            CreateITDepartmentModel(dbContext);
            CreateSalesDepartmentModel(dbContext);
            CreateProductionDepartmentModel(dbContext);
            SecuritySetUp(dbContext);
            dbContext.SaveChanges();
        }

        private void SecuritySetUp(EFCoreDemoDbContext dbContext) {
            SecurityUser user = new SecurityUser() { Name = "John", Password = "John" };
            SecurityRole roleForUser = new SecurityRole();
            SecurityUser admin = new SecurityUser() { Name = "Admin", Password = "Admin" };
            SecurityRole roleForAdmin = new SecurityRole();

            ContactSecuritySetUp(roleForUser);
            DepartmentSecuritySetUp(roleForUser);
            TaskSecuritySetUp(roleForUser);

            UserRole userRole = new UserRole() { Role = roleForUser, User = user };
            UserRole adminRole = new UserRole() { Role = roleForAdmin, User = admin };

            dbContext.Add(userRole);
            dbContext.Add(adminRole);
        }

        private static void TaskSecuritySetUp(SecurityRole roleForUser) {
            // "Note" member of task "TopManagement", "Write" and "Draw" will be denied
            roleForUser.AddMemberPermission<EFCoreDemoDbContext, DemoTask>(SecurityOperation.Read, OperationState.Deny, "Note", (db, obj) => obj.PercentCompleted < 50);
            // Task "Hardcode" will be denied
            roleForUser.AddObjectPermission<EFCoreDemoDbContext, DemoTask>(SecurityOperation.Read, OperationState.Deny, (db, obj) => obj.ContactTasks.Any(p => p.Contact.Name == "John"));
        }

        private static void DepartmentSecuritySetUp(SecurityRole roleForUser) {
            // Department "Sales" will be denied
            roleForUser.AddMemberPermission<EFCoreDemoDbContext, Department>(SecurityOperation.Read, OperationState.Deny, "Office", (db, obj) => obj.Title == "Sales");
            roleForUser.AddObjectPermission<EFCoreDemoDbContext, Department>(SecurityOperation.Read, OperationState.Deny, (db, obj) => obj.Contacts.Contains(db.Contacts.First(c => c.Name == "Barry")));
        }

        private static void ContactSecuritySetUp(SecurityRole roleForUser) {
            // "Address" member of contacts "Jack", "Barry" and "Mike" will be denied
            roleForUser.AddMemberPermission<EFCoreDemoDbContext, Contact>(SecurityOperation.Read, OperationState.Deny, "Address", (db, obj) => obj.Department != null && obj.Department.Office == "Texas");
            // Contacts "Zack", "Marina", "Kate" will be denied
            roleForUser.AddObjectPermission<EFCoreDemoDbContext, Contact>(SecurityOperation.Read, OperationState.Deny, (db, obj) => obj.Department != null && obj.Department.Title == "Sales");
            // Contact "Ezra" will be denied
            roleForUser.AddObjectPermission<EFCoreDemoDbContext, Contact>(SecurityOperation.Read, OperationState.Deny, (db, obj) => obj.ContactTasks.Any(p => p.Task.Description == "Draw"));
        }

        private void CreateSalesDepartmentModel(EFCoreDemoDbContext dbContext) {
            Department salesDepartment = new Department() {
                Title = "Sales", 
                Office = "LA"
            };
            DemoTask sellTask = new DemoTask() {
                Description = "Sell",
                Note = "Good sales are good premium",
                StartDate = new DateTime(2010, 09, 23),
                DateCompleted = new DateTime(2016, 12, 31),
                PercentCompleted = 94
            };
            DemoTask manageTask = new DemoTask() {
                Description = "Manage",
                Note = "Manage personal",
                StartDate = new DateTime(2007, 02, 03),
                DateCompleted = new DateTime(2011, 05, 01),
                PercentCompleted = 100
            };
            DemoTask topManagerTask = new DemoTask() {
                Description = "TopManagement",
                Note = "Manage company",
                StartDate = new DateTime(2015, 08, 14),
                DateCompleted = new DateTime(2018, 11, 21),
                PercentCompleted = 39
            };
            Contact seller = new Contact() {
                Name = "Zack",
                Address = "LA",
                Department = salesDepartment
            };
            Contact manager = new Contact() {
                Name = "Marina",
                Address = "Moscow",
                Department = salesDepartment
            };
            Contact topManager = new Contact() {
                Name = "Kate",
                Address = "Madrid",
                Department = salesDepartment
            };
            ContactTask sellContactTask = new ContactTask() {
                Contact = seller,
                Task = sellTask
            };
            ContactTask manageContactTask = new ContactTask() {
                Contact = manager,
                Task = manageTask
            };
            ContactTask topManagerContactTask = new ContactTask() {
                Contact = topManager,
                Task = topManagerTask
            };
            dbContext.Tasks.Add(sellTask);
            dbContext.Tasks.Add(manageTask);
            dbContext.Tasks.Add(topManagerTask);
            dbContext.ContactTasks.Add(sellContactTask);
            dbContext.ContactTasks.Add(manageContactTask);
            dbContext.ContactTasks.Add(topManagerContactTask);
            dbContext.Contacts.Add(seller);
            dbContext.Contacts.Add(manager);
            dbContext.Contacts.Add(topManager);
            dbContext.Departments.Add(salesDepartment);
        }
        private void CreateProductionDepartmentModel(EFCoreDemoDbContext dbContext) {
            Department productionDepartment = new Department() {
                Title = "Production",
                Office = "Texas"
            };
            DemoTask packingTask = new DemoTask() {
                Description = "Pack",
                Note = "Packaging a products",
                StartDate = new DateTime(1991, 09, 02),
                DateCompleted = new DateTime(2016, 04, 12),
                PercentCompleted = 99
            };
            DemoTask transferTask = new DemoTask() {
                Description = "Transfer",
                Note = "Transfer a products to a customers",
                StartDate = new DateTime(2008, 01, 13),
                DateCompleted = new DateTime(2013, 02, 11),
                PercentCompleted = 100
            };
            DemoTask produceTask = new DemoTask() {
                Description = "Produce",
                Note = "Produce the finished product",
                StartDate = new DateTime(2012, 12, 22),
                DateCompleted = new DateTime(2017, 04, 01),
                PercentCompleted = 75
            };
            Contact packer = new Contact() {
                Name = "Jack",
                Address = "Minessota",
                Department = productionDepartment
            };
            Contact carrier = new Contact() {
                Name = "Barry",
                Address = "Chikago",
                Department = productionDepartment
            };
            Contact producer = new Contact() {
                Name = "Mike",
                Address = "London",
                Department = productionDepartment
            };
            ContactTask packingContactTask = new ContactTask() {
                Contact = packer,
                Task = packingTask
            };
            ContactTask transferContactTask = new ContactTask() {
                Contact = carrier,
                Task = transferTask
            };
            ContactTask produceContactTask = new ContactTask() {
                Contact = producer,
                Task = produceTask
            };
            dbContext.Tasks.Add(packingTask);
            dbContext.Tasks.Add(transferTask);
            dbContext.Tasks.Add(produceTask);
            dbContext.ContactTasks.Add(packingContactTask);
            dbContext.ContactTasks.Add(transferContactTask);
            dbContext.ContactTasks.Add(produceContactTask);
            dbContext.Contacts.Add(packer);
            dbContext.Contacts.Add(carrier);
            dbContext.Contacts.Add(producer);
            dbContext.Departments.Add(productionDepartment);
        }

        private void CreateITDepartmentModel(EFCoreDemoDbContext dbContext) {
            Department itDepartment = new Department() {
                Title = "IT",
                Office = "SiliconValley"
            };
            DemoTask itTask = new DemoTask() {
                Description = "HardCode",
                Note = "This must be perfect code",
                StartDate = new DateTime(2016, 01, 23),
                DateCompleted = new DateTime(2016, 06, 13),
                PercentCompleted = 52
            };
            DemoTask writeTask = new DemoTask() {
                Description = "Write",
                Note = "Write docs",
                StartDate = new DateTime(2015, 09, 14),
                DateCompleted = new DateTime(2018, 07, 18),
                PercentCompleted = 25
            };
            DemoTask designTask = new DemoTask() {
                Description = "Draw",
                Note = "Draw pictures like Picasso",
                StartDate = new DateTime(2016, 04, 03),
                DateCompleted = new DateTime(2020, 11, 04),
                PercentCompleted = 3
            };
            Contact developer = new Contact() {
                Name = "John",
                Address = "Boston",
                Department = null
            };
            Contact writer = new Contact() {
                Name = "Kevin",
                Address = "California",
                Department = itDepartment
            };
            Contact designer = new Contact() {
                Name = "Ezra",
                Address = "San Francisko",
                Department = itDepartment
            };
            ContactTask itContactTask = new ContactTask() {
                Contact = developer,
                Task = itTask
            };
            ContactTask writeContactTask = new ContactTask() {
                Contact = writer,
                Task = writeTask
            };
            ContactTask designContactTask = new ContactTask() {
                Contact = designer,
                Task = designTask
            };
            dbContext.ContactTasks.Add(itContactTask);
            dbContext.ContactTasks.Add(designContactTask);
            dbContext.ContactTasks.Add(writeContactTask);
            dbContext.Tasks.Add(itTask);
            dbContext.Tasks.Add(writeTask);
            dbContext.Tasks.Add(designTask);
            dbContext.Contacts.Add(designer);
            dbContext.Contacts.Add(writer);
            dbContext.Contacts.Add(developer);
            dbContext.Departments.Add(itDepartment);
        }
    }
}
