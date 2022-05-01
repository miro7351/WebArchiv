﻿

$(document).ready(function () {
    // Setup - add a text input to each footer cell
    $('#datatableErrors tfoot th').each(function () {
        var title = $(this).text();
        $(this).html('<input type="text" placeholder="Najdi ' + title + '" />');
    });

    $('#datatableErrors').dataTable({


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
        "filter": false,     // this is for disable filter (search box)
        "orderMulti": false, // for disable multiple column at once
        "ajax": {
            "url": "/Errors/LoadData",
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs":
            [{
                "targets": [0],
                "visible": false,
                "searchable": false
            },

            {
                "targets": [1],
                "visible": true,
                "searchable": true,
                render: $.fn.dataTable.render.moment('YYYY-MM-DDTHH:mm:ss', 'DD.MM.YYYY HH:MM')
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