using System;
using System.Collections.Generic;
using System.Linq;
using EFCoreSecurityODataService.DataModel;
using Microsoft.EntityFrameworkCore;
// using ExpressionSerialization;
namespace EFCoreSecurityConsoleDemo {
    class Program {
        static void Main(string[] args) {
            Updater.UpdateDatabase();
            string enterResult = "";
            while(enterResult != "Exit") {
                Console.WriteLine("Choose an action: ");
                Console.WriteLine("1. LogIn");
                Console.WriteLine("2. Exit");
                switch(Console.ReadLine()) {
                    case "1":
                        Logon();
                        break;
                    case "2":
                        enterResult = "Exit";
                        break;
                    default:
                        break;
                }
            }
        }
        private static void Logon() {
            using(EFCoreDemoDbContext dbContext = new EFCoreDemoDbContext()) {
                Console.WriteLine("Username: ");
                string userName = Console.ReadLine();
                Console.WriteLine("Please enter password: ");
                string password = Console.ReadLine();
                try {
                    dbContext.Logon(userName, password);
                    ExecuteCRUDOperations(dbContext);
                }
                catch(Exception exception) {
                    Console.WriteLine(exception.Message);
                }
            }
        }

        private static void ExecuteCRUDOperations(EFCoreDemoDbContext dbContext) {
            string enterResult = "";
            while(enterResult != "LogOff") {
                Console.WriteLine("Choose an action:");
                Console.WriteLine(" 1. Create new data");
                Console.WriteLine(" 2. Read Data");
                Console.WriteLine(" 3. Write Data");
                Console.WriteLine(" 4. Delete Data");
                Console.WriteLine(" 5. LogOff");
                switch(Console.ReadLine()) {
                    case "1":
                        AddNewData(dbContext);
                        break;
                    case "2":
                        ReadData(dbContext);
                        break;
                    case "3":
                        WriteData(dbContext);
                        break;
                    case "4":
                        DeleteData(dbContext);
                        break;
                    case "5":
                        enterResult = "LogOff";
                        break;
                    default:
                        break;
                }
            }
        }

        private static void DeleteData(EFCoreDemoDbContext dbContext) {
            string enterResult = "";
            while(enterResult != "Back") {
                Console.WriteLine("Choose a data you want to delete:");
                Console.WriteLine(" 1. Delete a contact");
                Console.WriteLine(" 2. Delete a department");
                Console.WriteLine(" 3. Delete a task");
                Console.WriteLine(" 4. Back");
                switch(Console.ReadLine()) {
                    case "1":
                        DeleteContact(dbContext);
                        break;
                    case "2":
                        DeleteDepartment(dbContext);
                        break;
                    case "3":
                        DeleteTask(dbContext);
                        break;
                    case "4":
                        enterResult = "Back";
                        break;
                    default:
                        break;
                }
            }
        }

        private static void DeleteTask(EFCoreDemoDbContext dbContext) {
            Console.WriteLine("Enter a description of the task which you need to delete:");
            string description = Console.ReadLine();
            DemoTask task = dbContext.Tasks.Where(c => c.Description == description).FirstOrDefault();
            if(task != null) {
                dbContext.Tasks.Remove(task);
                dbContext.SaveChanges();
            }
            else {
                Console.WriteLine("Object is not found.");
            }
        }

        private static void DeleteDepartment(EFCoreDemoDbContext dbContext) {
            Console.WriteLine("Enter a title of the department which you need to delete:");
            string title = Console.ReadLine();
            Department department = dbContext.Departments.Where(c => c.Title == title).FirstOrDefault();
            if(department != null) {
                dbContext.Departments.Remove(department);
                dbContext.SaveChanges();
            }
            else {
                Console.WriteLine("Object is not found.");
            }
        }

        private static void DeleteContact(EFCoreDemoDbContext dbContext) {
            Console.WriteLine("Enter a name of the contact which you need to delete:");
            string name = Console.ReadLine();
            Contact contact = dbContext.Contacts.Where(c => c.Name == name).FirstOrDefault();
            if(contact != null) {
                dbContext.Contacts.Remove(contact);
                dbContext.SaveChanges();
            }
            else {
                Console.WriteLine("Object is not found.");
            }
        }

