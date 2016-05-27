using EFCoreSecurityConsoleClient.Default;
using EFCoreSecurityConsoleClient.EFCoreSecurityODataService.DataModel;
using Microsoft.OData.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreSecurityConsoleClient {
    class Program {
        static void Main(string[] args) {
            Uri serviceUri = new Uri("http://localhost:54342/");
            Container container = new Container(serviceUri);
            container.MergeOption = MergeOption.OverwriteChanges;
            string enterResult = "";
            while(enterResult != "Exit") {
                Console.WriteLine("Choose an action: ");
                Console.WriteLine("1. LogIn");
                Console.WriteLine("2. Exit");
                switch(Console.ReadLine()) {
                    case "1":
                        Logon(container);
                        break;
                    case "2":
                        enterResult = "Exit";
                        break;
                    default:
                        break;
                }
            }
        }
        private static void Logon(Container container) {
            Console.WriteLine("Username: ");
            string userName = Console.ReadLine();
            Console.WriteLine("Please enter password: ");
            string password = Console.ReadLine();
            container.Credentials = new NetworkCredential(userName, password);
            try {
                ExecuteCRUDOperations(container, userName);
            }
            catch(DataServiceQueryException exception) {
                if(exception.Response.StatusCode == 401 && exception.InnerException.Message.Contains("401.3 - Logon failed")) {
                    Console.WriteLine("Logon failed.");
                }
                if(exception.Response.StatusCode == 401 && exception.InnerException.Message.Contains("401.1 - Please provide Authorization headers with your request")) {
                    Console.WriteLine("Please provide Authorization headers with your request");
                }
            }
        }
        private static void ExecuteCRUDOperations(Container container, string userName) {
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
                        AddNewData(container);
                        break;
                    case "2":
                        ReadData(container, userName);
                        break;
                    case "3":
                        WriteData(container);
                        break;
                    case "4":
                        DeleteData(container);
                        break;
                    case "5":
                        enterResult = "LogOff";
                        break;
                    default:
                        break;
                }
            }
        }

        private static void DeleteData(Container container) {
            string enterResult = "";
            while(enterResult != "Back") {
                Console.WriteLine("Choose a data you want to delete:");
                Console.WriteLine(" 1. Delete a contact");
                Console.WriteLine(" 2. Delete a department");
                Console.WriteLine(" 3. Delete a task");
                Console.WriteLine(" 4. Back");
                switch(Console.ReadLine()) {
                    case "1":
                        DeleteContact(container);
                        break;
                    case "2":
                        DeleteDepartment(container);
                        break;
                    case "3":
                        DeleteTask(container);
                        break;
                    case "4":
                        enterResult = "Back";
                        break;
                    default:
                        break;
                }
            }
        }
        private static void DeleteTask(Container container) {
            Console.WriteLine("Enter a description of the task which you need to delete:");
            string description = Console.ReadLine();
            DemoTask task = container.Tasks.Where(c => c.Description == description).FirstOrDefault();
            if(task != null) {
                container.DeleteObject(task);
                var serviceResponse = container.SaveChanges();
                foreach(var operationResponse in serviceResponse) {
                    Console.WriteLine("Response: {0}", operationResponse.StatusCode);
                }
            }
            else {
                Console.WriteLine("Object is not found.");
            }
        }
        private static void DeleteDepartment(Container container) {
            Console.WriteLine("Enter a title of the department which you need to delete:");
            string title = Console.ReadLine();
            Department department = container.Departments.Where(c => c.Title == title).FirstOrDefault();
            if(department != null) {
                container.DeleteObject(department);
                var serviceResponse = container.SaveChanges();
                foreach(var operationResponse in serviceResponse) {
                    Console.WriteLine("Response: {0}", operationResponse.StatusCode);
                }
            }
            else {
                Console.WriteLine("Object is not found.");
            }
        }
        private static void DeleteContact(Container container) {
            Console.WriteLine("Enter a name of the contact which you need to delete:");
            string name = Console.ReadLine();
            Contact contact = container.Contacts.Where(c => c.Name == name).FirstOrDefault();
            if(contact != null) {
                container.DeleteObject(contact);
                var serviceResponse = container.SaveChanges();
                foreach(var operationResponse in serviceResponse) {
                    Console.WriteLine("Response: {0}", operationResponse.StatusCode);
                }
            }
            else {
                Console.WriteLine("Object is not found.");
            }
        }

        private static void WriteData(Container container) {
            string enterResult = "";
            while(enterResult != "Back") {
                Console.WriteLine("Choose a data you want to edit:");
                Console.WriteLine(" 1. Edit contacts");
                Console.WriteLine(" 2. Edit departments");
                Console.WriteLine(" 3. Edit tasks");
                Console.WriteLine(" 4. Back");
                switch(Console.ReadLine()) {
                    case "1":
                        EditContact(container);
                        break;
                    case "2":
                        EditDepartment(container);
                        break;
                    case "3":
                        EditTask(container);
                        break;
                    case "4":
                        enterResult = "Back";
                        break;
                    default:
                        break;
                }
            }
        }
        private static void EditTask(Container container) {
            Console.WriteLine("Enter a description of the task which you need to edit:");
            string description = Console.ReadLine();
            DemoTask task = container.Tasks.Where(c => c.Description == description).FirstOrDefault();
            if(task != null) {
                Console.WriteLine("Enter a new description:");
                task.Description = Console.ReadLine();
                Console.WriteLine("Enter a new note:");
                task.Note = Console.ReadLine();
                container.UpdateObject(task);
                var serviceResponse = container.SaveChanges();
                foreach(var operationResponse in serviceResponse) {
                    Console.WriteLine("Response: {0}", operationResponse.StatusCode);
                }
            }
            else {
                Console.WriteLine("Object is not found.");
            }
        }
        private static void EditDepartment(Container container) {
            Console.WriteLine("Enter a title of the department which you need to edit:");
            string title = Console.ReadLine();
            Department department = container.Departments.Where(c => c.Title == title).FirstOrDefault();
            if(department != null) {
                Console.WriteLine("Enter a new title:");
                department.Title = Console.ReadLine();
                Console.WriteLine("Enter a new office:");
                department.Office = Console.ReadLine();
                container.UpdateObject(department);
                var serviceResponse = container.SaveChanges();
                foreach(var operationResponse in serviceResponse) {
                    Console.WriteLine("Response: {0}", operationResponse.StatusCode);
                }
            }
            else {
                Console.WriteLine("Object is not found.");
            }
        }
        private static void EditContact(Container container) {
            Console.WriteLine("Enter a name of the contact which you need to edit:");
            string name = Console.ReadLine();
            Contact contact = container.Contacts.Where(c => c.Name == name).FirstOrDefault();
            if(contact != null) {
                Console.WriteLine("Enter a new name:");
                contact.Name = Console.ReadLine();
                Console.WriteLine("Enter a new address:");
                contact.Address = Console.ReadLine();
                container.UpdateObject(contact);
                var serviceResponse = container.SaveChanges();
                foreach(var operationResponse in serviceResponse) {
                    Console.WriteLine("Response: {0}", operationResponse.StatusCode);
                }
            }
            else {
                Console.WriteLine("Object is not found.");
            }
        }

        private static void AddNewData(Container container) {
            string enterResult = "";
            while(enterResult != "Back") {
                Console.WriteLine("Please choose what data you want to create:");
                Console.WriteLine(" 1. Create a new Contact");
                Console.WriteLine(" 2. Create a new Department");
                Console.WriteLine(" 3. Create a new Task");
                Console.WriteLine(" 4. Back");
                switch(Console.ReadLine()) {
                    case "1":
                        AddNewContact(container);
                        break;
                    case "2":
                        AddNewDepartment(container);
                        break;
                    case "3":
                        AddNewTask(container);
                        break;
                    case "4":
                        enterResult = "Back";
                        break;
                    default:
                        break;
                }
            }
        }
        private static void AddNewTask(Container container) {
            Console.WriteLine("Enter a description:");
            DemoTask task = new DemoTask() { Description = Console.ReadLine() };
            Console.WriteLine("Enter a note:");
            task.Note = Console.ReadLine();
            container.AddToTasks(task);
            var serviceResponse = container.SaveChanges();
            foreach(var operationResponse in serviceResponse) {
                Console.WriteLine("Response: {0}", operationResponse.StatusCode);
            }
        }
        private static void AddNewDepartment(Container container) {
            Console.WriteLine("Enter a title:");
            Department department = new Department() { Title = Console.ReadLine() };
            Console.WriteLine("Enter an office:");
            department.Office = Console.ReadLine();
            container.AddToDepartments(department);
            var serviceResponse = container.SaveChanges();
            foreach(var operationResponse in serviceResponse) {
                Console.WriteLine("Response: {0}", operationResponse.StatusCode);
            }
        }
        private static void AddNewContact(Container container) {
            Console.WriteLine("Enter a name:");
            Contact contact = new Contact() { Name = Console.ReadLine() };
            Console.WriteLine("Enter an address:");
            contact.Address = Console.ReadLine();
            container.AddToContacts(contact);
            var serviceResponse = container.SaveChanges();
            foreach(var operationResponse in serviceResponse) {
                Console.WriteLine("Response: {0}", operationResponse.StatusCode);
            }
        }

        private static bool ReadData(Container container, string userName) {
            Console.WriteLine("Please choose what data we need to watch:");
            Console.WriteLine(" 1. Contacts");
            Console.WriteLine(" 2. Departments");
            Console.WriteLine(" 3. Tasks");
            Console.WriteLine(" 4. LogOff");
            switch(Console.ReadLine()) {
                case "1":
                    Console.WriteLine("\nContacts which allow for {0}: ", userName);
                    ViewAllContacts(container);
                    Console.WriteLine("Press any key to return to main menu...");
                    Console.ReadLine();
                    break;
                case "2":
                    Console.WriteLine("\nDepartments which allow for {0}: ", userName);
                    ViewAllDepartments(container);
                    Console.WriteLine("Press any key to return to main menu...");
                    Console.ReadLine();
                    break;
                case "3":
                    Console.WriteLine("\nTasks which allow for {0}: ", userName);
                    ViewAllTasks(container);
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
        private static void ViewAllTasks(Container container) {
            Console.WriteLine("\nTasks:");
            int i = 1;
            foreach(DemoTask task in container.Tasks) {
                Console.WriteLine("\n{0}. Description: {1}", i, task.Description);
                Console.WriteLine("\n   Note: {0}", task.Note);
                container.LoadProperty(task, "ContactTasks");
                if(task.ContactTasks.Count > 0) {
                    Console.WriteLine("   Contacts in this task:");
                    foreach(ContactTask contactTask in task.ContactTasks) {
                        container.LoadProperty(contactTask, "Contact");
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
        private static void ViewAllDepartments(Container container) {
            Console.WriteLine("\nDepartments:");
            int i = 1;
            foreach(Department department in container.Departments) {
                Console.WriteLine("\n{0}. Title: {1}", i, department.Title);
                container.LoadProperty(department, "Contacts");
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
        private static void ViewAllContacts(Container container) {
            int i = 1;
            foreach(Contact contact in container.Contacts) {
                container.LoadProperty(contact, "Department");
                container.LoadProperty(contact, "ContactTasks");
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
                            container.LoadProperty(contactTask, "Task");
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
