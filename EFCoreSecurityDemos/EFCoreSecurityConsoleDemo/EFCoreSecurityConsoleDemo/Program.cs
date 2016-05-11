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
            bool isGettingOut = false;
            while(!isGettingOut) {
                Console.WriteLine("Choose an action: ");
                Console.WriteLine("1. LogIn");
                Console.WriteLine("2. Exit");
                switch(Console.ReadLine()) {
                    case "1":
                        Logon();
                        break;
                    case "2":
                        isGettingOut = true;
                        break;
                    default:
                        break;
                }
            }
        }
        private static void ListAllTasks(EFCoreDemoDbContext dbContext) {
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
        private static void ListAllDepartments(EFCoreDemoDbContext dbContext) {
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

        private static void ListAllContacts(EFCoreDemoDbContext dbContext) {
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

        private static void Logon() {
            using(EFCoreDemoDbContext dbContext = new EFCoreDemoDbContext()) {
                bool isLoggedOff = false;
                Console.WriteLine("Choose an action: ");
                Console.WriteLine("Username: ");
                string username = Console.ReadLine();
                Console.WriteLine("Please enter password: ");
                string password = Console.ReadLine();
                while(!isLoggedOff) {
                    try {
                        dbContext.Logon(username, password);
                        isLoggedOff = ViewData(dbContext);
                    }
                    catch(Exception exception) {
                        Console.WriteLine(exception.Message);
                        isLoggedOff = true;
                    }
                }
            }
        }

        private static bool ViewData(EFCoreDemoDbContext dbContext) {
            Console.WriteLine("Please choose what data we need to watch:");
            Console.WriteLine(" 1. Contacts");
            Console.WriteLine(" 2. Departments");
            Console.WriteLine(" 3. Tasks");
            Console.WriteLine(" 4. LogOff");
            switch(Console.ReadLine()) {
                case "1":
                    Console.WriteLine("\nContacts which allow for {0}: ", dbContext.CurrentUser.Name);
                    ListAllContacts(dbContext);
                    Console.WriteLine("Press any key to return to main menu...");
                    Console.ReadLine();
                    break;
                case "2":
                    Console.WriteLine("\nDepartments which allow for {0}: ", dbContext.CurrentUser.Name);
                    ListAllDepartments(dbContext);
                    Console.WriteLine("Press any key to return to main menu...");
                    Console.ReadLine();
                    break;
                case "3":
                    Console.WriteLine("\nTasks which allow for {0}: ", dbContext.CurrentUser.Name);
                    ListAllTasks(dbContext);
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
    }
}
