This example illustrates how to create а simple console OData v4 client application with EF Core Security. An example on how to create a service for thois client application is available at [EF-Core-Security/EFCoreSecurityDemos/EFCoreSecuritySimpleODataService/](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurityDemos/EFCoreSecuritySimpleODataService).

- Create a basic OData client application using the [Create an OData v4 Client App](http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api/odata-v4/create-an-odata-v4-client-app) tutorial.

- Implement the basic authentication by a username and a password:
```csharp
            Console.WriteLine("Username: ");
            string userName = Console.ReadLine();
            Console.WriteLine("Please enter password: ");
            string password = Console.ReadLine();
            container.Credentials = new NetworkCredential(userName, password);
```
- Request a data:
```csharp
        private static void ViewAllContacts(Container container) {
            int i = 1;
            foreach(Contact contact in container.Contacts) {
                if(i == 1) {
                    Console.WriteLine("Contacts:");
                }
                Console.WriteLine("\n{0}. Name: {1}\nAddress: {2}", i, contact.Name, contact.Address);
                i++;
            }
        }
```
- Build the [EFCoreSecurity](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurity) solution, and then compile the current project.

All necessary external binaries are located in the [EFCoreSecurity/EFCore-bin](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurity/EFCore-bin) folder.

All required NuGet packages are downloaded and installed automatically before compiling.
