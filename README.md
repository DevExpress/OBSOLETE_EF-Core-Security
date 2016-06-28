# EF Core Security (Maintained by [DevExpress](http://devexpress.com/))
## About 

This project allows you to secure a CRUD access to data in applications based on [Entity Framework Core (EF Core)](https://github.com/aspnet/EntityFramework/wiki). With **EF Core Security** you can grant and deny *entity-level*, *object-level* and *member-level* permissions for authenticated users.

## Installing EF Core Security

In Visual Studio, you can use the [GitHub Extension for VisualStudio](https://visualstudio.github.com/) to clone this repository.

Most of required external assemblies are loaded from NuGet (Microsoft.EntityFrameworkCore and others).
Other external assemblies are located in the [EFCoreSecurity/dependencies](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurity/dependencies) folder.

## Getting Started 

The following demos and examples for the following use-cases are available in this repositiory.

#### OData Service
The OData service demo is published at [SecurityODataService](http://efcoresecurityodataservicedemo.azurewebsites.net/). The service is protected. To read data from this it, use 'Admin'/'Admin' username and password to login as an administrator, or 'John'/'John' to login as restricted user. You can inspect this demo in details or create your own application to read and modify its data. See the following links:

* [Metadata](http://efcoresecurityodataservicedemo.azurewebsites.net/$metadata)
* [Contacts](http://efcoresecurityodataservicedemo.azurewebsites.net/Contacts)
* [Departments](http://efcoresecurityodataservicedemo.azurewebsites.net/Departments)
* [Tasks](http://efcoresecurityodataservicedemo.azurewebsites.net/Tasks)

The OData service demo souce is available at [Demo ODataService](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurityDemos/EFCoreSecurityODataService).

#### DevExtreme Application
The [DevExtreme](http://js.devexpress.com/) application demo is published at [DevExtremeClient](http://efcoresecuritydevextremedemoweb.azurewebsites.net). At the logon screen, use 'Admin'/'Admin' username and password to login as an administrator, or 'John'/'John' to login as restricted user. This application is based on the published OData service and allows you to read and modify its data. Client-server application includes DevExtreme client application and OData service on the server side. See the [EFCoreSecurityDemos/EFCoreSecurityDevExtremeDemo](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurityDemos/EFCoreSecurityDevExtremeDemo) example to learn how build it locally.

#### .NET OData Client
The [.NET C# Console demo](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurityDemos/EFCoreSecurityODataConsoleClient) example demonstrates how to access the OData service from C# code.

#### Android OData Client
The [Android demo](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurityDemos/EFCoreSecurityODataAndroidClient) example demonstrates how to access the OData service from the Android application.

#### Direct Access to the Entity Framework DBContext 
The [.NET C# Console demo](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurityDemos/EFCoreSecurityConsoleDemo) example demonstrates how to use the EF DBContext directly.

## Wiki
Learn more about this project in [our wiki](https://github.com/DevExpress/EF-Core-Security/wiki).
