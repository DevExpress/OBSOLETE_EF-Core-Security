This demo application is publised at [EFCoreSecurityDevExtremeDemo](http://efcoresecuritydevextremedemoweb.azurewebsites.net/)

The example demonstrates how to access  [EFCoreSecurityODataService](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurityDemos/EFCoreSecurityODataService)  using a  [DevExtreme](http://js.devexpress.com/) application.

First, create [OData service](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurityDemos/EFCoreSecurityODataService) or use [an existing demo service](http://efcoresecurityodataservicedemo.azurewebsites.net/). Then, you can use any visual widgets from DevExtreme and [connect them to the OData service](https://www.devexpress.com/Support/Center/Question/Details/Q450569). Use the following code to create a data source with the OData service:

```
  (function() {
    TestApp.db = {
        sampleData: new DevExpress.data.ODataContext({
            url: "http://efcoresecurityodataservicedemo.azurewebsites.net/",           
            type: 'odata',
            version: 4,
            contentType: 'application/json',
            headers: { 'Access-Control-Allow-Origin': '*' },
            crossDomain: true,
            dataType: "jsonp",
            async: true,
            errorHandler: function (error) {
                if (error.httpStatus == 401)
                    TestApp.app.navigate('Login');
                else alert(error.message);
            },
            entities: {
                Contacts: { key: "Id" },
                Departments: { key: "Id" },
                Tasks: { key: "Id" }
            },
            beforeSend: function (request) {
                request.headers["Authorization"] = "Basic " + DevExpress.data.base64_encode([TestApp.app.UserName, TestApp.app.Password].join(":"));
            }        
        })
    };
})
```
Use following code to connect widgets to the data source.
```
    var viewModel = {
        dataGridOptions: {
            dataSource: new DevExpress.data.DataSource(TestApp.db.sampleData.Contacts),
     ...
```

Every entity contains blocked and read-only members('BlockedMembers/'ReadOnlyMembers'). By default, a blocked member value is replaced by a default value (or null). You can replace the blocked value by the 'Protected Content' text. The following code demonstrating how to do this in a grid view:
```
(function () {
    TestApp.BaseDataGridOptions = {
        paging: {
            pageSize: 10
        },

        pager: {
            showPageSizeSelector: true,
            allowedPageSizes: [5, 10, 20],
            showInfo: true
        },

        onCellPrepared: function (options) {
            var fieldData = options.value,
                fieldHtml = "",
                model = options.model;

            if (options.rowType == "data") {
                var columnCaption = options.column.caption;
                var blockedMembers = options.data.BlockedMembers;
                if (blockedMembers.indexOf(columnCaption) >= 0) {
                    options.cellElement.addClass("protected");
                    fieldHtml += "<span>Protected</span>";
                } else {
                    fieldHtml = fieldData.value;
                }
                options.cellElement.html(fieldHtml);
            }
        },
    };
})();
```