        private static void WriteData(EFCoreDemoDbContext dbContext) {
            string enterResult = "";
            while(enterResult != "Back") {
                Console.WriteLine("Choose a data you want to edit:");
                Console.WriteLine(" 1. Edit contacts");
                Console.WriteLine(" 2. Edit departments");
                Console.WriteLine(" 3. Edit tasks");
                Console.WriteLine(" 4. Back");
                switch(Console.ReadLine()) {
                    case "1":
                        EditContact(dbContext);
                        break;
                    case "2":
                        EditDepartment(dbContext);
                        break;
                    case "3":
                        EditTask(dbContext);
                        break;
                    case "4":
                        enterResult = "Back";
                        break;
                    default:
                        break;
                }
            }
        }

        private static void EditTask(EFCoreDemoDbContext dbContext) {
            Console.WriteLine("Enter a description of the task which you need to edit:");
            string description = Console.ReadLine();
            DemoTask task = dbContext.Tasks.Where(c => c.Description == description).FirstOrDefault();
            if(task != null) {
                Console.WriteLine("Enter a new description:");
                task.Description = Console.ReadLine();
                Console.WriteLine("Enter a new note:");
                task.Note = Console.ReadLine();
                dbContext.SaveChanges();
            }
            else {
                Console.WriteLine("Object is not found.");
            }
        }

        private static void EditDepartment(EFCoreDemoDbContext dbContext) {
            Console.WriteLine("Enter a title of the department which you need to edit:");
            string title = Console.ReadLine();
            Department department = dbContext.Departments.Where(c => c.Title == title).FirstOrDefault();
            if(department != null) {
                Console.WriteLine("Enter a new title:");
                department.Title = Console.ReadLine();
                Console.WriteLine("Enter a new office:");
                department.Office = Console.ReadLine();
                dbContext.SaveChanges();
            }
            else {
                Console.WriteLine("Object is not found.");
            }
        }

        private static void EditContact(EFCoreDemoDbContext dbContext) {
            Console.WriteLine("Enter a name of the contact which you need to edit:");
            string name = Console.ReadLine();
            Contact contact = dbContext.Contacts.Where(c => c.Name == name).FirstOrDefault();
            if(contact != null) {
                Console.WriteLine("Enter a new name:");
                contact.Name = Console.ReadLine();
                Console.WriteLine("Enter a new address:");
                contact.Address = Console.ReadLine();
                dbContext.SaveChanges();
            }
            else {
                Console.WriteLine("Object is not found.");
            }
        }

        private static void AddNewData(EFCoreDemoDbContext dbContext) {
            string enterResult = "";
            while(enterResult != "Back") {
                Console.WriteLine("Please choose what data you want to create:");
                Console.WriteLine(" 1. Create a new Contact");
                Console.WriteLine(" 2. Create a new Department");
                Console.WriteLine(" 3. Create a new Task");
                Console.WriteLine(" 4. Back");
                switch(Console.ReadLine()) {
                    case "1":
                        AddNewContact(dbContext);
                        break;
                    case "2":
                        AddNewDepartment(dbContext);
                        break;
                    case "3":
                        AddNewTask(dbContext);
                        break;
                    case "4":
                        enterResult = "Back";
                        break;
                    default:
                        break;
                }
            }
        }

        private static void AddNewTask(EFCoreDemoDbContext dbContext) {
            Console.WriteLine("Enter a description:");
            DemoTask task = new DemoTask() { Description = Console.ReadLine() };
            Console.WriteLine("Enter a note:");
            task.Note = Console.ReadLine();
            dbContext.Tasks.Add(task);
            dbContext.SaveChanges();
        }

        private static void AddNewDepartment(EFCoreDemoDbContext dbContext) {
            Console.WriteLine("Enter a title:");
            Department department = new Department() { Title = Console.ReadLine() };
            Console.WriteLine("Enter an office:");
            department.Office = Console.ReadLine();
            dbContext.Departments.Add(department);
            dbContext.SaveChanges();
        }

        private static void AddNewContact(EFCoreDemoDbContext dbContext) {
            Console.WriteLine("Enter a name:");
            Contact contact = new Contact() { Name = Console.ReadLine() };
            Console.WriteLine("Enter an address:");
            contact.Address = Console.ReadLine();
            dbContext.Contacts.Add(contact);
            dbContext.SaveChanges();
        }

