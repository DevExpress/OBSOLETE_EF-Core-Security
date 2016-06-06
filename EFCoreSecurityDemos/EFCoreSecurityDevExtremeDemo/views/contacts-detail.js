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
            TestApp.db.sampleData.Contacts.byKey(id, { expand: ["Department"] }).done(function (data) {
            // TestApp.db.sampleData.Contacts.byKey(id).done(function (data) {
                // debugger;
                contact.fromJS(data[0]);
                isReady.resolve();
            });
        }
    };
    return viewModel;
};