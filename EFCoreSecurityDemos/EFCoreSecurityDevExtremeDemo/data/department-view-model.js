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
            this.Title(undefined);
            this.Office(undefined);
        }
    });
})();