To view a demo application on our cloud server, click the link [EFCoreSecurityDevExtremeDemo](http://efcoresecuritydevextremedemoweb.azurewebsites.net/)

This solution demonstrates how to represent  [EFCoreSecurityODataService](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurityDemos/EFCoreSecurityODataService)  as visual representation by dint of [DevExtreme](http://js.devexpress.com/).

First you need create [OData service](https://github.com/DevExpress/EF-Core-Security/tree/master/EFCoreSecurityDemos/EFCoreSecurityODataService) or using [our service](http://www.odata.org/getting-started/). After creating OData service you will need to use any visual widgets from DevEhtreme. Then connect your visual widgets to your OData service. How do this you look at this link [How to load and display OData entities](https://www.devexpress.com/Support/Center/Question/Details/Q450569). To create data source with our OData service use following code:
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

