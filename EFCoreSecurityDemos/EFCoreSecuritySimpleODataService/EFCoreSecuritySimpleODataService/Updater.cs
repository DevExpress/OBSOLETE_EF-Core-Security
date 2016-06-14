using DevExpress.EntityFramework.SecurityDataStore;
using DevExpress.EntityFramework.SecurityDataStore.Authorization;
using EFCoreSecurityODataService.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EFCoreSecurityODataService {
    public static class Updater {
        public static void UpdateDatabase() {
            CreateContacts();
            SecuritySetUp();
        }

        private static void SecuritySetUp() {
            using(PermissionsProviderContext dbContext = new PermissionsProviderContext()) {
                SecurityUser user = new SecurityUser() { Name = "John", Password = "John" };
                SecurityRole roleForUser = new SecurityRole();
                SecurityUser admin = new SecurityUser() { Name = "Admin", Password = "Admin" };
                SecurityRole roleForAdmin = new SecurityRole();

                // "Address" member of contacts "Ezra" will be denied
                roleForUser.AddMemberPermission<EFCoreDemoDbContext, Contact>(SecurityOperation.Read, OperationState.Deny, "Address", (db, obj) => obj.Name == "Ezra");
                // Contact "Kevin" will be denied
                roleForUser.AddObjectPermission<EFCoreDemoDbContext, Contact>(SecurityOperation.Read, OperationState.Deny, (db, obj) => obj.Address == "California");

                user.AddRole(roleForUser);
                admin.AddRole(roleForAdmin);

                dbContext.Add(user);
                dbContext.Add(admin);
                dbContext.SaveChanges();
            }
        }
        private static void CreateContacts() {
            using(EFCoreDemoDbContext dbContext = new EFCoreDemoDbContext()) {
                Contact developer = new Contact() {
                    Name = "John",
                    Address = "Boston"
                };
                Contact writer = new Contact() {
                    Name = "Kevin",
                    Address = "California",
                };
                Contact designer = new Contact() {
                    Name = "Ezra",
                    Address = "San Francisko",
                };
                dbContext.Contacts.Add(designer);
                dbContext.Contacts.Add(writer);
                dbContext.Contacts.Add(developer);
                dbContext.SaveChanges(); 
            }
        }
    }
}