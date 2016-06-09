using System;
using System.Collections.Generic;
using System.Linq;
using EFCoreSecurityConsoleDemo.DataModel;
using DevExpress.EntityFramework.SecurityDataStore.Security;
using Microsoft.EntityFrameworkCore;

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
                        ViewAllContacts(GetPermissionProvider());
                        Console.ReadLine();
                        break;
                    case "2":
                        enterResult = "Exit";
                        break;
                    default:
                        break;
                }
            }
        }
        private static IPermissionsProvider GetPermissionProvider() {
            using(PermissionProviderContext context = new PermissionProviderContext()) {
                Console.WriteLine("Username: ");
                string userName = Console.ReadLine();
                Console.WriteLine("Please enter password: ");
                string password = Console.ReadLine();
                IPermissionsProvider permissionProvider = context.GetUserByCredentials(userName, password);
                if(permissionProvider == null) {
                    throw new Exception("Incorrect user name or password. Please try again with the right credentials.");
                }
                else {
                    return permissionProvider;
                }
            }
        }
        private static void ViewAllContacts(IPermissionsProvider permissionsProvider) {
            using(EFCoreDemoDbContext dbContext = new EFCoreDemoDbContext(permissionsProvider)) {
                int i = 1;
                IEnumerable<Contact> contacts = dbContext.Contacts;
                foreach(Contact contact in contacts) {
                    if(i == 1) {
                        Console.WriteLine("Contacts:");
                    }
                    //foreach(string blockedMember in contact.BlockedMembers) {
                    //    if(!(blockedMember == "Department" || blockedMember == "ContactTasks")) {
                    //        System.Reflection.PropertyInfo propertyInfo = contact.GetType().GetProperty(blockedMember);
                    //        propertyInfo.SetValue(contact, "Protected Content");
                    //    }
                    //}
                    Console.WriteLine("\n{0}. Name: {1}\nAddress: {2}", i, contact.Name, contact.Address);
                    i++;
                } 
            }
        }
    }
}
