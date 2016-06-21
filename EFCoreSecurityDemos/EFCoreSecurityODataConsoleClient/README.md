This example illustrate how to create simple console OData v4 client application with EF Core Security.  

How to create OData v4 Service with EF Core Security can be found in [EF-Core-Security/EFCoreSecurityDemos/EFCoreSecuritySimpleODataService/](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurityDemos/EFCoreSecuritySimpleODataService).

Use tutorial [Create an OData v4 Client App](http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api/odata-v4/create-an-odata-v4-client-app) to create application sample.

Realize basic authentication by username and password:

            Console.WriteLine("Username: ");
            string userName = Console.ReadLine();
            Console.WriteLine("Please enter password: ");
            string password = Console.ReadLine();
            container.Credentials = new NetworkCredential(userName, password);

Request data:

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

Build the [EFCoreSecurity](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurity) solution before compiling this solution.

All necessary external binaries are located in the [EFCoreSecurity/EFCore-bin](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurity/EFCore-bin) folder.

All necessary NuGet packages will be downloaded and installed automatically before compilation.

