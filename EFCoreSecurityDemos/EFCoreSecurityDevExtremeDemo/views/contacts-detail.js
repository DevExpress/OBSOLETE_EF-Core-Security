TestApp.contacts_detail = function (params) {
    var id = parseInt(params.id);
    var contact = new TestApp.ContactViewModel();
    var isReady = $.Deferred();
    var viewModel = {
        id: id,
        isReady: isReady.promise(),
        contact: contact,

        dSource: new DevExpress.data.DataSource(TestApp.db.sampleData.Contacts),

        viewShown: function () {
                // debugger;
            TestApp.db.sampleData.Contacts.byKey(id).done(function (data) {
                debugger;
                // contact(data[0]);
                contact.fromJS(data[0]);
                isReady.resolve();
            });
        }
    };
    return viewModel;
};