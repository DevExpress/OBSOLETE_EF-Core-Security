(function() {
    TestApp.db = {
        sampleData: new DevExpress.data.ODataContext({
            url: "http://efcoresecurityodataservicedemo.azurewebsites.net/",
            // url: "http://192.168.50.200:800",
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
            /*
            entities: {
                ContactsList: {
                    key: "Contacts",
                    name: "Contacts"
                },
            },
            */
            entities: {
                Contacts: { key: "Id" },
                Departments: { key: "Id" },
                ContactTasks: { key: "Id" }
            },
            beforeSend: function (request) {
                request.headers["Authorization"] = "Basic " + DevExpress.data.base64_encode([TestApp.app.UserName, TestApp.app.Password].join(":"));
                console.log("authorization header: " + request.headers["Authorization"]);

                // request.params.UserName = TestApp.app.UserName;
                // request.params.Password = TestApp.app.Password;
            }        
        })
    };
})();