using EFCoreSecurityConsoleClient.Default;
using EFCoreSecurityConsoleClient.EFCoreSecurityODataService.Models;
using Microsoft.OData.Client;
using System;
using System.Collections.Generic;
using System.Linq;
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
                if(task.ContactTasks.Count > 0) {
                    Console.WriteLine("   Contacts in this task:");
                    foreach(ContactTask contactTask in task.ContactTasks) {
                        Console.WriteLine("\tName: {0}", contactTask.Contact.Name);
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
            Console.WriteLine("Contacts:");
            int i = 1;
            foreach(Contact contact in container.Contacts) {
                Console.WriteLine("\n{0}. Name: {1}\nAddress: {2}\nDepartment: {3}", i, contact.Name, contact.Address, contact.Department != null ? contact.Department.Title : "Without department");
                Console.WriteLine("Tasks:");
                if(contact.ContactTasks.Count > 0) {
                    foreach(ContactTask contactTask in contact.ContactTasks) {
                        Console.WriteLine("   Description: {0}", contactTask.Task.Description);
                    }
                }
                else {
                    Console.WriteLine("Tasks: No tasks.");
                }
                i++;
            }
        }
        static void Main(string[] args) {
            string serviceUri = "http://localhost:54342/";
            Container container = new Container(new Uri(serviceUri));
            ListAllEntities(container);
            Console.ReadLine();
        }
    }
}
