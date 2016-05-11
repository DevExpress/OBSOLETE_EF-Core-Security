# EF-Core-Security 
## About 

This project is security system for Entity Framework Core. As Entity Framework Core is currently pre-release, EF-Core-Security is developing in parallel with it.

EF-Core-Security allow to restrict an access to a data for CRUD operations on entity-level, object-level and member-level. Also it provide built-in authorization means. 

For example, EF-Core-Security excellent suit to make an application in which need to restrict an access to a data for some users, but to provide full access for other users. And there is system administrator, which controls security rules and allows or denies an access to a data. 

## Getting started 

EF-Core-Security is an extension for a EF Core DbContext. It is suitable for WinForms/ASP.NET/MVC/Android/DevExtreme/AnyJSFramework/Win8/iOS and other application development. You need MS Visual Studio to develop, testing and debug. If you have already worked with EF6 or EFCore, then you will not be difficult to understand system work principles. 

To getting started you need to get \ef7security.

- Latest EF Core binaries are placed into the ef7security/ef7-bin folder.

- The main functionality of EF-Core-Security located in  /ef7security/DevExpress.EntityFramework.SecurityDataStore/

- The extension for authorization functionality located in /ef7security/DevExpress.EntityFramework.Authorization/ 

- The unit tests located in /ef7security/DevExpress.EntityFramework.SecurityDataStore.Tests/ 

## Demos 

See also some demos, which demonstrate an applications examples using EF-Core-Security:

- EFCoreSecurityConsoleDemo (ссылка): Small console application realizes simple scenario of authorization and a restricted access to data.

- EFCoreSecurityODataConsoleClient (ссылка): Client-server application include console application on client side and OData service on server side. The client requests a data from the server and displays it in console. The server controls an access to data and determines which data will be sent to the client.

- EFCoreSecurityODataAndroidClient (ссылка): Client-server application include mobile application based on Android on client side and OData service on server side.

- EFCoreSecurityODataDevExtremeClient (ссылка): Client-server application include DevExtreme application on client side and OData service on server side.
