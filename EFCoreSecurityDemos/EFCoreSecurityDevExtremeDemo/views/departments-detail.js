TestApp.departments_detail = function (params) {
    var id = parseInt(params.id);
    var department = new TestApp.DepartmentViewModel();
    var isReady = $.Deferred();
    var viewModel = {
        id: id,
        isReady: isReady.promise(),
        department: department,

        dSource: new DevExpress.data.DataSource(TestApp.db.sampleData.Departments),

        viewShown: function () {
            // TestApp.db.sampleData.Contacts.byKey(id, { expand: ["Department"] }).done(function (data) {
            TestApp.db.sampleData.Departments.byKey(id).done(function (data) {
                // debugger;
                department.fromJS(data[0]);
                isReady.resolve();
            });
        }
    };
    return viewModel;
};