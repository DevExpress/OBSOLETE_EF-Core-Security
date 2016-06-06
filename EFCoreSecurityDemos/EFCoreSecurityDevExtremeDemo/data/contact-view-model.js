(function () {
    TestApp.ContactViewModel = function (data) {
        this.BlockedMembers = ko.observableArray();
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
                BlockedMembers: this.BlockedMembers(),
                Id: this.Id(),
                Address: this.Address(),
                Name: this.Name(),
                DepartmentId: this.Department,
            };
        },

        fromJS: function (data) {
            if (data) {
                this.BlockedMembers(data.BlockedMembers);
                this.Id(data.Id);
                this.Address(data.Address);
                this.Name(data.Name);
                this.Department(data.DepartmentId);
                this.processProtectedContent();
            }
        },

        processProtectedContent: function() {
            for(var i = 0; i < this.BlockedMembers().length; i++) {
                var blockedMember = this.BlockedMembers()[i];
                this[blockedMember] = "Protected";
            }
        },

        getValueClass: function(name) {
            var cssClass = "";
            if (this.BlockedMembers().indexOf(name) >= 0) {
                cssClass = "protected";
            }
            return cssClass;
        },

        clear: function () {
            this.BlockedMembers(undefined);
            this.Id(undefined);
            this.Address(undefined);
            this.Name(undefined);
            this.DepartmentId(undefined);
        }
    });
})();