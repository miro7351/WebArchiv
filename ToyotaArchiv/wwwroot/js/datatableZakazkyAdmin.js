

//!!!! Pred spustenim musia byt nahrate prislusne *.js subory, podla manualu !!!!!!
$(document).ready(function () {

    // Setup - add a text input to each footer cell
    $('#datatableZakazky tfoot th').each(function () {
        var title = $(this).text();
        //console.log("title:" + title);
        if (title == "VIN") {
            $(this).html('<input type="text"  class="filter1" placeholder=" ' + title + '" style="width:180px" />');
        }
        else if (title == "Ukoncena") {
            $(this).html('<input type="text" class="filter1" placeholder=" ' + "A/N" + '" style="width:70px" />');
        }
        else if (title == "CWS") {
            $(this).html('<input type="text" class="filter1" placeholder=" ' + "CWS" + '" style="width:120px" />');
        }
        else if (title == "CisloProtokolu") {
            $(this).html('<input type="text" class="filter1" placeholder=" ' + "Číslo prot." + '" style="width:120px" />');
        }
        else if (title == "Majitel") {
            $(this).html('<input type="text" class="filter1" placeholder=" ' + "Majiteľ" + '" style="width:250px" />');
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

        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.18/i18n/Slovak.json"
        },
        "createdRow": function (row, data, index) {
            console.log("createdRow");//tu pridem MH-02.05.2022
            console.log("createdRow row['Ukoncena']:" +row['Ukoncena']);
            console.log("createdRow $(row)['Ukoncena']:" + $(row)['Ukoncena']);//createdRow row['ukoncena']:undefined
            //$('td', row).css("background-color", "#21CE2A");//OK vsetly riadky su svetlo zelene
            //$(row).css("background-color", "#21CE2A");//OK vsetly riadky su svetlo zelene
            //var rowDataLength = data.length;  //undefined
            //alert("rowData.Length" + rowDataLength);

            //var rowData7 = $(row).eq(7);  //row.eq(7)=[object Object]
            //if (  $(row).eq(7) == "N" )
            //  console.log("row.eq(7)=N");

            //var rowData = $(row.eq(6).innerHTML);//.data();

            //console.log("data[0]=" + data[0]);//data[0] undefined PRECO????, ale v Datatable02 mi to ide; tu este nie su data???
            //console.log("data[7]=" + data[7]);//data[7] undefined

            //if (data[7] == "N") {
            //    console.log('createdRow: data[7] == "N"');
            //    $('td', row).css("background-color", 'red');
            //    //alert("data[6] = N");
            //}
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
                "targets": [8],//Ukoncena
                "visible": true,
                "searchable": true,
                "createdCell": function (td, cellData, rowData, row, col) { //createdRow sa spusta az po createdCell!!!!
                    console.log("targets[7] cellData=" + cellData);
                    console.log("targets[8] $(row)['ukoncena']:" + $(row)['ukoncena']);//targets[8] row['ukoncena']:undefined
                    if (cellData == "A") {
                        console.log("targets[7] cellData='A' - zmena farieb");
                        //$('td', row).css("background-color", 'red');//NEJDE
                        //$(row).css("background-color", 'red');//NEJDE
                        //$(td).css('color', 'blue');  //OK
                        //$(td).css('background-color', 'yellow');//OK
                    }
                }

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
                "targets": [9],/*SPZ */
                "visible": true,
                "searchable": false

            },
            {
                "targets": [10],/*Majitel */
                "visible": true,
                "searchable": false

            },

            {
                "targets": [11],/*Poznamka */
                "visible": false,
                "searchable": false

            },
            {
                "targets": [12],/*Vytvoril */
                "visible": true,
                "searchable": false

            },
            {
                "targets": [13],/*Zmenil */
                "visible": true,
                "searchable": false

            },
            {
                "targets": [14],//Zmenene
                "visible": true,
                "searchable": false,
                render: $.fn.dataTable.render.moment('YYYY-MM-DDTHH:mm:ss', 'DD.MM.YYYY HH:mm'),
            },

            {
                "targets": [15],//Vymazat
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

            { "data": "ukoncena", "name": "Ukoncena", "autoWidth": true },
            { "data": "spz", "name": "SPZ", "autoWidth": true },
            { "data": "majitel", "name": "Majitel", "autoWidth": true },
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



