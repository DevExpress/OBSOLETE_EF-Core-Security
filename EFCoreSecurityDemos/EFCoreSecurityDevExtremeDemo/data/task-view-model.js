(function () {
    TestApp.TaskViewModel = function (data) {
        this.BlockedMembers = ko.observableArray();
        this.Id = ko.observable();
        this.Description = ko.observable();
        this.Note = ko.observable();
        this.StartDate = ko.observable();
        this.DateCompleted = ko.observable();
        this.PercentCompleted = ko.observable();

        if (data)
            this.fromJS(data);
    };

    $.extend(TestApp.TaskViewModel.prototype, {
        toJS: function () {
            return {
                BlockedMembers: this.BlockedMembers(),
                Id: this.Id(),
                Description: this.Description(),
                Note: this.Note(),
                StartDate: this.StartDate(),
                DateCompleted: this.DateCompleted(),
                PercentCompleted: this.PercentCompleted()
            };
        },

        fromJS: function (data) {
            if (data) {
                this.BlockedMembers(data.BlockedMembers);
                this.Id(data.Id);
                this.Description(data.Description);
                this.Note(data.Note);
                this.StartDate(data.StartDate);
                this.DateCompleted(data.DateCompleted);
                this.PercentCompleted(data.PercentCompleted);

                this.processProtectedContent();
            }
        },

        clear: function () {
            this.BlockedMembers(undefined);
            this.Id(undefined);
            this.Description(undefined);
            this.Note(undefined);
            this.StartDate(undefined);
            this.DateCompleted(undefined);
            this.PercentCompleted(undefined);
        }
    });

    $.extend(TestApp.TaskViewModel.prototype, TestApp.BaseViewModel);
})();