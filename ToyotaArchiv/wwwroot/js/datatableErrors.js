/*
if (jQuery) {
    alert("jquery is loaded");
} else {
    alert("Not loaded");
}
*/

$(document).ready(function () {

    // Setup - add a text input to each footer cell
    $('#datatableErrors tfoot th').each(function () {
        var title = $(this).text();
        $(this).html('<input type="text" placeholder=" ' + title + '" />');
    });

    $('#datatableErrors').DataTable({

        "dom": '<"top"if>rt<"bottom"lp><"clear">', //OK
        "lengthMenu": [[25, 10, 30, 50, -1], [25, 10, 30, 50, "Všetky"]],
        //"lengthMenu": [25],  //OK je tam len jedna moznost na vyber
        "search": { return: true }, //Search box nad tabulkou  hlada az po stlaceni Enter

        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.18/i18n/Slovak.json"
        },
        initComplete: function () {
            // Apply the search
            this.api().columns().every(function () {
                var that = this;

                $('input', this.footer()).on('keydown', function (ev) {
                    if (ev.keyCode == 13) { //only on enter keypress (code 13)
                        that
                            .search(this.value)
                            .draw();
                    }
                });
            });
        },
        "processing": true, // for show progress bar
        "serverSide": true, // for process server side
        "filter": true,     // this is for disable filter (search box)
        "orderMulti": false, // for disable multiple column at once
        "ajax": {
            "url": "../Errors/LoadData",
            "type": "POST",
            "datatype": "json"
        },

        "columnDefs":
            [{
                "targets": [0],//ErrorLogID
                "visible": false,
                "searchable": false
            },

            {
                "targets": [1],//ErrorDate
                "visible": true,
                "searchable": true,
                render: $.fn.dataTable.render.moment('YYYY-MM-DDTHH:mm:ss', 'DD.MM.YYYY HH:mm')
            },
            {
                "targets": [2], //ErrorMsg
                "visible": true,
                "searchable": true,
            },

            {
                "targets": [3],//ErrorNUmber
                "visible": true,
                "searchable": false,
            },
            {
                "targets": [4], //ErrorProcedure
                "visible": true,
                "searchable": false,
            },
            {
                "targets": [5], //ErrorLine
                "visible": true,
                "searchable": false,
            },
            {
                "targets": [6], //User
                "visible": true,
                "searchable": true,
            },


            ],

        "columns": [
            { "data": "errorLogId", "name": "ErrorLogId", "autoWidth": true },
            { "data": "errorDate", "name": "ErrorDate", "autoWidth": true },
            { "data": "errorMsg", "name": "ErrorMsg", "autoWidth": true },
            { "data": "errorNumber", "name": "ErrorNumber", "autoWidth": true },
            { "data": "errorProcedure", "name": "ErrorProcedure", "autoWidth": true },
            { "data": "errorLine", "name": "ErrorLine", "autoWidth": true },
            { "data": "user", "name": "User", "autoWidth": true },
        ]

    });

});
