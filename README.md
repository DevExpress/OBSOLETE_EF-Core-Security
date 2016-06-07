# EF Core Security 
## About 

This project allows to restrict an access to a data for CRUD operations on entity-level, object-level and member-level.

## Getting started 

We built and published an OData service and DevExtreme application that works with this service.

1. See odata service at [SecurityODataService](http://efcoresecurityodataservicedemo.azurewebsites.net/). This service is protected, use 'Admin' as user name and 'Admin' as password (or 'John'/'John' to login as restricted user) to read data from this service. You can expect it directly or create any application to read and modify its data. See the following links:
- [Metadata](http://efcoresecurityodataservicedemo.azurewebsites.net/$metadata)
- [Contacts](http://efcoresecurityodataservicedemo.azurewebsites.net/Contacts)
- [Departments](http://efcoresecurityodataservicedemo.azurewebsites.net/Departments)
- [Tasks](http://efcoresecurityodataservicedemo.azurewebsites.net/Tasks)
See [Demo ODataService](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurityDemos/EFCoreSecurityODataService) to build it locally.

2. See [DevExtreme](http://js.devexpress.com/) application at [DevExtremeClient](link). This application is based on the published OData service and allows to read and modify its data: Client-server application includes DevExtreme application on client side and OData service on the server side. See (link) to build it locally.

3. How to build and run [console demo](link) to work with ODataService.
4. How to build and run [Android demo](link) to work with ODataService.
5. How to build and run [console demo](link) to work with DBContext directly.
