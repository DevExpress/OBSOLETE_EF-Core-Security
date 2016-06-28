In this example, it is demonstrated how to create an OData v4 Service with EF Core Security.

- Follow the [Create an OData v4 Endpoint Using ASP.NET Web API 2.2](http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api/odata-v4/create-an-odata-v4-endpoint) tutorial to create the basic OData service.



- To configure the OData endpoint, open the *App_Start/WebApiConfig.cs* file and add the following code to the **Register** method: 
```csharp
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
```
- Secure the OData service using [Basic Authentication with Custom Credentials](https://msdn.microsoft.com/en-us/data/gg192997.aspx). In the custom authentication provider, change the **TryAuthenticate** method implementation as follows:
```csharp
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
```
- Initialize the **EFCoreDemoDbContext** instance and pass the permissions provider to the constructor. To get the permissions provider instance, use the **PermissionsProviderContext.GetPermissionsProvider** static method. A user should be authenticated before a request is passed to the controller.
```csharp
        private EFCoreDemoDbContext Context = new EFCoreDemoDbContext(PermissionsProviderContext.GetPermissionsProvider());
```
- Add several security rules to security roles the initial data is created:
```csharp
        // Access to the "Address" member of  the "Ezra" contact is denied:
        roleForUser.AddMemberPermission<EFCoreDemoDbContext, Contact>(SecurityOperation.Read, OperationState.Deny, "Address", 
                (db, obj) => obj.Name == "Ezra");
        // Access to contacts from California is denied:
        roleForUser.AddObjectPermission<EFCoreDemoDbContext, Contact>(SecurityOperation.Read, OperationState.Deny, 
                (db, obj) => obj.Address == "California");
 ```
Build the [EFCoreSecurity](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurity) solution before compiling this example.

All necessary external binaries are located in the [EFCoreSecurity/EFCore-bin](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurity/EFCore-bin) folder.

All necessary NuGet packages will be downloaded and installed automatically before compilation.
