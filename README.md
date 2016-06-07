# EF Core Security 
## About 

This project allows to restrict an access to a data for CRUD operations on entity-level, object-level and member-level.

## Getting started 

We built and published an OData service and DevExtreme application that works with this service.

See odata service at [SecurityODataService](http://efcoresecurityodataservicedemo.azurewebsites.net/). This service is protected, use 'Admin' as user name and 'Admin' as password (or 'John'/'John' to login as restricted user) to read data from this service. You can expect it directly or create any application to read and modify its data. See the following links:
- [Metadata](http://efcoresecurityodataservicedemo.azurewebsites.net/$metadata)
- [Contacts](http://efcoresecurityodataservicedemo.azurewebsites.net/Contacts)
- [Departments](http://efcoresecurityodataservicedemo.azurewebsites.net/Departments)
- [Tasks](http://efcoresecurityodataservicedemo.azurewebsites.net/Tasks)
See [Demo ODataService](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurityDemos/EFCoreSecurityODataService) to build it locally.

See DevExtreme application at [EFCoreSecurityODataDevExtremeClient](link). This application is based on the published OData service and allows to read and modify its data: Client-server application includes DevExtreme application on client side and OData service on the server side. See (link) to build it locally.

TODO: See MVC application at [EFCoreSecurityODataMVCClient](link). 

How to build and run [console demo to work with DBContext directly](link).
How to build and run [console demo to work with ODataService](link).
How to build and run [Android demo to work with ODataService](link).
TODO: How to build and run [MVC demo to work with ODataService](link).

---
TODELETE
## Concept

EF Core Security is very simple to integrate in your application. The main functional placed in the BaseSecurityDbContext class except the mechanism of the permissions setup and storage. 
The setup and storage of permissions realized in two descendants of the BaseSecurityDbContext class: SecurityDbContext and SecurityDbContextWithUsers. 
Thus, all you need to do is to inherit your data context either from the SecurityDbContext class or from the SecurityDbContextWithUsers class instead a native EFCore DbContext class, 
and also to override the OnSecuredConfiguring method instead of a native OnConfiguring method of EFCore.

The one object for the setup and storage of permissions is used in the SecurityDbContext class, it is PermissionsContainer. 
This class has special methods to setup permissions, such as SetTypePermission, AddObjectPermission, AddMemberPermission and others, which are an implementation of the IPermissionsContainer interface.

The SecurityDbContexWithUsers allows to create an users and roles, that provides more flexible permissions setup and allows to divide an access to a data for different users. 
To setup permissions you also need to use methods of the IPermissionsContainer interface. Thus, you need to perform following steps for the permissions setup:

1. Create one or some roles

2. Setup permissions for each role

3. Create one or some users

4. Add appropriate roles in appropriate users

5. Add users to the context and save changes

You use the BaseSecurityDbContext class as well as the DbContext class of EFCore, because the BaseSecurityDBContext class is a descendant of the DbContext class. 
Also the BaseSecurityDbContext contain the RealDbContext property to access to the native DbContext in cases when you do not need security.
-----
EF Core Security is an extension for a EF Core DbContext. It is suitable for WinForms/ASP.NET/MVC/Android/DevExtreme/Any JSFramework/Win10/iOS and other application development. 
You need MS Visual Studio to develop, test and debug. If you have already worked with EF6 or EFCore, then it won't be difficult for you to understand system work principles. 

To get started you need to get \EF-Core-Security.

- Latest EF Core binaries are placed into the /EFCoreSecurity/EFCore-bin folder.

- The main functionality of EF Core Security is located into /EFCoreSecurity/DevExpress.EntityFramework.SecurityDataStore/

- The extension for authorization functionality is located into /EFCoreSecurity/DevExpress.EntityFramework.Authorization/ 

- Unit tests are located in /EFCoreSecurity/DevExpress.EntityFramework.SecurityDataStore.Tests/

