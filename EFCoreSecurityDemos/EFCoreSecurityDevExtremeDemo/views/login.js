TestApp.Login = function (params) {
    function btnLogin_OnClick(e) {
        window.TestApp.config.navigation.map(function (x, index, array) {
            array[index].visible(true);
        });
        TestApp.app.navigate('Contacts', { root: true });
    }
    function User(name, password) {
        var self = this;
        self.name = name;
        self.password = password;
    }
    var loginModel = {
        users: ko.observableArray([ "John", "Admin" ]),

        updateCurrentUser: function (e) {
            TestApp.app.UserName = e.value;
            TestApp.app.Password = e.value;
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
        },

        viewShown: function () {
            var hideNavElements = ["#Departments", "#Contacts", "#Tasks"];
            window.TestApp.config.navigation.map(function (x, index, array) {
                var currentElement = array[index];
                if (hideNavElements.indexOf(currentElement.onExecute) >= 0)
                    currentElement.visible(false);
            });
        }
    };
    return loginModel;
};