This example demonstrates how to use **DbContext** with EF Core Security in a simple console application:

- Create a data context and inherit it from the **SecurityDbContext** class. Add a constructor which gets the permissions provider as a parameter:

        public EFCoreDemoDbContext(IPermissionsProvider permissionsProvider) {
            PermissionsContainer.AddPermissions(permissionsProvider.GetPermissions());
        }

- Create a context to store and access permissions. Here, the **SecurityRole** and **SecurityUser** classes which contain permissions are used. Add the **GetUserByCredentials** method to authenticate users by username and password

        public DbSet<SecurityRole> Roles { get; set; }
        public DbSet<SecurityUser> Users { get; set; }
        public ISecurityUser GetUserByCredentials(string userName, string password) {
            return this.Users.
                Include(p => p.UserRoleCollection).ThenInclude(p => p.Role).ThenInclude(p => p.MemberPermissions).
                Include(p => p.UserRoleCollection).ThenInclude(p => p.Role).ThenInclude(p => p.OperationPermissions).
                Include(p => p.UserRoleCollection).ThenInclude(p => p.Role).ThenInclude(p => p.ObjectPermissions).
                Include(p => p.UserRoleCollection).ThenInclude(p => p.Role).ThenInclude(p => p.TypePermissions).
                FirstOrDefault(p => p.Name == userName && p.Password == password);
        }
            
- Use the **PermissionProviderContext** class to get a **PermissionProvider** containing required permissions. In this example, the authentication by username and password id used.

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

- To create an instance of the **EFCoreDemoDbContext**, pass **PermissionProvider** to its constructor. Then, you can query and display the data.

        using(EFCoreDemoDbContext dbContext = new EFCoreDemoDbContext(permissionsProvider)) {
            int i = 1;
            IEnumerable<Contact> contacts = dbContext.Contacts;
            foreach(Contact contact in contacts) {
                if(i == 1) {
                    Console.WriteLine("Contacts:");
                }
                Console.WriteLine("\n{0}. Name: {1}\nAddress: {2}", i, contact.Name, contact.Address);
                i++;
            } 
        }
            
- Add several security rules to the security role:

        // An acces to the "Address" field of the "Ezra" contact will be denied:
        roleForUser.AddMemberPermission<EFCoreDemoDbContext, Contact>(SecurityOperation.Read, OperationState.Deny, "Address", 
            (db, obj) => obj.Name == "Ezra");
        // An acces to contacts from California will be denied:
        roleForUser.AddObjectPermission<EFCoreDemoDbContext, Contact>(SecurityOperation.Read, OperationState.Deny, 
            (db, obj) => obj.Address == "California");

Build the [EFCoreSecurity](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurity) solution. Then, compile the current example.

All necessary external binaries are located in the [EFCoreSecurity/EFCore-bin](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurity/EFCore-bin) folder.

All necessary NuGet packages are downloaded and installed automatically before compilation.
