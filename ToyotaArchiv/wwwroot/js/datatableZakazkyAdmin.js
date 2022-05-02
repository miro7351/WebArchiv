
$(document).ready(function () {

    // Setup - add a text input to each footer cell
    $('#datatableZakazky tfoot th').each(function () {
        var title = $(this).text();
        if (title == "VIN")
            $(this).html('<input type="text" placeholder=" ' + title + '" style="width:180px" />');
        else if (title == "") {
            ;
        }
        else if (title == "Ukoncena") {
            $(this).html('<input type="text" placeholder=" ' + "A/N" + '" style="width:50px" />');
        }
        else if (title == "CisloProtokolu") {
            $(this).html('<input type="text" placeholder=" ' + "Číslo prot." + '" style="width:120px" />');
        }
        else {
            $(this).html('<input type="text" placeholder=" ' + title + '" style="width:100px" />');
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
        "filter": true, // this is for disable filter (search box), ak je false nefunguju ani filtre nad stlpcami!!!!
        "orderMulti": false, // for disable multiple column at once

        "ajax": {
            "url": "/ZakazkyJQ/LoadData",
            "type": "POST",
            "datatype": "json"
        },
        "createdRow": function (row, data, index) {
            //alert("created");//tu pridem MH-02.05.2022
            //$('td', row).css("background-color", "#21CE2A");//OK vsetly riadky su svetlo zelene
            //$(row).css("background-color", "#21CE2A");
            //var rowDataLength = data.length;  //undefined
            //alert("rowData.Length" + rowDataLength);
            //var rowData = row.eq(6).innerHTML;//.data();
            //var rowData = $(row.eq(6).innerHTML);//.data();
            //alert("data[6]=" + rowData);//
            //if (data[6] == "N") {
            //    $('td', row).css("background-color", "#21CE2A");
            //    //alert("data[6] = N");
            //}
        },
        "columnDefs":
            [{
                "targets": [0],//ZakazkaId
                "visible": false,
                "searchable": false,

            },
            {
                "targets": [1],//Vytvorene
                "visible": true,
                "searchable": false,
                "width": 200,
                render: $.fn.dataTable.render.moment('YYYY-MM-DDTHH:mm:ss', 'DD.MM.YYYY'),
            },
            {
                "targets": [2],//ZakazkaTg
                "width": 100,
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
                "targets": [8],/*Poznamka */
                "visible": false,
                "searchable": false

            },
            {
                "targets": [9],/*Vytvoril */
                "visible": true,
                "searchable": false

            },
            {
                "targets": [10],/*Zmenil */
                "visible": true,
                "searchable": false

            },
            {
                "targets": [11],//Zmenene
                "visible": true,
                "searchable": false,
                render: $.fn.dataTable.render.moment('YYYY-MM-DDTHH:mm:ss', 'DD.MM.YYYY HH:mm'),
            },

            {
                "targets": [12],//Vymazat
                "render": function (data, type, row) {
                    return '<a href="/ZakazkyJQ/Delete/' + $.trim(row['zakazkaId']) + '">' + 'Vymazať</a>';
                }
            },
            ],
        "columns": [
            { "data": "zakazkaId", "name": "ZakazkaId", "autoWidth": false },
            { "data": "vytvorene", "name": "Vytvorene", "autoWidth": false },
            { "data": "zakazkaTg", "name": "ZakazkaTg", "autoWidth": false },
            { "data": "zakazkaTb", "name": "ZakazkaTb", "autoWidth": false },
            { "data": "vin", "name": "Vin", "autoWidth": true },
            { "data": "cws", "name": "Cws", "autoWidth": true },
            { "data": "cisloProtokolu", "name": "CisloProtokolu", "autoWidth": true },

            { "data": "ukoncena", "name": "Ukoncena", "autoWidth": true },
            { "data": "poznamka", "name": "Poznamka", "autoWidth": false },

            { "data": "vytvoril", "name": "Vytvoril", "autoWidth": true },

            { "data": "zmenil", "name": "Zmenil", "autoWidth": true },
            { "data": "zmenene", "name": "Zmenene", "autoWidth": true },

        ]

    });

    
    //Filtre su v headeri tabulky
    $('#datatableZakazky tfoot tr').appendTo('#datatableZakazky thead');

    // $('#datatableZakazky').DataTable({
    //    scrollY: "600px",
    //    scrollX: true,
    //    scrollCollapse: true,
    //    paging: false,
    //    fixedColumns: {
    //        heightMatch: 'none'
    //    }
    //});

});



