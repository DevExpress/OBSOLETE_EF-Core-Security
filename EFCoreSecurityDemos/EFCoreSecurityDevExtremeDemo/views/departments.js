TestApp.Departments = function (params) {
    var viewModel = {
        dataGridOptions: {
            dataSource: new DevExpress.data.DataSource(TestApp.db.sampleData.Departments),

            columns: ["Id", "Title", "Office"],

            onRowClick: function (info) {
                TestApp.app.navigate('departments_detail/' + info.data.Id);
            }
        }
    };

    ko.utils.extend(viewModel.dataGridOptions, TestApp.BaseDataGridOptions);

    return viewModel;
};