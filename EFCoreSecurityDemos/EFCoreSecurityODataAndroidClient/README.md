This example illustrates how to create Android OData v4 client application that will be communicate with EF Core Security.

This application uses [Apache Olingo](https://olingo.apache.org/) opensource Java library as OData client and [SlidingMenu](https://github.com/jfeinstein10/SlidingMenu).

Application structure
--------------
The application has 3 activities:
* Login activity
* ListView activity
* DetailView activity


Login activity
--------------
You can change user in the Login acitvity (John and Admin are available) and press Login button.


ListView activity
-----------------
Next, the ListView activity will be loaded.
By default, Contacts entities are loaded.

The list of entities is loaded as a table. Not all entities' fields are displayed in this activity.

There are following types of entities:
* Contacts
* Departments
* Tasks

You can switch between them in the SlidingMenu (swipe right to open it or press the arrow in the top-left corner).
If you tap on a row in the table, the DetailView activity will be opened.


DetailView activity
-----------------
All entity fields are displayed in this activity, with navigation items and collections (blue color).


Details
--------------
You need Android Studio 2+ to compile this project.
Minimum Android API version for this application is 7 (Eclair, 2.1), but API version 23 is required to build application.

Here is some info about implementation details.
Data is loaded from the test OData server by ODataEntityLoader class.
Entities (android java objects) are created by EntityCreator class.
Blocked members are processed (text is being replaced to "Protected", color is being changed to Orange) in NavigationListViewAdapter class.

All network work is placed to a separate async task (LoadEntitiesTask class)

Also, there is an implementation of HTTP Basic Authentication (PreemptiveAuthInterceptor and PreemptiveBasicAuthHttpClientFactory)