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
        static void ListAllEntities(Container container) {
            ListAllContacts(container);
            ListAllDepartments(container);
            ListAllTasks(container);
        }
        private static void ListAllTasks(Container container) {
            Console.WriteLine("\nTasks:");
            int i = 1;
            foreach(DemoTask task in container.Tasks) {
                Console.WriteLine("\n{0}. Description: {1}", i, task.Description);
                Console.WriteLine("\n   Note: {0}", task.Note);
                //if(task.ContactTasks.Count > 0) {
                //    Console.WriteLine("   Contacts in this task:");
                //    foreach(ContactTask contactTask in task.ContactTasks) {
                //        Console.WriteLine("\tName: {0}", contactTask.Contact.Name);
                //    }
                //}
                //else {
                //    Console.WriteLine("   No contacts");
                //}
                i++;
            }
        }
        private static void ListAllDepartments(Container container) {
            Console.WriteLine("\nDepartments:");
            int i = 1;
            foreach(Department department in container.Departments) {
                Console.WriteLine("\n{0}. Title: {1}", i, department.Title);
                //if(department.Contacts.Count > 0) {
                //    Console.WriteLine("   Contacts in this department:");
                //    foreach(Contact contact in department.Contacts) {
                //        Console.WriteLine("\tName: {0}", contact.Name);
                //    }
                //}
                //else {
                //    Console.WriteLine("   No contacts");
                //}
                i++;
            }
        }

        private static void ListAllContacts(Container container) {
            int i = 1;
            try {
                foreach(Contact contact in container.Contacts) {
                    if(i == 1) {
                        Console.WriteLine("Contacts:"); 
                    }
                    foreach(string blockedMember in contact.BlockedMembers) {
                        System.Reflection.PropertyInfo propertyInfo = contact.GetType().GetProperty(blockedMember);
                        propertyInfo.SetValue(contact, "Protected Content");
                    }
                    Console.WriteLine("\n{0}. Name: {1}\nAddress: {2}", i, contact.Name, contact.Address);
                    //Console.WriteLine("Tasks:");
                    //if(contact.ContactTasks.Count > 0) {
                    //    foreach(ContactTask contactTask in contact.ContactTasks) {
                    //        Console.WriteLine("   Description: {0}", contactTask.Task.Description);
                    //    }
                    //}
                    //else {
                    //    Console.WriteLine("   No tasks.");
                    //}
                    i++;
                }
            }
            catch(DataServiceQueryException exception) {
                if(exception.Response.StatusCode == 401 && exception.InnerException.Message.Contains("401.3 - Logon failed")) {
                    Console.WriteLine("Logon failed.");
                }
                if(exception.Response.StatusCode == 401 && exception.InnerException.Message.Contains("401.1 - Please provide Authorization headers with your request")) {
                    Console.WriteLine("Please provide Authorization headers with your request");
                }
                return;
            }
        }
        static void Main(string[] args) {
            Uri serviceUri = new Uri("http://localhost:54342/");
            Container container = new Container(serviceUri);
            
            // Logon for Admin
            NetworkCredential adminCreds = new NetworkCredential("Admin", "Admin");
            container.Credentials = adminCreds;
            Console.WriteLine("\nContacts which allow for admin: ");
            ListAllContacts(container);

            // Logon for User
            container.MergeOption = MergeOption.OverwriteChanges;
            NetworkCredential userCreds = new NetworkCredential("John", "John");
            container.Credentials = userCreds;
            Console.WriteLine("\nContacts which allow for user: ");
            ListAllContacts(container);

            Console.ReadLine();
        }
    }
}