        private static bool ReadData(EFCoreDemoDbContext dbContext) {
            Console.WriteLine("Please choose what data we need to watch:");
            Console.WriteLine(" 1. Contacts");
            Console.WriteLine(" 2. Departments");
            Console.WriteLine(" 3. Tasks");
            Console.WriteLine(" 4. LogOff");
            switch(Console.ReadLine()) {
                case "1":
                    Console.WriteLine("\nContacts which allow for {0}: ", dbContext.CurrentUser.Name);
                    ViewAllContacts(dbContext);
                    Console.WriteLine("Press any key to return to main menu...");
                    Console.ReadLine();
                    break;
                case "2":
                    Console.WriteLine("\nDepartments which allow for {0}: ", dbContext.CurrentUser.Name);
                    ViewAllDepartments(dbContext);
                    Console.WriteLine("Press any key to return to main menu...");
                    Console.ReadLine();
                    break;
                case "3":
                    Console.WriteLine("\nTasks which allow for {0}: ", dbContext.CurrentUser.Name);
                    ViewAllTasks(dbContext);
                    Console.WriteLine("Press any key to return to main menu...");
                    Console.ReadLine();
                    break;
                case "4":
                    return true;
                default:
                    break;
            }
            return false;
        }
        private static void ViewAllTasks(EFCoreDemoDbContext dbContext) {
            Console.WriteLine("\nTasks:");
            int i = 1;
            IEnumerable<DemoTask> tasks = dbContext.Tasks.Include(t => t.ContactTasks).ThenInclude(ct => ct.Contact).ThenInclude(c => c.Department);
            foreach(DemoTask task in tasks) {
                Console.WriteLine("\n{0}. Description: {1}", i, task.Description);
                Console.WriteLine("\n   Note: {0}", task.Note);
                if(task.ContactTasks.Count > 0) {
                    Console.WriteLine("   Contacts in this task:");
                    foreach(ContactTask contactTask in task.ContactTasks) {
                        if(contactTask.Contact != null) {
                            Console.WriteLine("\tName: {0}", contactTask.Contact.Name);
                        }
                    }
                }
                else {
                    Console.WriteLine("   No contacts");
                }
                i++;
            }
        }
        private static void ViewAllDepartments(EFCoreDemoDbContext dbContext) {
            Console.WriteLine("\nDepartments:");
            int i = 1;
            IEnumerable<Department> departments = dbContext.Departments.Include(d => d.Contacts).ThenInclude(p => p.ContactTasks).ThenInclude(p => p.Task);
            foreach(Department department in departments) {
                Console.WriteLine("\n{0}. Title: {1}", i, department.Title);
                if(department.Contacts.Count > 0) {
                    Console.WriteLine("   Contacts in this department:");
                    foreach(Contact contact in department.Contacts) {
                        Console.WriteLine("\tName: {0}", contact.Name);
                    }
                }
                else {
                    Console.WriteLine("   No contacts");
                }
                i++;
            }
        }
        private static void ViewAllContacts(EFCoreDemoDbContext dbContext) {
            int i = 1;
            IEnumerable<Contact> contacts = dbContext.Contacts.Include(c => c.ContactTasks).ThenInclude(ct => ct.Task).Include(c => c.Department);
            foreach(Contact contact in contacts) {
                if(i == 1) {
                    Console.WriteLine("Contacts:");
                }
                foreach(string blockedMember in contact.BlockedMembers) {
                    if(!(blockedMember == "Department" || blockedMember == "ContactTasks")) {
                        System.Reflection.PropertyInfo propertyInfo = contact.GetType().GetProperty(blockedMember);
                        propertyInfo.SetValue(contact, "Protected Content");
                    }
                }
                Console.WriteLine("\n{0}. Name: {1}\nAddress: {2}", i, contact.Name, contact.Address);
                if(contact.Department != null) {
                    Console.WriteLine("Department: {0}", contact.Department.Title);
                }
                else {
                    if(contact.BlockedMembers.Contains("Department")) {
                        Console.WriteLine("Department: Protected Content");
                    }
                    else {
                        Console.WriteLine("No department");
                    }
                }
                if(contact.ContactTasks != null) {
                    Console.WriteLine("Tasks:");
                    if(contact.ContactTasks.Count > 0) {
                        foreach(ContactTask contactTask in contact.ContactTasks) {
                            if(contactTask.Task != null) {
                                Console.WriteLine("   Description: {0}", contactTask.Task.Description);
                            }
                        }
                    }
                    else {
                        Console.WriteLine("   No tasks");
                    }
                }
                i++;
            }
        }
    }
}
