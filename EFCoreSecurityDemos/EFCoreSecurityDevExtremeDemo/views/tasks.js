TestApp.Tasks = function (params) {
    var viewModel = {
        dSource: new DevExpress.data.DataSource(TestApp.db.sampleData.Tasks)
    };
    return viewModel;
};