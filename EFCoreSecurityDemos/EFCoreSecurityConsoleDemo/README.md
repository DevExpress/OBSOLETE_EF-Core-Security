This solution demonstrates how to work with DbContext directly in a console application:

- Create a context to contain data and inherit it from the SecurityDbContext class. Add constructor which gets permissions provider as parameter

        public EFCoreDemoDbContext(IPermissionsProvider permissionsProvider) {
            PermissionsContainer.AddPermissions(permissionsProvider.GetPermissions());
        }

- Create a context to store and access permissions. We use security classes such as SecurityRole and SecurityUser which contain permissions. Add method GetUserByCredentials to authenticate user by username and password

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
            
- Use the PermissionProviderContext class to get PermissionProvider which contains required permissions. For example, we had used authentication by username and password

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

- To create an instance of the EFCoreDemoDbContext class we call EFCoreDemoDbContext constructor with PermissionProvider as parameter and display the data

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
            
- Add some security rules in security roles:

        // "Address" member of contacts "Ezra" will be denied
        roleForUser.AddMemberPermission<EFCoreDemoDbContext, Contact>(SecurityOperation.Read, OperationState.Deny, "Address", 
            (db, obj) => obj.Name == "Ezra");
        // Contact "Kevin" will be denied
        roleForUser.AddObjectPermission<EFCoreDemoDbContext, Contact>(SecurityOperation.Read, OperationState.Deny, 
            (db, obj) => obj.Address == "California");

Build the [EFCoreSecurity](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurity) solution before compiling this solution.

All necessary external binaries are located in the [EFCoreSecurity/EFCore-bin](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurity/EFCore-bin) folder.

All necessary NuGet packages will be downloaded and installed automatically before compilation.
