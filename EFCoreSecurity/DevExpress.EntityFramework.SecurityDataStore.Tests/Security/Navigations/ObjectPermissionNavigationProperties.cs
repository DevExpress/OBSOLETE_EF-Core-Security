using DevExpress.EntityFramework.SecurityDataStore.Security;
using DevExpress.EntityFramework.SecurityDataStore.Tests.DbContexts;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.EntityFramework.SecurityDataStore.Tests.Security {
    [TestFixture]
    public class ObjectPermissionNavigationProperties {
        [SetUp]
        public void SetUp() {
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                dbContextConnectionClass.Database.EnsureCreated();
            }
        }
        [TearDown]
        public void TearDown() {
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                dbContextConnectionClass.Database.EnsureDeleted();
            }
        }
        // TODO: fixme
        /*
        [Test]
        public void Read_PolicyAllow_TestDependencyPrincipalProperty() {
            SecurityTestHelper.InitializeContextWithNavigationProperties();
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                Company company = dbContextConnectionClass.Company.Include(p => p.Person).Include(p => p.Offices).First(p => p.CompanyName == "1");
                Assert.IsNotNull(company.Person);
                Assert.AreEqual(3, company.Offices.Count);
                Person persons = dbContextConnectionClass.Persons.Include(p => p.Company).First(p => p.PersonName == "1");
                Assert.IsNotNull(persons.Company);
                Assert.IsNotNull(persons.One);
            }
        }
        */
        [Test]
        public void Read_PolicyDeny_OneObjectAllow() {
            SecurityTestHelper.InitializeContextWithNavigationProperties();
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                dbContextConnectionClass.Security.PermissionsContainer.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);
                dbContextConnectionClass.Security.PermissionsContainer.AddObjectPermission(SecurityOperation.Read, OperationState.Allow, SecurityTestHelper.CompanyNameEqualsOne);
                Assert.AreEqual(1, dbContextConnectionClass.Company.Count());
            }
        }

        [Test]
        public void Read_PolicyAllow_OneObjectDeny() {
            SecurityTestHelper.InitializeContextWithNavigationProperties();
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                dbContextConnectionClass.Security.PermissionsContainer.SetPermissionPolicy(PermissionPolicy.AllowAllByDefault);
                dbContextConnectionClass.Security.PermissionsContainer.AddObjectPermission(SecurityOperation.Read, OperationState.Deny, SecurityTestHelper.CompanyNameEqualsOne);
                Assert.AreEqual(2, dbContextConnectionClass.Company.Count());
            }
        }

        [Test]
        public void Read_PolicyDeny_OneObjectAllow_IncludeNavigateObject() {
            SecurityTestHelper.InitializeContextWithNavigationProperties();
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                dbContextConnectionClass.Security.PermissionsContainer.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);
                dbContextConnectionClass.Security.PermissionsContainer.AddObjectPermission(SecurityOperation.Read, OperationState.Allow, SecurityTestHelper.CompanyNameEqualsOne);
                Assert.AreEqual(1, dbContextConnectionClass.Company.Include(p => p.Person).Count());
                Assert.AreEqual(0, dbContextConnectionClass.Persons.Include(p => p.Company).Count());
                Company company = dbContextConnectionClass.Company.Include(p => p.Person).First();

                Assert.IsNull(company.Person);
            }
        }
        [Test]
        public void Read_PolicyAllow_OneObjectDeny_IncludeNavigateObject() {
            SecurityTestHelper.InitializeContextWithNavigationProperties();
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                dbContextConnectionClass.Security.PermissionsContainer.SetPermissionPolicy(PermissionPolicy.AllowAllByDefault);
                dbContextConnectionClass.Security.PermissionsContainer.AddObjectPermission(SecurityOperation.Read, OperationState.Deny, SecurityTestHelper.PersonNameEqualsOne);
                Assert.AreEqual(3, dbContextConnectionClass.Company.Include(p => p.Person).Count());
                Assert.AreEqual(2, dbContextConnectionClass.Persons.Include(p => p.Company).Count());

                Company company1 = dbContextConnectionClass.Company.Include(p => p.Person).First(p => p.CompanyName == "1");
                Assert.IsNull(company1.Person);
                Company company2 = dbContextConnectionClass.Company.Include(p => p.Person).First(p => p.CompanyName == "2");
                Assert.IsNotNull(company2.Person);
            }
        }
        [Test]
        public void Read_PolicyDeny_OneObjectAllow_IncludeNavigateObject_CheckCollection() {
            SecurityTestHelper.InitializeContextWithNavigationProperties();
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                dbContextConnectionClass.Security.PermissionsContainer.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);
                dbContextConnectionClass.Security.PermissionsContainer.AddObjectPermission(SecurityOperation.Read, OperationState.Allow, SecurityTestHelper.CompanyNameEqualsOne);

                Company company1 = dbContextConnectionClass.Company.Include(p => p.Person).Include(p => p.Offices).Single();
                Assert.IsNull(company1.Person);
                Assert.AreEqual(0, company1.Offices.Count);

                dbContextConnectionClass.Security.PermissionsContainer.AddObjectPermission(SecurityOperation.Read, OperationState.Allow, SecurityTestHelper.OfficeNameEqualsOne);

                company1 = dbContextConnectionClass.Company.Include(p => p.Person).Include(p => p.Offices).Single();

                Assert.AreEqual(1, company1.Offices.Count);
                // Assert.IsNotNull(company1.Person);

                Office offices = dbContextConnectionClass.Offices.Include(p => p.Company).Single();

                Assert.IsNotNull(offices.Company);
            }
        }
        [Test]
        public void Read_PolicyDeny_OneObjectAllow_IncludeNavigateObject_SaveChanges() {
            SecurityTestHelper.InitializeContextWithNavigationProperties();
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                dbContextConnectionClass.Security.PermissionsContainer.SetPermissionPolicy(PermissionPolicy.DenyAllByDefault);
                dbContextConnectionClass.Security.PermissionsContainer.AddObjectPermission(SecurityOperation.ReadWrite, OperationState.Allow, SecurityTestHelper.CompanyNameEqualsOne);

                Company company1 = dbContextConnectionClass.Company.Include(p => p.Person).Single(d => d.CompanyName == "1");
                Assert.IsNull(company1.Person);

                company1.Description = "5";
                dbContextConnectionClass.SaveChanges();
            }

            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                var company1 = dbContextConnectionClass.Company.Include(p => p.Person).First(p => p.Description == "5");
                Assert.IsNotNull(company1.Person);
            }
        }
        [Test]
        public void Read_PolicyAllow_OneObjectDeny_IncludeNavigateObject_SaveChanges() {
            SecurityTestHelper.InitializeContextWithNavigationProperties();
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                dbContextConnectionClass.Security.PermissionsContainer.SetPermissionPolicy(PermissionPolicy.AllowAllByDefault);
                dbContextConnectionClass.Security.PermissionsContainer.AddObjectPermission(SecurityOperation.ReadWrite, OperationState.Deny, SecurityTestHelper.PersonNameEqualsOne);

                Company company1 = dbContextConnectionClass.Company.Include(p => p.Person).Single(d => d.CompanyName == "1");
                Assert.IsNull(company1.Person);

                company1.Description = "5";
                dbContextConnectionClass.SaveChanges();
            }

            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                var company1 = dbContextConnectionClass.Company.Include(p => p.Person).First(p => p.Description == "5");
                Assert.IsNotNull(company1.Person);
            }
        }
        [Test]
        public void Read_OneObjectDeny_ObjectCount() {
            SecurityTestHelper.InitializeContextWithNavigationProperties();
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                dbContextConnectionClass.Security.PermissionsContainer.AddObjectPermission(SecurityOperation.ReadWrite, OperationState.Deny, SecurityTestHelper.OfficeNameEqualsOne);
                Company company = dbContextConnectionClass.Company.Include(p => p.Offices).Single(d => d.CompanyName == "1");
                Assert.AreEqual(2, company.Offices.Count);
            }
        }
        [Test]
        public void Read_Collection_ObjectCount() {
            SecurityTestHelper.InitializeContextWithNavigationProperties();
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                //Company company = dbContextConnectionClass.Company.Include(p => p.Person).First(p => p.CompanyName == "1");
                //Assert.AreEqual(1, company.Offices.Count);
                //company = dbContextConnectionClass.Company.Include(p => p.Offices).First(p => p.CompanyName == "1");
                //Assert.AreEqual(3, company.Offices.Count);

                Company company = dbContextConnectionClass.Company.Include(p => p.Person).First(p => p.CompanyName == "1");
                Assert.AreEqual(1, company.Offices.Count, company.CompanyName);
                company = dbContextConnectionClass.Company.Include(p => p.Offices).First(p => p.CompanyName == "1");
                Assert.AreEqual(3, company.Offices.Count);
            }
        }
        [Test]
        public void Modify_FakeCollectionObject() {
            SecurityTestHelper.InitializeContextWithNavigationProperties();
            using(DbContextConnectionClass dbContextConnectionClass = new DbContextConnectionClass()) {
                dbContextConnectionClass.Security.PermissionsContainer.AddMemberPermission(SecurityOperation.Read, OperationState.Deny, "Name", SecurityTestHelper.OfficeTrue);
                Company company = dbContextConnectionClass.Company.Include(p => p.Offices).First(p => p.CompanyName == "1");
                Office office = dbContextConnectionClass.Offices.First();
                Assert.IsNull(office.Name);
                Office officeFromCollection = company.Offices.First();
                Assert.IsNull(officeFromCollection.Name);
            }
        }
        [Test]
        public void Read_Collection_NestedCriteria() {
            using(DbContextConnectionClass dbContext = new DbContextConnectionClass()) {
                Company company = new Company() {
                    CompanyName = "Pixar"
                };
                Office office = new Office() {
                    Name = "London",
                    Company = company
                };
                dbContext.Offices.Add(office);
                dbContext.Company.Add(company);
                dbContext.Security.PermissionsContainer.AddObjectPermission<DbContextConnectionClass, Company>(SecurityOperation.Read, OperationState.Deny, (db, obj) => obj.Offices.Any(p => p.Name == "London"));
                dbContext.SaveChanges();

                Assert.IsNull(dbContext.Company.Where(p => p.CompanyName == "Pixar").FirstOrDefault());
            }
        }
        [Test]
        public void ReadCompanyPerson_WhenPersonsIsDeny() {
            SecurityTestHelper.InitializeContextWithNavigationProperties();
            using(DbContextConnectionClass dbContext = new DbContextConnectionClass()) {
                dbContext.Security.PermissionsContainer.AddObjectPermission<DbContextConnectionClass, Office>(SecurityOperation.Read, OperationState.Deny, (db, obj) => obj.Name == "1");
                Company company = dbContext.Company.Include(p => p.Offices).First(p => p.CompanyName == "1");

                Assert.AreEqual(2, company.Offices.Count);
                Assert.IsFalse(company.Offices.Any(c => c.Name == "1"));
                Assert.AreEqual(null, company.Person);
            }
        }
        [Test]
        public void ReadTaskContactDepartment() {
            using(DbContextManyToManyRelationship dbContext = new DbContextManyToManyRelationship()) {
                SecurityTestHelper.InitializeData(dbContext);
                DemoTask task = dbContext.Tasks.Include(p => p.ContactTasks).ThenInclude(ct => ct.Contact).ThenInclude(c => c.Department).First(p => p.Description == "Draw");

                Assert.IsNotNull(task);
                Assert.IsNotEmpty(task.ContactTasks);
                Assert.IsNotNull(task.ContactTasks.First());
                Assert.IsNotNull(task.ContactTasks.First().Contact);
                Assert.AreEqual(task.ContactTasks.First().Contact.Name, "Ezra");
                Assert.IsNotNull(task.ContactTasks.First().Contact.Department);
                Assert.AreEqual(task.ContactTasks.First().Contact.Department.Title, "IT");
            }
        }
        [Test]
        public void ReadTaskContactDepartment_WhenContactIsDeny() {
            using(DbContextManyToManyRelationship dbContext = new DbContextManyToManyRelationship()) {
                SecurityTestHelper.InitializeData(dbContext);
                dbContext.Security.PermissionsContainer.AddObjectPermission<DbContextManyToManyRelationship, Contact>(SecurityOperation.Read, OperationState.Deny, (db, obj) => obj.ContactTasks.Any(ct => ct.Task.Description == "Draw"));
                DemoTask task = dbContext.Tasks.Include(p => p.ContactTasks).ThenInclude(ct => ct.Contact).ThenInclude(c => c.Department).First(p => p.Description == "Draw");

                Assert.IsNotNull(task);
                Assert.IsNotEmpty(task.ContactTasks);
                Assert.IsNotNull(task.ContactTasks.First());
                Assert.IsNull(task.ContactTasks.First().Contact);
            }
        }
        [Test]
        public void EFSecurityDemoTest_ReadAllData() {
            using(DbContextManyToManyRelationship dbContext = new DbContextManyToManyRelationship()) {
                SecurityTestHelper.InitializeData(dbContext);
                SecurityTestHelper.ContactSecuritySetUp(dbContext);
                SecurityTestHelper.DepartmentSecuritySetUp(dbContext);
                SecurityTestHelper.TaskSecuritySetUp(dbContext);
                dbContext.SaveChanges();

                int contactsnativeCriteriaCount = dbContext.GetRealDbContext().Contacts.
                    Include(c => c.Department).Include(c => c.ContactTasks).ThenInclude(ct => ct.Task).
                    Where(obj => !(obj.Department != null && obj.Department.Title == "Sales")).
                    Where(obj => obj.ContactTasks.Any(p => !(p.Task.Description == "Draw"))).Count();

                IEnumerable<Contact> contacts = dbContext.Contacts.Include(c => c.Department).Include(c => c.ContactTasks).ThenInclude(ct => ct.Task);
                Assert.AreEqual(contactsnativeCriteriaCount, contacts.Count());
                IEnumerable<Department> departments = dbContext.Departments.Include(d => d.Contacts).ThenInclude(c => c.ContactTasks).ThenInclude(ct => ct.Task);
                CheckDepartmentsData(departments);
                IEnumerable<DemoTask> tasks = dbContext.Tasks.Include(t => t.ContactTasks).ThenInclude(ct => ct.Contact).ThenInclude(c => c.Department);
                CheckTasksData(tasks);
            }
        }

        private void CheckTasksData(IEnumerable<DemoTask> tasks) {
            Assert.AreEqual(tasks.Count(), 8);
            foreach(DemoTask task in tasks) {
                switch(task.Description) {
                    case "Sell":
                        Assert.AreEqual(task.Note, "Good sales are good premium");
                        Assert.AreEqual(task.BlockedMembers.Count(), 0);
                        Assert.AreEqual(task.ContactTasks.Count, 1);
                        Assert.IsNull(task.ContactTasks.First().Contact);
                        break;
                    case "Manage":
                        Assert.AreEqual(task.Note, "Manage personal");
                        Assert.AreEqual(task.BlockedMembers.Count(), 0);
                        Assert.AreEqual(task.ContactTasks.Count, 1);
                        Assert.IsNull(task.ContactTasks.First().Contact);
                        break;
                    case "TopManagement":
                        Assert.IsNull(task.Note);
                        Assert.AreEqual(task.BlockedMembers.Count(), 1);
                        Assert.AreEqual(task.BlockedMembers.First(), "Note");
                        Assert.AreEqual(task.ContactTasks.Count, 1);
                        Assert.IsNull(task.ContactTasks.First().Contact);
                        break;
                    case "Pack":
                        Assert.AreEqual(task.Note, "Packaging a products");
                        Assert.AreEqual(task.BlockedMembers.Count(), 0);
                        Assert.AreEqual(task.ContactTasks.Count, 1);
                        Contact jack = task.ContactTasks.First().Contact;
                        Assert.AreEqual(jack.Name, "Jack");
                        Assert.IsNull(jack.Department);
                        Assert.IsNull(jack.Address);
                        break;
                    case "Transfer":
                        Assert.AreEqual(task.Note, "Transfer a products to a customers");
                        Assert.AreEqual(task.BlockedMembers.Count(), 0);
                        Assert.AreEqual(task.ContactTasks.Count, 1);
                        Contact barry = task.ContactTasks.First().Contact;
                        Assert.AreEqual(barry.Name, "Barry");
                        Assert.IsNull(barry.Department);
                        Assert.IsNull(barry.Address);
                        break;
                    case "Produce":
                        Assert.AreEqual(task.Note, "Produce the finished product");
                        Assert.AreEqual(task.BlockedMembers.Count(), 0);
                        Assert.AreEqual(task.ContactTasks.Count, 1);
                        Contact mike = task.ContactTasks.First().Contact;
                        Assert.AreEqual(mike.Name, "Mike");
                        Assert.IsNull(mike.Department);
                        Assert.IsNull(mike.Address);
                        break;
                    case "Write":
                        Assert.IsNull(task.Note);
                        Assert.AreEqual(task.BlockedMembers.First(), "Note");
                        Assert.AreEqual(task.ContactTasks.Count, 1);
                        Contact kevin = task.ContactTasks.First().Contact;
                        Assert.AreEqual(kevin.Name, "Kevin");
                        Assert.AreEqual(kevin.Department.Title, "IT");
                        Assert.AreEqual(kevin.Address, "California");
                        Assert.AreEqual(kevin.Department.Contacts.Count, 1);
                        break;
                    case "Draw":
                        Assert.IsNull(task.Note);
                        Assert.AreEqual(task.BlockedMembers.First(), "Note");
                        Assert.AreEqual(task.ContactTasks.Count, 1);
                        Assert.IsNull(task.ContactTasks.First().Contact);
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            }
        }

        private void CheckDepartmentsData(IEnumerable<Department> departments) {
            Assert.AreEqual(departments.Count(), 2);
            foreach(Department department in departments) {
                switch(department.Title) {
                    case "IT":
                        Assert.AreEqual(department.BlockedMembers.Count(), 0);
                        Assert.AreEqual(department.Contacts.Count, 1);
                        Contact contact = department.Contacts.First();
                        Assert.AreEqual(contact.Name, "Kevin");
                        Assert.AreEqual(contact.ContactTasks.Count, 1);
                        Assert.AreEqual(contact.ContactTasks.First().Task.Description, "Write");
                        Assert.AreEqual(department.Office, "SiliconValley");
                        break;
                    case "Sales":
                        Assert.AreEqual(department.BlockedMembers.Count(), 1);
                        Assert.AreEqual(department.BlockedMembers.First(), "Office");
                        Assert.IsNull(department.Contacts);
                        Assert.IsNull(department.Office);
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            }
        }

        private void CheckContactsData(IEnumerable<Contact> contacts) {
            Assert.AreEqual(contacts.Count(), 5); // must be 5, but contact don`t load when department is null. Thus contacts.Count equal 4.
            foreach(Contact contact in contacts) {
                switch(contact.Name) {
                    case "John":
                        Assert.AreEqual(contact.BlockedMembers.Count(), 0);
                        Assert.AreEqual(contact.Address, "Boston");
                        Assert.IsNull(contact.Department);
                        Assert.AreEqual(contact.ContactTasks.Count, 1);
                        Assert.IsNull(contact.ContactTasks.First().Task);
                        break;
                    case "Jack":
                        Assert.AreEqual(contact.BlockedMembers.Count(), 2);
                        Assert.IsTrue(contact.BlockedMembers.Contains("Address"));
                        Assert.IsTrue(contact.BlockedMembers.Contains("Department"));
                        Assert.IsNull(contact.Address);
                        Assert.IsNull(contact.Department);
                        Assert.AreEqual(contact.ContactTasks.Count, 1);
                        Assert.IsNotNull(contact.ContactTasks.First().Task);
                        Assert.AreEqual(contact.ContactTasks.First().Task.Description, "Pack");
                        break;
                    case "Kevin":
                        Assert.AreEqual(contact.BlockedMembers.Count(), 0);
                        Assert.AreEqual(contact.Address, "California");
                        Assert.AreEqual(contact.Department.Title, "IT");
                        Assert.AreEqual(contact.ContactTasks.Count, 1);
                        Assert.AreEqual(contact.ContactTasks.First().Task.Description, "Write");
                        break;
                    case "Barry":
                        Assert.AreEqual(contact.BlockedMembers.Count(), 2);
                        Assert.IsTrue(contact.BlockedMembers.Contains("Address"));
                        Assert.IsTrue(contact.BlockedMembers.Contains("Department"));
                        Assert.IsNull(contact.Address);
                        Assert.IsNull(contact.Department);
                        Assert.AreEqual(contact.ContactTasks.Count, 1);
                        Assert.IsNotNull(contact.ContactTasks.First().Task);
                        Assert.AreEqual(contact.ContactTasks.First().Task.Description, "Transfer");
                        break;
                    case "Mike":
                        Assert.AreEqual(contact.BlockedMembers.Count(), 2);
                        Assert.IsTrue(contact.BlockedMembers.Contains("Address"));
                        Assert.IsTrue(contact.BlockedMembers.Contains("Department"));
                        Assert.IsNull(contact.Address);
                        Assert.IsNull(contact.Department);
                        Assert.AreEqual(contact.ContactTasks.Count, 1);
                        Assert.IsNotNull(contact.ContactTasks.First().Task);
                        Assert.AreEqual(contact.ContactTasks.First().Task.Description, "Produce");
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            }
        }
    }
}
