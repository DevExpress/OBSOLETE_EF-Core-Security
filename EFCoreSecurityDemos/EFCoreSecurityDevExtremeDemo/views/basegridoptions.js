(function () {
    TestApp.BaseDataGridOptions = {
        paging: {
            pageSize: 10
        },

        pager: {
            showPageSizeSelector: true,
            allowedPageSizes: [5, 10, 20],
            showInfo: true
        },

        onCellPrepared: function (options) {
            var fieldData = options.value,
                fieldHtml = "",
                model = options.model;

            if (options.rowType == "data") {
                var columnCaption = options.column.caption;
                var blockedMembers = options.data.BlockedMembers;
                if (blockedMembers.indexOf(columnCaption) >= 0) {
                    options.cellElement.addClass("protected");
                    fieldHtml += "<span>Protected</span>";
                } else {
                    fieldHtml = fieldData.value;
                }
                options.cellElement.html(fieldHtml);
            }
        },
    };
})();