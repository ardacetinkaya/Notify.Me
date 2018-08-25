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
                    return data;
                }
            },
            {
                data: 'connectionDate',
                render: function (data, type, row) {
                    return data;
                }
            },
            {
                data: 'disconnectionDate',
                render: function (data, type, row) {
                    return data;
                }
            },
            {
                data: 'connected'
            },
            {
                data: 'userAgent'
            }
        ]
    });
    return dataTable;
}