﻿

$('#datatableLogy').dataTable({


    "language": {
        "url": "//cdn.datatables.net/plug-ins/1.10.18/i18n/Slovak.json"
    },
    "processing": true, // for show progress bar
    "serverSide": true, // for process server side
    "filter": true, // this is for disable filter (search box)
    "orderMulti": false, // for disable multiple column at once
    "ajax": {
        "url": "/Logs/LoadData",
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