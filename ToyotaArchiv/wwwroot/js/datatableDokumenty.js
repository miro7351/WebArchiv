

$('#datatableDocuments').dataTable({


    "language": {
        "url": "//cdn.datatables.net/plug-ins/1.10.18/i18n/Slovak.json"
    },
    "processing": true, // for show progress bar
    "serverSide": true, // for process server side
    "filter": true, // this is for disable filter (search box)
    "orderMulti": false, // for disable multiple column at once
    "ajax": {
        "url": "/Documents/LoadData",
        "type": "POST",
        "datatype": "json"
    },
    "columnDefs":
        [{
            "targets": [0],
            "visible": true,
            "searchable": false
        },
        
        {
            "targets": [8],
            "visible": true,
            "searchable": false,
            render: $.fn.dataTable.render.moment('YYYY-MM-DDTHH:mm:ss', 'DD.MM.YYYY HH:MM')
            },
        {
                "targets": [10],
                "visible": true,
                "searchable": false,
                render: $.fn.dataTable.render.moment('YYYY-MM-DDTHH:mm:ss', 'DD.MM.YYYY HH:MM')
            },
        ],
    "columns": [
        { "data": "dokumentId", "name": "DokumentId", "autoWidth": true },
        { "data": "zakazkaTg", "name": "ZakazkaTg", "autoWidth": true },
        { "data": "nazovDokumentu", "name": "NazovDokumentu", "autoWidth": true },
        { "data": "nazovSuboru", "name": "NazovSuboru", "autoWidth": true },
        { "data": "dokumentPlatny", "name": "DokumentPlatny", "autoWidth": true },

        { "data": "skupina", "name": "Skupina", "autoWidth": true },
        { "data": "poznamka", "name": "Poznamka", "autoWidth": true },
        { "data": "vytvoril", "name": "Vytvoril", "autoWidth": true },
        { "data": "vytvorene", "name": "Vytvorene", "autoWidth": true },
        { "data": "zmenil", "name": "Zmenil", "autoWidth": true },
        { "data": "zmenene", "name": "Zmenene", "autoWidth": true },
    ]

});