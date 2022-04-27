

$('#datatableZakazky').dataTable({


    "language": {
        "url": "//cdn.datatables.net/plug-ins/1.10.18/i18n/Slovak.json"
    },
    "processing": true, // for show progress bar
    "serverSide": true, // for process server side
    "filter": true, // this is for disable filter (search box)
    "orderMulti": false, // for disable multiple column at once
    "ajax": {
        "url": "/ZakazkyJQ/LoadData",
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
            "render": function (data, type, row) {
                return '<a  href="/ZakazkyJQ/Details/?zakazkaTg=' + $.trim(row['zakazkaTg']) + '">' + data + '</a>';
            },
            "targets": [2]
        },
            {
                "targets": [1],
                "visible": true,
                "searchable": false,
                render: $.fn.dataTable.render.moment('YYYY-MM-DDTHH:mm:ss', 'DD.MM.YYYY')
            },
        ],
    "columns": [
        { "data": "zakazkaId", "name": "ZakazkaId", "autoWidth": true },
        { "data": "vytvorene", "name": "Datum", "autoWidth": true },
        { "data": "zakazkaTg", "name": "ZakazkaTg", "autoWidth": true },
        { "data": "zakazkaTb", "name": "ZakazkaTb", "autoWidth": true },
        { "data": "vin", "name": "Vin", "autoWidth": true },
        { "data": "cws", "name": "Cws", "autoWidth": true },
        { "data": "cisloProtokolu", "name": "CisloProtokolu", "autoWidth": true },


        { "data": "ukoncena", "name": "Ukoncena", "autoWidth": true },
        { "data": "poznamka", "name": "Poznamka", "autoWidth": true },
        
    ]
});