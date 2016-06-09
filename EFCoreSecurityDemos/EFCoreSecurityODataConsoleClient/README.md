This example illustrate how to create simple console application with EF Core Security.  

You can find the full demo project here:

[EF-Core-Security/EFCoreSecurityDemos/EFCoreSecurityConsoleDemo](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurityDemos/EFCoreSecurityConsoleDemo)

## Prerequisites

The following prerequisites are needed to complete this walkthrough: 
* MS Visual Studio 2015 

## Create a new project

Open Visual Studio (this walkthrough uses 2015 but you can use any version from 2013 onwards) 
* File ‣ New ‣ Project... 
* From the left menu select Templates ‣ Visual C# ‣ Windows 
* Select the Console Application project template 
* Ensure you are targeting .NET Framework 4.5.1 or later 
* Give the project a name and click OK 

## Install EF-Core-Security
 
First, you need to clone the repository [EF-Core-Security](https://github.com/DevExpress/EF-Core-Security) . For this you can use any git client. After you have got git repository you need to add necessary references in your project. All necessary references located at [EFCore-bin](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurity/EFCore-bin) . In the near future, it can be done through Nuget package manager. 
After how you get the EF Core Security latest builds, you will need to build them.

Note:   
Please note that currently available EF providers only from our repository. In the future, you can use any EF provider for data access.

## Create your model
 
Now it`s time to define a context and entity classes that make up your model. 
* Create DataModel folder 
* Add context class (EFCoreDemoDbContext.cs)
* Add entity classes (Contact.cs, Department.cs, DemoTask.cs, ContactTask.cs)

EFCoreDemoDbContext code:

        public class EFCoreDemoDbContext : SecurityDbContextWithUsers  {
            public DbSet<Contact> Contacts { get; set; }
            public DbSet<Department> Departments { get; set; }
            public DbSet<DemoTask> Tasks { get; set; }
            public DbSet<ContactTask> ContactTasks { get; set; }
            
            protected override void OnSecuredConfiguring(DbContextOptionsBuilder optionsBuilder) {
                base.OnSecuredConfiguring(optionsBuilder);
                optionsBuilder.UseInMemoryDatabase();
            }
            
            protected override void OnModelCreating(ModelBuilder modelBuilder) {
                base.OnModelCreating(modelBuilder);
                modelBuilder.Entity<Contact>().HasOne(p => p.Department).WithMany(p => p.Contacts).
                    HasForeignKey(p => p.DepartmentId);
                modelBuilder.Entity<Contact>().HasMany(p => p.ContactTasks).WithOne(p => p.Contact).
                    HasForeignKey(p => p.ContactId);
                modelBuilder.Entity<DemoTask>().HasMany(p => p.ContactTasks).WithOne(p => p.Task).
                    HasForeignKey(p => p.TaskId);
            }
        }

As an example I show below is just the Contact class definition. 

Contact class code:

        public class Contact : BaseSecurityEntity {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public int DepartmentId { get; set; }
            public Department Department { get; set; }
            public List<ContactTask> ContactTasks { get; set; }
        }

Notice the OnSecuredConfiguring method that is used to specify the provider to use and, optionally, other configuration too. 

## Use your model
 
Now you can use your model to perform a data access. 
* Open Program.cs 
* Replace the contents of the file with the following code 
    
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

Updater is a static class which used to specify the initial data in database.  

        public static void UpdateDatabase() {
            EFCoreDemoDbContext dbContext = new EFCoreDemoDbContext();
            CreateITDepartmentModel(dbContext);
            CreateSalesDepartmentModel(dbContext);
            CreateProductionDepartmentModel(dbContext);
            SecuritySetUp(dbContext);
            dbContext.SaveChanges();
        } 

CreateITDepartmentModel, CreateSalesDepartmentModel, CreateProductionDepartmentModel functions contain a code to create and to add the initial data into the context.  
SecuritySetUp function contains the code to set up security rules. 

        private static void SecuritySetUp(EFCoreDemoDbContext dbContext) {
            SecurityUser user = new SecurityUser() { Name = "John", Password = "John" };
            SecurityRole roleForUser = new SecurityRole();
            SecurityUser admin = new SecurityUser() { Name = "Admin", Password = "Admin" };
            SecurityRole roleForAdmin = new SecurityRole();

            ContactSecuritySetUp(roleForUser);
            DepartmentSecuritySetUp(roleForUser);
            TaskSecuritySetUp(roleForUser);

            UserRole userRole = new UserRole() { Role = roleForUser, User = user };
            UserRole adminRole = new UserRole() { Role = roleForAdmin, User = admin };

            dbContext.Add(userRole);
            dbContext.Add(adminRole);
        }

        private static void TaskSecuritySetUp(SecurityRole roleForUser) {
            
            // "Note" member of task "TopManagement", "Write" and "Draw" will be denied
            roleForUser.AddMemberPermission<EFCoreDemoDbContext, DemoTask>(SecurityOperation.Read, 
                OperationState.Deny, "Note", (db, obj) => obj.PercentCompleted < 50);
            
            // Task "Hardcode" will be denied
            roleForUser.AddObjectPermission<EFCoreDemoDbContext, DemoTask>(SecurityOperation.Read, 
                OperationState.Deny, (db, obj) => obj.ContactTasks.Any(p => p.Contact.Name == "John"));
        }

        private static void DepartmentSecuritySetUp(SecurityRole roleForUser) {
            
            // Department "Sales" will be denied
            roleForUser.AddMemberPermission<EFCoreDemoDbContext, Department>(SecurityOperation.Read, 
                OperationState.Deny, "Office", (db, obj) => obj.Title == "Sales");
            
            roleForUser.AddObjectPermission<EFCoreDemoDbContext, Department>(SecurityOperation.Read, 
                OperationState.Deny, (db, obj) => obj.Contacts.Any(c => c.Name == "Barry"));
        }

        private static void ContactSecuritySetUp(SecurityRole roleForUser) {
            
            // "Address" member of contacts "Jack", "Barry" and "Mike" will be denied
            roleForUser.AddMemberPermission<EFCoreDemoDbContext, Contact>(SecurityOperation.Read, 
                OperationState.Deny, "Address", (db, obj) => obj.Department != null && obj.Department.Office == "Texas");
            
            // Contacts "Zack", "Marina", "Kate" will be denied
            roleForUser.AddObjectPermission<EFCoreDemoDbContext, Contact>(SecurityOperation.Read, 
                OperationState.Deny, (db, obj) => obj.Department != null && obj.Department.Title == "Sales");
            
            // Contact "Ezra" will be denied
            roleForUser.AddObjectPermission<EFCoreDemoDbContext, Contact>(SecurityOperation.Read, 
                OperationState.Deny, (db, obj) => obj.ContactTasks.Any(p => p.Task.Description == "Draw"));
        }

After specifying the initial data should be an user authentication. Because EFCoreDemoDbContext has been inherited from SecurityDbContextWithUsers we can use Logon method of context to authenticate the user by username and password. 

        private static void Logon() {
            using(EFCoreDemoDbContext dbContext = new EFCoreDemoDbContext()) {
                Console.WriteLine("Username: ");
                string userName = Console.ReadLine();
                Console.WriteLine("Please enter password: ");
                string password = Console.ReadLine();
                try {
                    dbContext.Logon(userName, password);
                    ExecuteCRUDOperations(dbContext, userName);
                }
                catch(Exception exception) {
                    Console.WriteLine(exception.Message);
                }
            }
        }

If the user has logged in, then he can access to a data according with his security permissions. ExecuteCRUDOperations method is responsible for the data access and executing CRUD operations on the data. 

        private static void ExecuteCRUDOperations(EFCoreDemoDbContext dbContext, string userName) {
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
    
Further we will see examples of the realization each from CRUD operations for the Contact object:    
    
- Create:

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

        private static void AddNewContact(EFCoreDemoDbContext dbContext) {
            Console.WriteLine("Enter a name:");
            Contact contact = new Contact() { Name = Console.ReadLine() };
            Console.WriteLine("Enter an address:");
            contact.Address = Console.ReadLine();
            dbContext.Contacts.Add(contact);
            dbContext.SaveChanges();
        }

- Read:

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

        private static void ViewAllContacts(EFCoreDemoDbContext dbContext) {
            int i = 1;
            IEnumerable<Contact> contacts = dbContext.Contacts.Include(c => c.ContactTasks)
                .ThenInclude(ct => ct.Task).Include(c => c.Department);
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

Note that all entities in our model are inherited from BaseSecurityEntity and have a special BlockedMembers property which contains a list of blocked members. So we can know which members are blocked and handle a program code to display protected content instead a real values of the blocked members. 
 
- Write:

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

- Delete:

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
 
