$(document).ready(function () {

    //po zobrazeni stranky
    $('#datatableAccounts').dataTable({


        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.18/i18n/Slovak.json"
        },
        
        "processing": true, // for show progress bar
        "serverSide": true, // for process server side
        "filter": false, // this is for disable filter (search box)
        "orderMulti": false, // for disable multiple column at once
        "ajax": {
            "url": "/Accounts/LoadData",
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs":
            [{
                "targets": [0],//LoginID
                "visible": false,
                "searchable": false
            },
            {
                "targets": [4],//Aktivny  true/false -> Ano/NIe
                "visible": true,
                "searchable": false,
                "data":"Aktivny",
                "render": function (Aktivny) {
                    if (Aktivny)
                        return "Áno";
                    return "Nie";

                }
            },
            {
                "targets": [7],//link Zmenit
                "render": function (data, type, row) {
                    return '<a  href="/Accounts/Edit/' + $.trim(row['loginId']) + '">Zmeniť</a>';
                    
                }
            },
            
            ],
        "columns": [ //poradie stlpcov musi byt take, ako su zoradene stlpce v Index.cshtml
            { "data": "loginId", "name": "LoginId", "autoWidth": true },
            { "data": "loginName", "name": "LoginName", "autoWidth": true },
            { "data": "loginPassword", "name": "LoginPassword", "autoWidth": true },
            { "data": "loginRola", "name": "LoginRola", "autoWidth": true },
            { "data": "aktivny", "name": "Aktivny", "autoWidth": true },
            { "data": "dbLogin", "name": "DbLogin", "autoWidth": true },
            { "data": "dbPassword", "name": "DbPassword", "autoWidth": true },
           
        ]

    });
});