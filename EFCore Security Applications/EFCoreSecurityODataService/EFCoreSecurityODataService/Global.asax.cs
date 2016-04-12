using DevExpress.EntityFramework.SecurityDataStore;
using EFCoreSecurityODataService.Models;
using System.Web.Http;
using System;

namespace EFCoreSecurityODataService {
    public class WebApiApplication : System.Web.HttpApplication
    {
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
            dbContext.SaveChanges();
        }

        private void CreateSalesDepartmentModel(EFCoreDemoDbContext dbContext) {
            Department salesDepartment = new Department() {
                Title = "Sales", 
                Office = "LA"
            };
            DemoTask sellTask = new DemoTask() {
                Description = "Sell",
                Note = "Good sales are good premium"
            };
            DemoTask manageTask = new DemoTask() {
                Description = "Manage",
                Note = "Manage personal"
            };
            DemoTask topManagerTask = new DemoTask() {
                Description = "TopManagement",
                Note = "Manage company"
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
                Note = "Packaging a products"
            };
            DemoTask transferTask = new DemoTask() {
                Description = "Transfer",
                Note = "Transfer a products to a customers"
            };
            DemoTask produceTask = new DemoTask() {
                Description = "Produce",
                Note = "Produce the finished product"
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
                Note = "This must be perfect code"
            };
            DemoTask writeTask = new DemoTask() {
                Description = "Write",
                Note = "Write docs"
            };
            DemoTask designTask = new DemoTask() {
                Description = "Draw",
                Note = "Draw pictures like Picasso"
            };
            Contact developer = new Contact() {
                Name = "John",
                Address = "Boston",
                Department = itDepartment
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
