TestApp.Departments = function (params) {
    var viewModel = {
        dSource: new DevExpress.data.DataSource(TestApp.db.sampleData.Departments)
    };
    return viewModel;
};