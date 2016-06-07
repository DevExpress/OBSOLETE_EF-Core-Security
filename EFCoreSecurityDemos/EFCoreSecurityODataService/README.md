In this tutorial we explain how to create an OData v4 Endpoint with EF Core Security Using Web Api 2.2.

## Prerequisites.

The following prerequisites are needed to complete this walkthrough: 
- MS Visual Studio 2015 
- Web Api 2.2 
- OData v4 
- .Net 4.5

## Create a new project.

In Visual Studio, from the File menu, select New > Project. 
Expand Installed > Templates > Visual C# > Web, and select the ASP.NET Web Application template. 

![](http://media-www-asp.azureedge.net/media/4929282/odata01.png)

In the New Project dialog, select the Empty template. Under "Add folders and core references...", click Web API. Click OK.

![](http://media-www-asp.azureedge.net/media/4929288/odata02.png)

## Install the OData Packages. 
 
From the Tools menu, select NuGet Package Manager > Package Manager Console. In the Package Manager Console window, type: 
<Install-Package Microsoft.AspNet.Odata> 
This command installs the latest OData NuGet packages. 
 
## Install EF-Core-Security. 
 
First, you need to clone the repository [EF-Core-Security](https://github.com/DevExpress/EF-Core-Security) . For this you can use any git client. After you have got git repository you need to add necessary references in your project. All necessary references located at [EFCore-bin](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurity/EFCore-bin). In the near future, it can be done through Nuget package manager. After how you get the EF-Core-Security latest builds, you will need to build them.

Note: 
Please note that currently available EF providers only from our repository. In the future, you can use any EF provider for data access.

## Add a Model Classes. 
 
Here we add a context class and entity classes which describe an entity model. By convention, model classes are placed in the Models folder, but you don’t have to follow this convention in your own projects. 
- Add context class (EFCoreDemoDbContext.cs) 
- Add entity classes (Contact.cs, Department.cs, DemoTask.cs, ContactTask.cs)

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

Notice the OnSecuredConfiguring method that is used to specify the provider to use and, optionally, other configuration too.

## Configure the OData Endpoint.

Open the file App_Start/WebApiConfig.cs. Then add the following code to the Register method: 

        public static class WebApiConfig {
            public static void Register(HttpConfiguration config) {
                config.MapODataServiceRoute(
                    routeName: "ODataRoute",
                    routePrefix: null,
                    model: GetEdmModel());
            }
            private static IEdmModel GetEdmModel() {
                ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
                builder.EntitySet<Contact>("Contacts");
                builder.EntitySet<DemoTask>("Tasks");
                builder.EntitySet<ContactTask>("ContactTasks");
                builder.EntitySet<Department>("Departments");

                foreach(var type in builder.StructuralTypes) {
                    if(typeof(ISecurityEntity).IsAssignableFrom(type.ClrType)) {
                        type.AddCollectionProperty(typeof(ISecurityEntity).GetProperty("BlockedMembers"));
                    }
                }

                IEdmModel edmModel = builder.GetEdmModel();
                return edmModel;
            }
        }

This code does two things: 
- Creates an Entity Data Model (EDM). 
- Adds a route.

An EDM is an abstract model of the data. The EDM is used to create the service metadata document. The ODataConventionModelBuilderclass creates an EDM by using default naming conventions. This approach requires the least code. If you want more control over the EDM, you can use the ODataModelBuilder class to create the EDM by adding properties, keys, and navigation properties explicitly. 

A route tells Web API how to route HTTP requests to the endpoint. To create an OData v4 route, call the MapODataServiceRoute extension method. 

If your application has multiple OData endpoints, create a separate route for each. Give each route a unique route name and prefix.

## Add the OData Controller. 
 
A controller is a class that handles HTTP requests. You create a separate controller for each entity set in your OData service. In this tutorial, you will create one controller, for the Contact entity. 

In Solution Explorer, right-click the Controllers folder and select Add > Class. Name the class ContactsController. 

Replace the boilerplate code in ProductsController.cs with the following.

        public class ContactsController : ODataController {
            EFCoreDemoDbContext contactContext = new EFCoreDemoDbContext();
            private bool ContactExists(int key) {
                return contactContext.Contacts.Any(p => p.Id == key);
            }
            protected override void Dispose(bool disposing) {
                contactContext.Dispose();
                base.Dispose(disposing);
            }
        }

The controller uses the EFCoreDemoDbContext class to access the database using EF. Notice that the controller overrides the Dispose method to dispose of the EFCoreDemoDbContext. 

This is the starting point for the controller. Next, we'll add methods for all of the CRUD operations.

## Querying the Entity Set.

Add the following methods to ContactsController.

        [EnableQuery]
        public IQueryable<Contact> Get() {
            IQueryable<Contact> result = contactContext.Contacts
                .Include(c => c.Department)
                .Include(c => c.ContactTasks)
                .ThenInclude(ct => ct.Task);
            return result;
        }
        [EnableQuery]
        public IQueryable<Contact> Get([FromODataUri] int key) {
            IQueryable<Contact> result = contactContext.Contacts
                .Where(p => p.Id == key)
                .Include(p => p.Department)
                .Include(c => c.ContactTasks)
                .ThenInclude(ct => ct.Task);
            return result;
        }

The parameterless version of the Get method returns the entire Products collection. The Get method with a key parameter looks up a product by its key (in this case, the Id property). 

The [EnableQuery] attribute enables clients to modify the query, by using query options such as $filter, $sort, and $page.

## Adding an Entity to the Entity Set.

To enable clients to add a new product to the database, add the following method to ContactController. 

        public async Task<IHttpActionResult> Post(Contact contact) {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            contactContext.Contacts.Add(contact);
            await contactContext.SaveChangesAsync();
            return Created(contact);
        }

## Updating an Entity.

OData supports two different semantics for updating an entity, PATCH and PUT. 
- PATCH performs a partial update. The client specifies just the properties to update. 
- PUT replaces the entire entity. 
The disadvantage of PUT is that the client must send values for all of the properties in the entity, including values that are not changing. 

In any case, here is the code for both PATCH and PUT methods:

        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Contact> contact) {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            var entity = await contactContext.Contacts.FirstOrDefaultAsync(p => p.Id == key);
            if(entity == null) {
                return NotFound();
            }
            contact.Patch(entity);
            try {
                await contactContext.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException) {
                if(!ContactExists(key)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }
            return Updated(contact);
        }
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Contact contact) {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            if(key != contact.Id) {
                return BadRequest();
            }
            contactContext.Entry(contact).State = EntityState.Modified;
            try {
                await contactContext.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException) {
                if(!ContactExists(key)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }
            return Updated(contact);
        }

In the case of PATCH, the controller uses the Delta<T> type to track the changes.

## Deleting an Entity. 
 
To enable clients to delete a product from the database, add the following method to ContactsController.

        public async Task<IHttpActionResult> Delete([FromODataUri] int key) {
            var contact = await contactContext.Contacts.FirstOrDefaultAsync(p => p.Id == key);
            if(contact == null) {
                return NotFound();
            }
            contactContext.Contacts.Remove(contact);
            await contactContext.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }

## Securing OData Services using Basic Authentication with Custom Credentials

In the case where the username and password provided by the client are authenticated on the server side by querying a database you need to implement a custom HTTP module and register it for use by the IIS application.

## Creating a Custom HTTP Module for Basic Authentication

A Custom HTTP Module is used to pull out the username and password from the request headers. You can then perform whatever validation you want need in order to authenticate these credentials. Typically, you will define the class that performs the actual username/password authentication separately from the module which acquires the values, referred to as an authentication provider. We will show the implementation and use of both these classes. 

With your project open, add a new Class (we called ours BasicAuthModule.cs). An HTTP module is defined by implementing the IHttpModule interface which contains two methods: Init and Dispose. Within the Init event, you are going to register a handler for the AuthenticateRequest event. 

        public class BasicAuthModule : IHttpModule {
            public void Init(HttpApplication app) {
                app.AuthenticateRequest += new EventHandler(app_AuthenticateRequest);
            }
            private void app_AuthenticateRequest(object sender, EventArgs args) {
                WebApiApplication app = (WebApiApplication)sender;
                if(!app.Request.Headers.AllKeys.Contains("Authorization")) {
                    CreateNotAuthorizedResponse(app, 401, 1,
                        "Please provide Authorization headers with your request.");
                    app.CompleteRequest();
                }
                else if(!BasicAuthProvider.Authenticate(app)) {
                    CreateNotAuthorizedResponse(app, 401, 3, "Logon failed.");
                    app.CompleteRequest();
                }
            }
            private static void CreateNotAuthorizedResponse(HttpApplication app, int code, int subCode, 
                string description) {
                HttpResponse response = app.Context.Response;
                response.StatusCode = code;
                response.SubStatusCode = subCode;
                response.StatusDescription = description;
                response.AppendHeader("WWW-Authenticate", "Basic");
            }
            public void Dispose() { }
        }

The handler itself is where you define your actual authentication work. The work you need to do amounts ensuring that an Authorization header was provided (again sending an HTTP error response if it is not) and finally performing the actual validation of the username and password using your authentication provider.  

Notice in the above that we craft a specific error response following standard HTTP error codes: 
- A 401.1 error is sent to challenge the client and prompt it provider the required Authorization headers. 
- A 401.3 error is also sent when the username/password is not valid or not allowed to access the service. 

## Building a Custom Authentication Provider

In the previous code sample, we leveraged a custom authentication provider called BasicAuthProvider, which takes as input the WebApiApplication so that it has access to the headers. Also WebApiApplication implements the "ISecurityApplication" interface which allows to store the user data to provide an access to user in the controllers. From a high level, the provider needs to extract the Authorization header, decode the value and split it into username and password strings and then actually authenticate the user.

To create an authentication provider such as this, we simple add a new class to the project and implement methods to support authentication. For our sample, we expose a single public static method Authenticate for use by the HTTP module.

The first step is to acquire the value of the Authorization header, and then we use that to authenticate. Observe that when authentication succeeds, we set the CurrentUser of the WebApiApplication. So we will be able to gain access to the user in our controllers:

        public static bool Authenticate(WebApiApplication app) {
            HttpContext context = app.Context;
            string authHeader = context.Request.Headers["Authorization"];
            ISecurityUser user;
            if(TryGetPrincipal(authHeader, out user)) {
                app.CurrentUser = user;
                return true;
            }
            return false;
        }

Next, we need to attempt to parse that header and authenticate the user. We wrap an authenticated user in an ISecurityUser.

        private static bool TryGetPrincipal(string authHeader, out ISecurityUser user) {
            string userName;
            string password;
            if(TryParseAuthorizationHeader(authHeader, out userName, out password)) {
                return TryAuthenticate(userName, password, out user);
            }
            user = null;
            return false;
        }

To actually parse the authorization header string, we need to get at the Base64 encoded portion of it (remember the value looks like Basic+YWRtaW46ZnNkYQ), decode it and then split it into a username string and password string. These three steps are highlighted below:

        private static bool TryParseAuthorizationHeader(string authHeader, out string user, out string password) {
            user = "";
            password = "";
            if(string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic")) {
                return false;
            }
            string base64EncodedCreds = authHeader.Substring(6);
            string[] creds = Encoding.ASCII.GetString(Convert.FromBase64String(base64EncodedCreds)).Split(new char[] 
                { ':' });
            if(creds.Length != 2 || string.IsNullOrEmpty(creds[0]) || string.IsNullOrEmpty(creds[1])) {
                return false;
            }
            user = creds[0];
            password = creds[1];
            return true;
        }

With our username and password in hand, we can perform whatever validation required to properly authenticate. EFCoreDemoDbContext has the GetUserByCredentials method which allow to get ISecurityUser object. If you happen to find a user with the same name and password, the function indicates that the authentication is successful and return the user. If the user was not found, the function returns false, causing the authentication error.

        private static bool TryAuthenticate(string userName, string password, out ISecurityUser securityUser) {
            EFCoreDemoDbContext dbContext = new EFCoreDemoDbContext();
            ISecurityUser user = dbContext.GetUserByCredentials(userName, password);
            if(user != null) {
                securityUser = user;
                return true;
            }
            else {
                securityUser = null;
                return false;
            }
        }

## Registering the Module

We still have one more step left before being able to leverage the custom HTTP module- we need to add an entry for it in web.config that tells the web server to use it in the request processing pipeline that leads up to our OData Service.  This is a matter of adding the following line to the system.webServer\modules node:

        public static class WebApiConfig
        {
            public static void Register(HttpConfiguration config)
            {
                config.MapODataServiceRoute(
                    routeName: "ODataRoute",
                    routePrefix: null,
                    model: GetEdmModel());
            }
            private static IEdmModel GetEdmModel() {
                ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
                builder.EntitySet<Contact>("Contacts");
                builder.EntitySet<DemoTask>("Tasks");
                builder.EntitySet<ContactTask>("ContactTasks");
                builder.EntitySet<Department>("Departments");
 
                foreach(var type in builder.StructuralTypes) {
                    if(typeof(ISecurityEntity).IsAssignableFrom(type.ClrType)) {
                        type.AddCollectionProperty(typeof(ISecurityEntity).GetProperty("BlockedMembers"));
                    }
                }

                IEdmModel edmModel = builder.GetEdmModel();
                return edmModel;
            }
        }

## Add a checking of the user in the controllers

When a request come in the controller, before approaching to an appropriate method, there must be the user authentication in the context of the current controller. For this, we will be use the user instance from application.CurrentUser, was initialized earlier, and the Logon method of the context, which gets the ISecurityUser user as parameter. 

Add the following code in the controller constructor:

        public ContactsController() {
            ISecurityApplication application = HttpContext.Current.ApplicationInstance as ISecurityApplication;
            if(application != null) {
                ISecurityUser user = application.CurrentUser;
                if(user != null) {
                    contactContext.Logon(user);
                }
            }
        }

## Setting the initial data of data model and security rules

To set the initial data we call the UpdateDatabase method of the Updater static class, where happens the data initialization. 

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            Updater.UpdateDatabase();
        }

        public static class Updater {
            public static void UpdateDatabase() {
                EFCoreDemoDbContext dbContext = new EFCoreDemoDbContext();
                CreateITDepartmentModel(dbContext);
                CreateSalesDepartmentModel(dbContext);
                CreateProductionDepartmentModel(dbContext);
                SecuritySetUp(dbContext);
                dbContext.SaveChanges();
           }

The methods, such as the CreateITDepartmentModel method, set the initial data of the data model for the each department.

        private static void CreateITDepartmentModel(EFCoreDemoDbContext dbContext) {
            Department itDepartment = new Department() {
                Title = "IT",
                Office = "SiliconValley"
            };
            DemoTask itTask = new DemoTask() {
                Description = "HardCode",
                Note = "This must be perfect code",
                StartDate = new DateTime(2016, 01, 23),
                DateCompleted = new DateTime(2016, 06, 13),
                PercentCompleted = 52
            };
            DemoTask writeTask = new DemoTask() {
                Description = "Write",
                Note = "Write docs",
                StartDate = new DateTime(2015, 09, 14),
                DateCompleted = new DateTime(2018, 07, 18),
                PercentCompleted = 25
            };
            DemoTask designTask = new DemoTask() {
                Description = "Draw",
                Note = "Draw pictures like Picasso",
                StartDate = new DateTime(2016, 04, 03),
                DateCompleted = new DateTime(2020, 11, 04),
                PercentCompleted = 3
            };
            Contact developer = new Contact() {
                Name = "John",
                Address = "Boston"
            };
            Contact writer = new Contact() {
                Name = "Kevin",
                Address = "California",
                Department = itDepartment
            };
            Contact designer = new Contact() {
                Name = "Ezra",
                Address = "San Francisko",
                Department = itDepartment
            };
            ContactTask itContactTask = new ContactTask() {
                Contact = developer,
                Task = itTask
            };
            ContactTask writeContactTask = new ContactTask() {
                Contact = writer,
                Task = writeTask
            };
            ContactTask designContactTask = new ContactTask() {
                Contact = designer,
                Task = designTask
            };
            dbContext.ContactTasks.Add(itContactTask);
            dbContext.ContactTasks.Add(designContactTask);
            dbContext.ContactTasks.Add(writeContactTask);
            dbContext.Tasks.Add(itTask);
            dbContext.Tasks.Add(writeTask);
            dbContext.Tasks.Add(designTask);
            dbContext.Contacts.Add(designer);
            dbContext.Contacts.Add(writer);
            dbContext.Contacts.Add(developer);
            dbContext.Departments.Add(itDepartment);
        }

The SecuritySetUp method sets the initial data of the security system. The method creates two users(administrator and simple user) with two roles, and add some security rules inside the methods, such as ContactSecuritySetUp, for each entity type.

        private static void ContactSecuritySetUp(SecurityRole roleForUser) {
            roleForUser.AddMemberPermission<EFCoreDemoDbContext, Contact>(SecurityOperation.Read, 
                OperationState.Deny, "Address", (db, obj) => obj.Department != null && obj.Department.Office == "Texas");

            roleForUser.AddObjectPermission<EFCoreDemoDbContext, Contact>(SecurityOperation.Read, 
                OperationState.Deny, (db, obj) => obj.Department != null && obj.Department.Title == "Sales");

            roleForUser.AddObjectPermission<EFCoreDemoDbContext, Contact>(SecurityOperation.Read, 
                OperationState.Deny, (db, obj) => obj.ContactTasks.Any(p => p.Task.Description == "Draw"));
        }
