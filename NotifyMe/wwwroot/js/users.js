var dataTable = null;
$(document).ready(function () {

    BindUsers();

});

function BindUsers() {
    dataTable = $('#usersdata').DataTable({
        "processing": true,
        "serverSide": true,
        "searching": false,
        "ordering": false,
        "info": false,
        "pageLength": 8,
        "lengthChange": false,
        "ajax": {
            url: "/Users?handler=Users",
            type: "GET"
        },
        "columns": [{
                data: 'user.userName',
                render: function (data, type, row) {
                    return '<span class="glyphicon glyphicon-user"></span> '+data;
                }
            },
            {
                data: 'connectionDate',
                render: function (data, type, row) {
                    return dateFormat(data, "dddd, mmmm dS, yyyy @ HH:MM:ss",true);
                }
            },
            {
                data: 'disconnectionDate',
                render: function (data, type, row) {
                    return dateFormat(data, "dddd, mmmm dS, yyyy @ HH:MM:ss",true);
                }
            },
            {
                data: 'connected',
                render: function (data, type, row) {
                    return data?'<span class="glyphicon glyphicon-ok-circle" style="color:green"></span>':'<span class="glyphicon glyphicon-remove-circle"></span>';
                }
                
            },
            {
                data: 'userAgent'
            }
        ]
    });
    return dataTable;
}