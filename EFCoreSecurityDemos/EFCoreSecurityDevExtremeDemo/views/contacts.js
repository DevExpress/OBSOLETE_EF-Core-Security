TestApp.Contacts = function(params) {
    var viewModel = {
        dataGridOptions: {
            dataSource: new DevExpress.data.DataSource(TestApp.db.sampleData.Contacts),
          
            columns: ["Id", "Name", "Address"],

            onRowClick: function (info) {
                TestApp.app.navigate('contacts_detail/' + info.data.Id);
            },            
        }
    };

    ko.utils.extend(viewModel.dataGridOptions, TestApp.BaseDataGridOptions);

    return viewModel;
};