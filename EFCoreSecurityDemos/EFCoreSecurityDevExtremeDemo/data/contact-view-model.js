(function () {
    TestApp.ContactViewModel = function (data) {
        this.Id = ko.observable();
        this.Address = ko.observable();
        this.Name = ko.observable();
        this.Department = ko.observable();
        if (data)
            this.fromJS(data);
    };

    $.extend(TestApp.ContactViewModel.prototype, {
        toJS: function () {
            return {
                Id: this.Id(),
                Address: this.Address(),
                Name: this.Name(),
                DepartmentId: this.Department,
            };
        },

        fromJS: function (data) {
            if (data) {
                this.Id(data.Id);
                this.Address(data.Address);
                this.Name(data.Name);
                this.Department(data.DepartmentId);
            }
        },

        clear: function () {
            this.Id(undefined);
            this.Address(undefined);
            this.Name(undefined);
            this.DepartmentId(undefined);
        }
    });
})();