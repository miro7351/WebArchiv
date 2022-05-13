$(document).ready(function () {


   
    // Setup - add a text input to each footer cell
    $('#datatableLogy tfoot th').each(function () {
        var title = $(this).text();
        $(this).html('<input type="text" placeholder=" ' + title + '" />');
    });


    $('#datatableLogy').dataTable({

        "dom": '<"top"if>rt<"bottom"lp><"clear">',
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
        "filter": true, // this is for disable filter (search box)
        "orderMulti": false, // for disable multiple column at once
        "ajax": {
            "url": "../Logs/LoadData",
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
            { "data": "logId", "name": "LogId", "autoWidth": true },
            { "data": "logDate", "name": "LogDate", "autoWidth": true },
            { "data": "tableName", "name": "TableName", "autoWidth": true },
            { "data": "logMessage", "name": "LogMessage", "autoWidth": true },
            { "data": "userAction", "name": "UserAction", "autoWidth": true },
            { "data": "userName", "name": "UserName", "autoWidth": true },
        ]

    });
    //Filtre su v headeri tabulky
    $('#datatableLogy tfoot tr').appendTo('#datatableLogy thead');
});