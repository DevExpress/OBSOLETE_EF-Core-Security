(function () {
    TestApp.BaseViewModel = {
        processProtectedContent: function () {
            for (var i = 0; i < this.BlockedMembers().length; i++) {
                var blockedMember = this.BlockedMembers()[i];
                this[blockedMember] = "Protected";
            }
        },

        getValueClass: function (name) {
            var cssClass = "";
            if (this.BlockedMembers().indexOf(name) >= 0) {
                cssClass = "protected";
            }
            return cssClass;
        }
    };
})();