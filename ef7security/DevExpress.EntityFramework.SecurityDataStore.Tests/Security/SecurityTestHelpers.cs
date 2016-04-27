using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts;
using NUnit.Framework;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.Security {
    class SecurityTestHelper {
        public static void FailSaveChanges(SecurityDbContext dbContextMultiClass) {
            bool withSecurityException = false;
            try {
                dbContextMultiClass.SaveChanges();
            }
            catch {
                withSecurityException = true;
            }
            //catch(Exception e) {
            //    Assert.Fail(e.Message);
            //}
            Assert.IsTrue(withSecurityException);
        }
        public static void InitializeContextWithNavigationProperties() {
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                Company companyFirst = null;               
                for(int i = 1; i < 4; i++) {
                    string indexString = i.ToString();
                    
                    Company company = new Company();
                    company.CompanyName = indexString;
                    company.Description = indexString;

                    Person person = new Person();
                    person.PersonName = indexString;
                    person.Description = indexString;
                    company.Person = person;
                    person.Company = company;
    
                    if(companyFirst == null) {
                        companyFirst = company;
                    }
    
                    companyFirst.Collection.Add(person);
                    dbContextConnectionClass.Company.Add(company);
                    dbContextConnectionClass.Persons.Add(person);
                }
                dbContextConnectionClass.SaveChanges();
            }
        }
        public static void InitializeContextWithNavigationPropertiesAndCollections() {
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                Company companyFirst = null;
                Company companySecond = null;
                for(int i = 1; i < 4; i++) {
                    string indexString = i.ToString();
                    
                    Company company = new Company();
                    company.CompanyName = indexString;
                    company.Description = indexString;

                    Person person = new Person();
                    person.PersonName = indexString;
                    person.Description = indexString;

                    // company.Person = person;
                    // person.Company = company;

                    if(companySecond == null && companyFirst != null) {
                        companySecond = company;
                    }
                    if(companyFirst == null) {
                        companyFirst = company;
                    }

                    companyFirst.Collection.Add(person);
                    if(companySecond != null)
                        companySecond.Collection.Add(person);

                    dbContextConnectionClass.Company.Add(company);
                    dbContextConnectionClass.Persons.Add(person);
                }
                dbContextConnectionClass.SaveChanges();
            }
        }
        public static Expression<Func<DbContextConnectionClass, Company, bool>> CompanyTrue {
            get {
                return (db, company) => true;
            }
        }
        public static Expression<Func<DbContextConnectionClass, Company, bool>> CompanyNameEqualsOne {
            get {
                return (db, company) => company.CompanyName == "1";
            }
        }
        public static Expression<Func<DbContextConnectionClass, Company, bool>> CompanyNameEqualsTwo {
            get {
                return (db, company) => company.CompanyName == "2";
            }
        }
        public static Expression<Func<DbContextConnectionClass, Person, bool>> PersonTrue {
            get {
                return (db, person) => true;
            }
        }
        public static Expression<Func<DbContextConnectionClass, Person, bool>> PersonNameEqualsOne {
            get {
                return (db, person) => person.PersonName == "1";
            }
        }
        public static Expression<Func<DbContextConnectionClass, Person, bool>> PersonNameEqualsTwo {
            get {
                return (db, person) => person.PersonName == "2";
            }
        }
        public static void InitializeData(DbContextManyToManyRelationship dbContext) {
            CreateITDepartment(dbContext);
            CreateSalesDepartment(dbContext);
            CreateProductionDepartment(dbContext);
            dbContext.SaveChanges();
        }

        public static void CreateITDepartment(DbContextManyToManyRelationship dbContext) {
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
            dbContext.Tasks.Add(itTask);
            dbContext.Tasks.Add(writeTask);
            dbContext.Tasks.Add(designTask);
            dbContext.Contacts.Add(designer);
            dbContext.Contacts.Add(writer);
            dbContext.Contacts.Add(developer);
            dbContext.ContactTasks.Add(itContactTask);
            dbContext.ContactTasks.Add(designContactTask);
            dbContext.ContactTasks.Add(writeContactTask);
            dbContext.Departments.Add(itDepartment);
        }

        public static void CreateSalesDepartment(DbContextManyToManyRelationship dbContext) {
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
        public static void CreateProductionDepartment(DbContextManyToManyRelationship dbContext) {
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
        public static void TaskSecuritySetUp(DbContextManyToManyRelationship dbContext) {
            // "Note" member of task "TopManagement", "Write" and "Draw" will be denied
            dbContext.Security.AddMemberPermission<DbContextManyToManyRelationship, DemoTask>(SecurityOperation.Read, OperationState.Deny, "Note", (db, obj) => obj.PercentCompleted < 50);
            // Task "Hardcode" will be denied
            dbContext.Security.AddObjectPermission<DbContextManyToManyRelationship, DemoTask>(SecurityOperation.Read, OperationState.Deny, (db, obj) => obj.ContactTasks.Any(p => p.Contact.Name == "John"));
        }

        public static void DepartmentSecuritySetUp(DbContextManyToManyRelationship dbContext) {
            // Department "Sales" will be denied
            dbContext.Security.AddMemberPermission<DbContextManyToManyRelationship, Department>(SecurityOperation.Read, OperationState.Deny, "Office", (db, obj) => obj.Title == "Sales");
            dbContext.Security.AddObjectPermission<DbContextManyToManyRelationship, Department>(SecurityOperation.Read, OperationState.Deny, (db, obj) => obj.Contacts.Any(c => c.Name == "Barry"));
        }

        public static void ContactSecuritySetUp(DbContextManyToManyRelationship dbContext) {
            // "Address" member of contacts "Jack", "Barry" and "Mike" will be denied
            dbContext.Security.AddMemberPermission<DbContextManyToManyRelationship, Contact>(SecurityOperation.Read, OperationState.Deny, "Address", (db, obj) => obj.Department != null && obj.Department.Office == "Texas");
            // Contacts "Zack", "Marina", "Kate" will be denied
            dbContext.Security.AddObjectPermission<DbContextManyToManyRelationship, Contact>(SecurityOperation.Read, OperationState.Deny, (db, obj) => obj.Department != null && obj.Department.Title == "Sales");
            // Contact "Ezra" will be denied
            dbContext.Security.AddObjectPermission<DbContextManyToManyRelationship, Contact>(SecurityOperation.Read, OperationState.Deny, (db, obj) => obj.ContactTasks.Any(p => p.Task.Description == "Draw"));
        }
    }
}
