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
            //SecuritySetup(dbContext);
            dbContext.SaveChanges();
        }

        private void SecuritySetup(EFCoreDemoDbContext dbContext) {
            dbContext.Security.AddMemberPermission<EFCoreDemoDbContext, Contact>(SecurityOperation.Read, OperationState.Deny, "Address", (db, obj) => obj.Name == "John");
            dbContext.Security.AddObjectPermission<EFCoreDemoDbContext, Contact>(SecurityOperation.Read, OperationState.Deny, (db, obj) => obj.Department.Title == "Sales");
        }

        private void CreateSalesDepartmentModel(EFCoreDemoDbContext dbContext) {
            DemoTask saleTask = new DemoTask() {
                Description = "Sell",
                Note = "Good sales are good premium"
            };
            Department salesDepartment = new Department() {
                Title = "Sales", 
                Office = "LA"
            };
            Contact seller = new Contact() {
                Name = "Zack",
                Address = "LA",
                Department = salesDepartment
            };
            ContactTask contactTask = new ContactTask() {
                Contact = seller,
                Task = saleTask
            };
            dbContext.Tasks.Add(saleTask);
            dbContext.ContactTasks.Add(contactTask);
            dbContext.Contacts.Add(seller);
            dbContext.Departments.Add(salesDepartment);
        }

        private void CreateITDepartmentModel(EFCoreDemoDbContext dbContext) {
            DemoTask itTask = new DemoTask() {
                Description = "HardCode",
                Note = "This must be perfect code"
            };
            DemoTask designTask = new DemoTask() {
                Description = "Draw",
                Note = "Draw pictures like Picasso"
            };
            Department itDepartment = new Department() {
                Title = "IT",
                Office = "SiliconValley"
            };
            Contact developer = new Contact() {
                Name = "John",
                Address = "Boston",
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
            ContactTask designContactTask = new ContactTask() {
                Contact = designer,
                Task = designTask
            };
            dbContext.ContactTasks.Add(itContactTask);
            dbContext.ContactTasks.Add(designContactTask);
            dbContext.Tasks.Add(itTask);
            dbContext.Tasks.Add(designTask);
            dbContext.Contacts.Add(designer);
            dbContext.Contacts.Add(developer);
            dbContext.Departments.Add(itDepartment);
        }
    }
}
