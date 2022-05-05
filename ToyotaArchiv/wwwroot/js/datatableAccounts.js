$(document).ready(function () {

  

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
                "targets": [7],//link zmenit
                "render": function (data, type, row) {
                    return '<a  href="/Accounts/Details/' + $.trim(row['loginId']) + '">' + 'Zmeniť(' + $.trim(row['loginId']);  + ')</a>';
                    
                }
            },
            
            ],
        "columns": [ //poradie musi byt take ako su zoradene stlpce v Index.cshtml
            { "data": "loginId", "name": "LoginId", "autoWidth": true },
            { "data": "loginName", "name": "LoginName", "autoWidth": true },
            { "data": "loginPassword", "name": "LoginPassword", "autoWidth": true },
            { "data": "loginRola", "name": "LoginRola", "autoWidth": true },
            { "data": "aktivny", "name": "Aktivny", "autoWidth": true },
            { "data": "dblogin", "name": "DbLogin", "autoWidth": true },
            { "data": "dbPassword", "name": "DbPassword", "autoWidth": true },
           
        ]

    });
});