TestApp.tasks_detail = function (params) {
    var id = parseInt(params.id);
    var task = new TestApp.TaskViewModel();
    var isReady = $.Deferred();
    var viewModel = {
        id: id,
        isReady: isReady.promise(),
        task: task,

        dSource: new DevExpress.data.DataSource(TestApp.db.sampleData.Tasks),

        viewShown: function () {
            // TestApp.db.sampleData.Contacts.byKey(id, { expand: ["Department"] }).done(function (data) {
            TestApp.db.sampleData.Tasks.byKey(id).done(function (data) {
                // debugger;
                task.fromJS(data[0]);
                isReady.resolve();
            });
        }
    };
    return viewModel;
};