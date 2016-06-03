TestApp.Login = function (params) {
    function btnLogin_OnClick(e) {
        // console.log("old " + TestApp.app.UserName + " " + TestApp.app.Password);
        // TestApp.app.UserName = loginModel.txtUserNameSettings.value();
        // TestApp.app.Password = loginModel.txtPasswordSettings.value();
        // console.log("new " + TestApp.app.UserName + " " + TestApp.app.Password);
        var uri = TestApp.app.router.format({
            view: 'Index'
        });
        TestApp.app.navigate(uri);
    }
    function User(name, password) {
        var self = this;
        self.name = name;
        self.password = password;
    }
    var loginModel = {
        users: ko.observableArray([
            new User("John", "John"),
            new User("Admin", "Admin"),
        ]),

        updateCurrentUser: function (e) {
            TestApp.app.UserName = e.value.name;
            TestApp.app.Password = e.value.password;
        },

        txtUserNameSettings: {
            value: ko.observable(TestApp.app.UserName)
        },
        txtPasswordSettings: {
            value: ko.observable(TestApp.app.Password),
            mode: 'password'
        },
        btnLoginSettings: {
            text: 'Login',
            onClick: btnLogin_OnClick,
            clickAction: btnLogin_OnClick
        }
    };
    return loginModel;
};