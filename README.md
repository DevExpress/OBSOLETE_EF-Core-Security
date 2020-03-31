# OBSOLETE - EF Core Security (Maintained by [DevExpress](http://devexpress.com/))

### We'd like to announce that we've published an example demonstrating how to access data protected by the XAF [Security System](https://docs.devexpress.com/eXpressAppFramework/113366/concepts/security-system) from a Console application with Entity Framework Core: [EFCore/Console](https://github.com/DevExpress-Examples/XAF_how-to-use-the-integrated-mode-of-the-security-system-in-non-xaf-applications-e4908/blob/20.1/EFCore/Console/Readme.md?utm_source=DevExpress&utm_medium=Website&utm_campaign=XAF&utm_content=XAF_Security_NonXAF_Console_EFCore)




===============================================================



**NOTE**: We stopped maintaining this example starting from the EF Core 1.1.0 version. We do not carry out its active development in order to guarantee that this example works with the latest versions of EF Core. 
If you are interested in our support of the XAF Security module for EF Core, we would greatly appreciate it if you described your requirements and the exact use-scenarios where this support is needed.

## About 

This project allows you to secure a CRUD access to data in applications based on [Entity Framework Core (EF Core)](https://github.com/aspnet/EntityFramework/wiki). With **EF Core Security** you can grant and deny *entity-level*, *object-level* and *member-level* permissions for authenticated users.

## Installing EF Core Security

In Visual Studio, you can use the [GitHub Extension for VisualStudio](https://visualstudio.github.com/) to clone this repository.

Most of required external assemblies are loaded from NuGet (Microsoft.EntityFrameworkCore and others).
Other external assemblies are located in the [EFCoreSecurity/dependencies](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurity/dependencies) folder.

## Getting Started 

The following demos and examples for the following use-cases are available in this repositiory.

#### OData Service
The OData service demo souce is available at [Demo ODataService](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurityDemos/EFCoreSecurityODataService).

#### DevExtreme Application
See the [EFCoreSecurityDemos/EFCoreSecurityDevExtremeDemo](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurityDemos/EFCoreSecurityDevExtremeDemo) example to learn how build it locally.

#### .NET OData Client
The [.NET C# Console demo](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurityDemos/EFCoreSecurityODataConsoleClient) example demonstrates how to access the OData service from C# code.

#### Android OData Client
The [Android demo](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurityDemos/EFCoreSecurityODataAndroidClient) example demonstrates how to access the OData service from the Android application.

#### Direct Access to the Entity Framework DBContext 
The [.NET C# Console demo](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurityDemos/EFCoreSecurityConsoleDemo) example demonstrates how to use the EF DBContext directly.

## Wiki
Learn more about this project in [our wiki](https://github.com/DevExpress/EF-Core-Security/wiki).
