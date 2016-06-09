using DevExpress.EntityFramework.SecurityDataStore;
using DevExpress.EntityFramework.SecurityDataStore.Authorization;
using DevExpress.EntityFramework.SecurityDataStore.Security;
using EFCoreSecurityConsoleDemo.DataModel;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EFCoreSecurityConsoleDemo {
    public static class Updater {
        public static void UpdateDatabase() {
            CreateContacts();
            SecuritySetUp();
        }
        private static void SecuritySetUp() {
            using(PermissionProviderContext context = new PermissionProviderContext()) {
                SecurityUser user = new SecurityUser() { Name = "John", Password = "John" };
                SecurityUser admin = new SecurityUser() { Name = "Admin", Password = "Admin" };

                SecurityRole roleForUser = new SecurityRole();
                // "Address" member of contacts "Ezra" will be denied
                roleForUser.AddMemberPermission<EFCoreDemoDbContext, Contact>(SecurityOperation.Read, OperationState.Deny, "Address", (db, obj) => obj.Name == "Ezra");
                // Contact "Kevin" will be denied
                roleForUser.AddObjectPermission<EFCoreDemoDbContext, Contact>(SecurityOperation.Read, OperationState.Deny, (db, obj) => obj.Address == "California");

                admin.AddRole(new SecurityRole());
                user.AddRole(roleForUser);

                context.Add(user);
                context.Add(admin);
                context.SaveChanges();
            }
        }
        private static void CreateContacts() {
            using(EFCoreDemoDbContext context = new EFCoreDemoDbContext(new SecurityUser())) {
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
                context.Contacts.Add(designer);
                context.Contacts.Add(writer);
                context.Contacts.Add(developer); 
                context.SaveChanges();
            }
        }
    }
}
