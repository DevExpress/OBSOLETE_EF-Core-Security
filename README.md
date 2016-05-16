# EF-Core-Security 
## About 

This project is a security system for Entity Framework Core. As Entity Framework Core is currently in the pre-release stage, EF-Core-Security is developed in parallel with it.

EF-Core-Security allow to restrict an access to a data for CRUD operations on entity-level, object-level and member-level. Also it provides built-in authorization module. 

For example, EF-Core-Security excellent suits to make an application in which need to restrict an access to a data for some users, but to provide full access for other users. And there is system administrator, which controls security rules and allows or denies an access to a data.

## Getting started 

EF-Core-Security is an extension for a EF Core DbContext. It is suitable for WinForms/ASP.NET/MVC/Android/DevExtreme/Any JSFramework/Win10/iOS and other application development. You need MS Visual Studio to develop, test and debug. If you have already worked with EF6 or EFCore, then it won't be difficult for you to understand system work principles. 

To get started you need to get \ef7security.

- Latest EF Core binaries are placed into the ef7security/ef7-bin folder.

- The main functionality of EF-Core-Security is located into /ef7security/DevExpress.EntityFramework.SecurityDataStore/

- The extension for authorization functionality is located into /ef7security/DevExpress.EntityFramework.Authorization/ 

- Unit tests are located in /ef7security/DevExpress.EntityFramework.SecurityDataStore.Tests/ 

## Demos 

See also some demos, which demonstrate an applications examples using EF-Core-Security:

- [EFCoreSecurityConsoleDemo](https://github.com/DevExpress/EF-Core-Security/wiki/How-to-create-console-application-with-EF-Core-Security): Small console application demonstrates simple scenario of authorization and a restricted access to data.

- EFCoreSecurityODataConsoleClient (link): Client-server application includes console application on client side and OData service on server side. The client requests a data from the server and displays it via console. The server controls an access to data and determines which data will be sent to the client.

- EFCoreSecurityODataAndroidClient (link): Client-server application includes Android client and OData service on the server side.

- EFCoreSecurityODataDevExtremeClient (link): Client-server application includes DevExtreme application on client side and OData service on the server side.
