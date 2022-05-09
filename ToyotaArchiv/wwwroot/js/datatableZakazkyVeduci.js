
$(document).ready(function () {

    // Setup - add a text input to each footer cell
    $('#datatableZakazky tfoot th').each(function () {
        var title = $(this).text();
        if (title == "VIN")
            $(this).html('<input id="VIN" type="text" class="filter1" placeholder=" ' + title + '" style="width:180px" />');
        else if (title == "") {
            ; /*$(this).html('<input type="Button"   value="Vymazať""  onclick="ClearFilter()"/>');*/
        }
        else if (title == "Ukoncena") {
            $(this).html('<input type="text" class="filter1" placeholder=" ' + "A/N" + '" style="width:50px" />');
        }
        else if (title == "CWS") {
            $(this).html('<input type="text" class="filter1" placeholder=" ' + "CWS" + '" style="width:120px" />');
        }
        else if (title == "CisloProtokolu") {
            $(this).html('<input type="text" class="filter1" placeholder=" ' + "Číslo prot." + '" style="width:120px" />');
        }
        else if (title == "spz") {
            $(this).html('<input id="SPZ"  type="text" class="filter1" placeholder=" ' + "SPZ" + '" style="width:120px" />');
        }
        else {
            $(this).html('<input type="text" class="filter1" placeholder=" ' + title + '" style="width:100px" />');
        }
    });


    $('#datatableZakazky').dataTable({


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
            "url": "../ZakazkyJQ/LoadData",
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs":
            [{
                "targets": [0],//ZakazkaId
                "visible": false,
                "searchable": false
            },
            {
                "targets": [1],//Vytvorene
                "visible": true,
                "searchable": false,
                render: $.fn.dataTable.render.moment('YYYY-MM-DDTHH:mm:ss', 'DD.MM.YYYY'),
            },
            {
                "targets": [2],//ZakazkaTg
                "render": function (data, type, row) {
                    return '<a  href="/ZakazkyJQ/Details/?zakazkaTg=' + $.trim(row['zakazkaTg']) + '">' + data + '</a>';
                }
            },
            {
                "targets": [3],//ZakazkaTb
                "visible": true,
                "searchable": true
            },
            {
                "targets": [4],//VIN
                "visible": true,
                "searchable": true
            },
            {
                "targets": [5],//CWS
                "visible": true,
                "searchable": true
            },

            {
                "targets": [6],//CisloProtokolu
                "visible": true,
                "searchable": true
            },
            {
                "targets": [7],//Ukoncena
                "visible": true,
                "searchable": true,
                //render: function (data, type) {  //NEJDE
                //    if (type === 'display') {
                //        if (data == 'A') {
                //            let color = 'red';
                //        }
                //        return '<span style="color:' + color + '">' + data + '</span>';
                //    }
                //}
                },
                {
                    "targets": [8],/*SPZ */
                    "visible": true,
                    "searchable": false

                },
                {
                    "targets": [9],/*Vlastnik */
                    "visible": true,
                    "searchable": false

                },

            {
                "targets": [10],/*Poznamka */
                "visible": false,
                "searchable": false

            },
           
            {
                "targets": [11],//Vymazat
                "render": function (data, type, row) {
                    return '<a href="/ZakazkyJQ/Delete/' + $.trim(row['zakazkaId']) + '">' + 'Vymazať</a>';
                }
            },
            ],
        "columns": [
            { "data": "zakazkaId", "name": "ZakazkaId", "autoWidth": true },
            { "data": "vytvorene", "name": "Vytvorene", "autoWidth": true },
            { "data": "zakazkaTg", "name": "ZakazkaTg", "autoWidth": true },
            { "data": "zakazkaTb", "name": "ZakazkaTb", "autoWidth": true },
            { "data": "vin", "name": "Vin", "autoWidth": true },
            { "data": "cws", "name": "Cws", "autoWidth": true },
            { "data": "cisloProtokolu", "name": "CisloProtokolu", "autoWidth": true },

            { "data": "ukoncena", "name": "Ukoncena", "autoWidth": true },
            { "data": "spz", "name": "SPZ", "autoWidth": true },
            { "data": "vlastnik", "name": "Vlastnik", "autoWidth": true },
            { "data": "poznamka", "name": "Poznamka", "autoWidth": false },
        ]

    });

    //Filtre su v headeri tabulky
    $('#datatableZakazky tfoot tr').appendTo('#datatableZakazky thead');

});



