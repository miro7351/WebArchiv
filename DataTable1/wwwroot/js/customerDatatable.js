


/* spusti sa po natiahnuti stranky, pozri DemoGridController: Index.cshtml 
 Italian.json
    Slovak.json OK:  Edge, Google Chrome, Firefox
     return "<a href='#'  onclick=DeleteData1('" + row['customerId'] + "'); >Vymazať</a>";
 */
$(document).ready(function () {
    $('#tableCustomer').dataTable({

        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.18/i18n/Slovak.json"
        },
        "processing": true, // for show progress bar
        "serverSide": true, // for process server side
        "filter": true, // this is for disable filter (search box)
        "orderMulti": false, // for disable multiple column at once
        "ajax": {
            "url": "/DemoGrid/LoadData",
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

                        return '<a  href="/DemoGrid/Edit/' + row['customerId'] + '">' + data + '</a>';
                    },
                    "targets": [1]
                },
                {
                    "render": function (data, type, row) {

                        return '<a href="/DemoGrid/Delete/' + $.trim(row['customerId']) + '">' + 'Vymazat(' + row['customerId'] + ') </a>'
                    },
                    "targets": [6]
                }
               
            ],
        "columns": [
            { "data": "customerId", "name": "CustomerID", "autoWidth": true },
            { "data": "name", "name": "Name", "autoWidth": true },
            { "data": "address", "name": "Address", "autoWidth": true },
            { "data": "country", "name": "Country", "autoWidth": true },
            { "data": "city", "name": "City", "autoWidth": true },
            { "data": "phoneNo", "name": "PhoneNo", "autoWidth": true },
            
        ]

    });
});

function ReadOnlyData() {
    $('#tableCustomer').dataTable({

        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.18/i18n/Slovak.json"
        },
        "processing": true, // for show progress bar
        "serverSide": true, // for process server side
        "filter": true, // this is for disable filter (search box)
        "orderMulti": false, // for disable multiple column at once
        "ajax": {
            "url": "/DemoGrid/LoadData",
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs":
            [{
                "targets": [0],
                "visible": false,
                "searchable": false
            }],
        "columns": [
            { "data": "customerId", "name": "CustomerID", "autoWidth": true },
            { "data": "name", "name": "Name", "autoWidth": true },
            { "data": "address", "name": "Address", "autoWidth": true },
            { "data": "country", "name": "Country", "autoWidth": true },
            { "data": "city", "name": "City", "autoWidth": true },
            { "data": "phoneNo", "name": "PhoneNo", "autoWidth": true },
        ]

    });
};

/* It is important to use camelCasing while defining the names of the variables. firstName will work. But FirstName won’t.
 * Quite weird, but that’s how js works.
 * Make sure you follow camelCasing standard while working with js scripts. 
 * 
 * var url = '@Url.Content("~/")' + "DemoGrid/Delete";
 * 
 * return "<a href='#'  onclick=DeleteData1('" + row['customerId'] + "'); >Vymazať</a>";
 *   return '<a href="/ZakazkyJQ/Delete/' + $.trim(row['zakazkaId']) + '">' + 'Vymazat(' + row['zakazkaId'] + ') </a>'
 * 
 * */

function DeleteData1(CustomerID) {
    if (confirm("Naozaj vymazať záznam: " + CustomerID + "?")) {
        Delete(CustomerID);
    }
    else {
        return false;
    }
}


function DeleteData(CustomerID) {
    if (confirm("Naozaj vymazať záznam ...?")) {
        Delete(CustomerID);
    }
    else {
        return false;
    }
}

function Delete(CustomerID) {
    var url = '@Url.Content("~/DemoGrid/Delete")';

    $.post(url, { ID: CustomerID }, function (data) {
        if (data) {
            oTable = $('#tableCustomer').DataTable();
            oTable.draw();
        }
        else {
            alert("Niekde nastala chyba!");
        }
    });
}
