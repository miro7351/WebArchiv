
$(document).ready(function () {

    //var table = $('#datatableZakazky').DataTable();

    


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
                "targets": [1]
            },
            {
                "targets": [9],
                "visible": true,
                "searchable": false,
                render: $.fn.dataTable.render.moment('YYYY-MM-DDTHH:mm:ss', 'DD.MM.YYYY HH:MM')
                },

                {
                    "targets": [11],
                    "visible": true,
                    "searchable": false,
                    render: $.fn.dataTable.render.moment('YYYY-MM-DDTHH:mm:ss', 'DD.MM.YYYY HH:MM')
                },
            {
                "render": function (data, type, row) {
                    return '<a href="/ZakazkyJQ/Delete/' + $.trim(row['zakazkaId']) + '">' + 'Vymazat(' + row['zakazkaId'] + ') </a>'
                },
                "targets": [12]
            }

            ],
        "columns": [
            { "data": "zakazkaId", "name": "ZakazkaId", "autoWidth": true },
            { "data": "zakazkaTg", "name": "ZakazkaTg", "autoWidth": true },
            { "data": "zakazkaTb", "name": "ZakazkaTb", "autoWidth": true },
            { "data": "vin", "name": "Vin", "autoWidth": true },
            { "data": "cws", "name": "Cws", "autoWidth": true },
            { "data": "cisloProtokolu", "name": "CisloProtokolu", "autoWidth": true },

            { "data": "ukoncena", "name": "Ukoncena", "autoWidth": true },
            { "data": "poznamka", "name": "Poznamka", "autoWidth": true },

            { "data": "vytvoril", "name": "Vytvoril", "autoWidth": true },
            { "data": "vytvorene", "name": "Vytvorene", "autoWidth": true },
            { "data": "zmenil", "name": "Zmenil", "autoWidth": true },
            { "data": "zmenene", "name": "Zmenene", "autoWidth": true },

        ]

    });

    //$('#datatableZakazky tbody')
    //    .on('mouseenter', 'td', function () {
    //        var colIdx = table.cell(this).index().column;

    //        $(table.cells().nodes()).removeClass('highlight');
    //        $(table.column(colIdx).nodes()).addClass('highlight');
    //    });
});

/*  OK!!  return '<a  href="/ZakazkyJQ/Edit2/?zakazkaTg=' + $.trim(row['zakazkaTg']) + '">' + data + '</a>';
 *  
 *  return '<a href="/ZakazkyJQ/Delete/' + row['zakazkaId'] + '">' + 'Vymazat('+ row['zakazkaId'] +') </a>';
 *  return "<a href='#'  onclick=DeleteData1('" + row['zakazkaTg'] + "'); >Vymazať</a>";
 *  
 *   return '<a  href="/ZakazkyJQ/Edit2/zakazkaTg=' + $.trim(row['zakazkaTg']) + '">' + data + '</a>';
 *  
 *   //return '<a  href="/ZakazkyJQ/Edit/#zakazkaTg' + $.trim(row['zakazkaTg']) + '">' + data + '</a>';
 * It is important to use camelCasing while defining the names of the variables. firstName will work. But FirstName won’t.
 * Quite weird, but that’s how js works.
 * Make sure you follow camelCasing standard while working with js scripts. */

function DeleteData1(zakazkaId) {
    if (confirm("Naozaj vymazať záznam: " + zakazkaId + "?")) {
        Delete(zakazkaId);
    }
    else {
        return false;
    }
}


function DeleteData(zakazkaTg) {
    if (confirm("Naozaj vymazať záznam ...?")) {
        Delete(zakazkaTg);
    }
    else {
        return false;
    }
}

function Delete(zakazkaId) {
    //var url = '@Url.Content("~/")' + "ZakazkyJQ/Delete";

    var url = '@Url.Content("~/ZakazkyJQ/Delete")';

    $.post(url, { ID: zakazkaId }, function (data) {
        if (data) {
            oTable = $('#datatableZakazky').DataTable();
            oTable.draw();
        }
        else {
            alert("Niekde nastala chyba!");
        }
    });
}