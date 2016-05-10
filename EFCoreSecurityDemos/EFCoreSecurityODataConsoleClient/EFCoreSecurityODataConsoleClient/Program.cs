using EFCoreSecurityConsoleClient.Default;
using EFCoreSecurityConsoleClient.EFCoreSecurityODataService.Models;
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
            string enterResult = null;
            while(enterResult != "Exit") {
                Console.WriteLine("Choose an action: ");
                Console.WriteLine("1. LogIn");
                Console.WriteLine("2. Exit");
                switch(Console.ReadLine()) {
                    case "1":
                        enterResult = Logon(container);
                        break;
                    case "2":
                        enterResult = "Exit";
                        break;
                    default:
                        break;
                }
            }
        }
        private static void ListAllTasks(Container container) {
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
        private static void ListAllDepartments(Container container) {
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

        private static void ListAllContacts(Container container) {
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

        private static string Logon(Container container) {
            Console.WriteLine("Choose an action: ");
            Console.WriteLine("Username: ");
            string username = Console.ReadLine();
            Console.WriteLine("Please enter password: ");
            string password = Console.ReadLine();
            container.Credentials = new NetworkCredential(username, password);
            string enterResult = "";
            while(enterResult != "LogOff") {
                try {
                    enterResult = ViewData(container, enterResult, username);
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
            return enterResult;
        }

        private static string ViewData(Container container, string enterResult, string username) {
            Console.WriteLine("Please choose what data we need to watch:");
            Console.WriteLine(" 1. Contacts");
            Console.WriteLine(" 2. Departments");
            Console.WriteLine(" 3. Tasks");
            Console.WriteLine(" 4. LogOff");
            switch(Console.ReadLine()) {
                case "1":
                    Console.WriteLine("\nContacts which allow for {0}: ", username);
                    ListAllContacts(container);
                    Console.WriteLine("Press any key to return to main menu...");
                    Console.ReadLine();
                    break;
                case "2":
                    Console.WriteLine("\nDepartments which allow for {0}: ", username);
                    ListAllDepartments(container);
                    Console.WriteLine("Press any key to return to main menu...");
                    Console.ReadLine();
                    break;
                case "3":
                    Console.WriteLine("\nTasks which allow for {0}: ", username);
                    ListAllTasks(container);
                    Console.WriteLine("Press any key to return to main menu...");
                    Console.ReadLine();
                    break;
                case "4":
                    enterResult = "LogOff";
                    break;
                default:
                    break;
            }

            return enterResult;
        }
    }
}
