window.TestApp = window.TestApp || {};

String.prototype.format = function () {
    var formatted = this;
    for (var i = 0; i < arguments.length; i++) {
        var regexp = new RegExp('\\{' + i + '\\}', 'gi');
        formatted = formatted.replace(regexp, arguments[i]);
    }
    return formatted;
};

$(function () {
    TestApp.app = new DevExpress.framework.html.HtmlApplication({
        namespace: TestApp,
        layoutSet: DevExpress.framework.html.layoutSets[TestApp.config.layoutSet],
        navigation: TestApp.config.navigation
    });
    TestApp.app.UserName = 'John';
    TestApp.app.Password = 'John';
    TestApp.app.router.register(":view/:id", { view: "Login", id: undefined });
    TestApp.app.navigate();
});
