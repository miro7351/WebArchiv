$(document).ready(function () {

    //po zobrazeni stranky
    $('#datatableAccounts').dataTable({

        "dom": '<"top"if>rt<"bottom"lp><"clear">', //OK
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.18/i18n/Slovak.json"
        },

        "processing": true, // for show progress bar
        "serverSide": true, // for process server side
        "filter": false, // this is for disable filter (search box)
        "orderMulti": false, // for disable multiple column at once
        "ajax": {
            "url": "../Accounts/LoadData",
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs":
            [{
                "targets": [0],//LoginName
                "visible": true,
                "width" : 60,
                "searchable": true
            },
            {
                "targets": [1],//LoginPassword
                "visible": false,
                "searchable": false
            },
            {
                "targets": [2],//LoginRola
                "visible": true,
                "width" : 20,
                "searchable": true
            },

            {
                "targets": [3],//Aktivny  true/false -> Ano/NIe
                "visible": true,
                "searchable": false,
                "data": "Aktivny",
                "render": function (Aktivny) {
                    if (Aktivny)
                        return "Áno";
                    return "Nie";

                }
            },
            {
                "targets": [4],//Db login
                "visible": false,
                "searchable": false
            },
            {
                "targets": [5],//Db heslo
                "visible": false,
                "searchable": false
            },
            {
                "targets": [6],//link Zmenit
                "render": function (data, type, row) {
                    return '<a  href="../Accounts/Edit/?loginName=' + $.trim(row['loginName']) + '">Zmeniť</a>';

                }
            },

            ],
        "columns": [ //poradie stlpcov musi byt take, ako su zoradene stlpce v Index.cshtml

            { "data": "loginName", "name": "LoginName", "autoWidth": false },
            { "data": "loginPassword", "name": "LoginPassword", "autoWidth": true },
            { "data": "loginRola", "name": "LoginRola", "autoWidth": false },
            { "data": "aktivny", "name": "Aktivny", "autoWidth": false },
            { "data": "dbLogin", "name": "DbLogin", "autoWidth": true },
            { "data": "dbPassword", "name": "DbPassword", "autoWidth": true },

        ]

    });
});