This example illustrates how to create an Android OData v4 client application that will communicate with EF Core Security.

This application uses the [Apache Olingo](https://olingo.apache.org/) and [SlidingMenu](https://github.com/jfeinstein10/SlidingMenu) open source libraries.

Application structure
--------------
The application has 3 activities:
* Login activity
* ListView activity
* DetailView activity


Login Activity
--------------
You can change user in the Login acitvity ('John' and 'Admin' are available) and tap the Login button.


ListView Activity
-----------------
After login, the ListView activity will is loaded.
By default, Contacts entities are loaded.

The list of entities is loaded as a table. Not all entities' fields are displayed in this activity.

There are following entity types types:
* Contacts
* Departments
* Tasks

You can switch between them in the SlidingMenu (swipe right to open it or press the arrow in the top-left corner).
If you tap on a table row, the DetailView activity will be opened.


DetailView Activity
-----------------
All entity fields are displayed in this activity, with navigation items and collections (blue color).

Requirements
------------
The Android Studio 2+ is required to compile this project. Minimum Android API version for this application is 7 (Eclair, 2.1), but API version 23 is required to build the application.

Implementation Details
--------------
Data is loaded from the test OData server by the *ODataEntityLoade*r object. Entities (Android Java objects) are created by the *EntityCreator* object. Members with resticted access are processed (values are replaced to "Protected", color is changed to 'Orange') in the *NavigationListViewAdapter* class. All network communications are execuded in a separate asynchronous task (the *LoadEntitiesTask* class). The *HTTP Basic Authentication* is implementated in*PreemptiveAuthInterceptor* and *PreemptiveBasicAuthHttpClientFactory* classes.
