TestApp.Contacts = function(params) {
    var viewModel = {
        dSource: new DevExpress.data.DataSource(TestApp.db.sampleData.Contacts)
    };
    return viewModel;
};