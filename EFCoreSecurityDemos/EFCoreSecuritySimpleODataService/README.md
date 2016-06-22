In this tutorial we explain how to create an OData v4 Service with EF Core Security.

- Use tutorial [Create an OData v4 Endpoint Using ASP.NET Web API 2.2](http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api/odata-v4/create-an-odata-v4-endpoint) to create application sample

- Configure the OData Endpoint.
Open the file App_Start/WebApiConfig.cs. Then add the following code to the Register method: 

        private static IEdmModel GetEdmModel() {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Contact>("Contacts");
            foreach(var type in builder.StructuralTypes) {
                if(typeof(ISecurityEntity).IsAssignableFrom(type.ClrType)) {
                    type.AddCollectionProperty(typeof(ISecurityEntity).GetProperty("BlockedMembers"));
                }
            }
            IEdmModel edmModel = builder.GetEdmModel();
            return edmModel;
        }

- Securing OData Services using Basic Authentication with Custom Credentials

Based on [Using Basic Authentication with Custom Credentials](https://msdn.microsoft.com/en-us/data/gg192997.aspx) tutorial.

Change a code of the TryAuthenticate method in the custom authentication provider on the following code:

    private static bool TryAuthenticate(string userName, string password, out IPrincipal principal) {
        using(PermissionsProviderContext dbContext = new PermissionsProviderContext()) {
            if(dbContext.Users.Any(p => p.Name == userName && p.Password == password)) {
                principal = new GenericPrincipal(new GenericIdentity(userName), new string[] { "Users" });
                return true;
            }
            else {
                principal = null;
                return false;
            }
        }
    }

- Initialize EFCoreDemoDbContext instance with permissions provider in the controller. When a request come in the controller, before approaching to an appropriate method, there must be the user authentication in the context of the current controller. For this, we will be use the GetPermissionsProvider static method of the PermissionsProviderContext class:

        private EFCoreDemoDbContext Context = new EFCoreDemoDbContext(PermissionsProviderContext.GetPermissionsProvider());

- Add some security rules in security roles when happens the data initialization:

        // "Address" member of contacts "Ezra" will be denied
        roleForUser.AddMemberPermission<EFCoreDemoDbContext, Contact>(SecurityOperation.Read, OperationState.Deny, "Address", 
                (db, obj) => obj.Name == "Ezra");
        // Contact "Kevin" will be denied
        roleForUser.AddObjectPermission<EFCoreDemoDbContext, Contact>(SecurityOperation.Read, OperationState.Deny, 
                (db, obj) => obj.Address == "California");
 
Build the [EFCoreSecurity](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurity) solution before compiling this solution.

All necessary external binaries are located in the [EFCoreSecurity/EFCore-bin](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurity/EFCore-bin) folder.

All necessary NuGet packages will be downloaded and installed automatically before compilation.
