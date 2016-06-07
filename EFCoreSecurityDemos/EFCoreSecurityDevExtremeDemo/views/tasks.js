TestApp.Tasks = function (params) {
    var viewModel = {
        dataGridOptions: {
            dataSource: new DevExpress.data.DataSource(TestApp.db.sampleData.Tasks),

            columns: [
               { dataField: "Note", width: '30%' },
               { dataField: "StartDate", width: '30%' },
               { dataField: "PercentCompleted", width: '30%', caption: "Percent" }],

            onRowClick: function (info) {
                TestApp.app.navigate('tasks_detail/' + info.data.Id);
            },
        }
    };

    ko.utils.extend(viewModel.dataGridOptions, TestApp.BaseDataGridOptions);

    return viewModel;
};