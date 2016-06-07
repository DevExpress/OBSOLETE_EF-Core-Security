(function () {
    TestApp.DepartmentViewModel = function (data) {
        this.BlockedMembers = ko.observableArray();
        this.Id = ko.observable();
        this.Title = ko.observable();
        this.Office = ko.observable();

        if (data)
            this.fromJS(data);
    };

    $.extend(TestApp.DepartmentViewModel.prototype, {
        toJS: function () {
            return {
                BlockedMembers: this.BlockedMembers(),
                Id: this.Id(),
                Title: this.Title(),
                Office: this.Office()
            };
        },

        fromJS: function (data) {
            if (data) {
                this.BlockedMembers(data.BlockedMembers);
                this.Id(data.Id);
                this.Title(data.Title);
                this.Office(data.Office);

                this.processProtectedContent();
            }
        },

        clear: function () {
            this.BlockedMembers(undefined);
            this.Id(undefined);
            this.Title(undefined);
            this.Office(undefined);
        }
    });

    $.extend(TestApp.DepartmentViewModel.prototype, TestApp.BaseViewModel);
})();