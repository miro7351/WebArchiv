

//!!!! Pred spustenim musia byt nahrate prislusne *.js subory, podla manualu !!!!!!
$(document).ready(function () {

    // Setup - add a text input to each footer cell; title je nazov stlpca z <tfoot> <tr> <td>...
    $('#datatableZakazky tfoot th').each(function () {
        var title = $(this).text();
        //console.log("title:" + title);
        if (title == "VIN") {
            $(this).html('<input type="text"  class="filter1" placeholder=" ' + title + '" style="width:180px" />');
        }
        else if (title == "Ukončená") {
            $(this).html('<input type="text" class="filter1" placeholder=" ' + "A/N" + '" style="width:70px" />');
        }
        else if (title == "CWS") {
            $(this).html('<input type="text" class="filter1" placeholder=" ' + "CWS" + '" style="width:120px" />');
        }
        else if (title == "Ćíslo protokolu") {
            $(this).html('<input type="text" class="filter1" placeholder=" ' + "Číslo prot." + '" style="width:120px" />');
        }
        else if (title == "Majiteľ") {
            $(this).html('<input type="text" class="filter1" placeholder=" ' + title + '" style="width:250px" />');
        }
        else if (title == "") {//stlpec pre link 'Vymazat' bude tu button na vymazanie udajov z fitrov
            ; /*$(this).html('<input type="Button"   value="Vymazať""  onclick="ClearFilter()"/>');*/
        }
        else {
            //console.log("title:" + title);
            $(this).html('<input type="text" class="filter1" placeholder=" ' + title + '" style="width:120px" />');
        }
    });


    $('#datatableZakazky').dataTable({

        //"dom": '<"top"i>rt<"bottom"flp><"clear">', //OK
        "dom": '<"top"if>rt<"bottom"lp><"clear">', //OK
        "lengthMenu": [[25, 10, 30, 50, -1], [25, 10, 30, 50, "Všetky"]],
        //"lengthMenu": [25],  //OK je tam len jedna moznost na vyber
        "search": { return: true }, //Search box nad tabulkou  hlada az po stlaceni Enter
        //"fixedColumns": true,  //Freezes the left most column to the side of the table
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.18/i18n/Slovak.json"
        },


        initComplete: function () {
            // Apply the search, podla filtra pre stlpec sa hlada po stlaceni Enter
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
        "filter": true,     // this is for disable filter (search box), ak je false nefunguju ani filtre nad stlpcami!!!!
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
                    return '<a  href="../ZakazkyJQ/Details/?zakazkaTg=' + $.trim(row['zakazkaTg']) + '">' + data + '</a>';
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
                "targets": [7],//CisloDielu
                "visible": true,
                "searchable": true
            },

            {
                "targets": [8],/*SPZ */
                "visible": true,
                "searchable": false

            },
            {
                "targets": [9],/*Majitel */
                "visible": true,
                "searchable": false

            },
            {
                "targets": [10],/*Faktura cislo */
                "visible": true,
                "searchable": false

            },

            {
                "targets": [11],//Ukoncena
                "visible": true,
                "searchable": true

            },
            {
                "targets": [12],/*Poznamka */
                "visible": false,
                "searchable": false

            },
            {
                "targets": [13],/*Vytvoril */
                "visible": true,
                "searchable": false

            },
            {
                "targets": [14],/*Zmenil */
                "visible": true,
                "searchable": false

            },
            {
                "targets": [15],//Zmenene
                "visible": true,
                "searchable": false,
                render: $.fn.dataTable.render.moment('YYYY-MM-DDTHH:mm:ss', 'DD.MM.YYYY HH:mm'),
            },

            {
                "targets": [16],//Vymazat
                "searchable": false,
                "width": 90,
                "render": function (data, type, row) {
                    return '<a href="../ZakazkyJQ/Delete/' + $.trim(row['zakazkaId']) + '">' + 'Vymazať</a>';
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
            { "data": "cisloDielu", "name": "CisloDielu", "autoWidth": true },


            { "data": "spz", "name": "SPZ", "autoWidth": true },
            { "data": "majitel", "name": "Majitel", "autoWidth": true },
            { "data": "cisloFaktury", "name": "CisloFaktury", "autoWidth": true },
            { "data": "ukoncena", "name": "Ukoncena", "autoWidth": true },
            { "data": "poznamka", "name": "Poznamka", "autoWidth": false },

            { "data": "vytvoril", "name": "Vytvoril", "autoWidth": true },
            { "data": "zmenil", "name": "Zmenil", "autoWidth": true },
            { "data": "zmenene", "name": "Zmenene", "autoWidth": true },

        ],
        "createdRow": function (row, data, index) {
            // row = tr node
            // data = raw data (array or obj)
            // index = The index of the row in DataTables' internal storage.
            //console.log('createdRow:' + index);//tu pridem MH-02.05.2022
            //console.log('data.ukoncena=' + data.ukoncena);//data.Ukoncena-undefined;  data.ukoncena OK!!!!!!!!!!!!!!

            if (data.ukoncena == 'A') {
                console.log('1mh-createdRow: data.ukoncena == "N"');
                //$('td', row).css("background-color", '"#21CE2A');
                $(row).css("background-color", "#C4FACF");//OK!!!! po tyzdni...konecne!!!!!
            }
        },

    });
    //Filtre su v headeri tabulky
    $('#datatableZakazky tfoot tr').appendTo('#datatableZakazky thead');
});



