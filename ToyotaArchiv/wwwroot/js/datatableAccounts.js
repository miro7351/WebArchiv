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

            
            ],
        "columns": [
            { "data": "loginId", "name": "LoginId", "autoWidth": true },
            { "data": "loginName", "name": "LoginName", "autoWidth": true },
            { "data": "loginPassword", "name": "LoginPassword", "autoWidth": true },
            { "data": "loginRola", "name": "LoginRola", "autoWidth": true },
        ]

    });
});