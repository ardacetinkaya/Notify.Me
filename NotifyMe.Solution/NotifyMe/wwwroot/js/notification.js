var dataTable=null;
$(document).ready(function () {
    tinymce.init({
        selector: 'textarea',
        plugins: [
            "advlist anchor autolink codesample colorpicker contextmenu image imagetools",
            " lists link media noneditable preview",
            " table template textcolor wordcount"
          ]
    });
    BindNotifications();

});

function BindNotifications() {
    dataTable = $('#notificationdata').DataTable({
        "processing": true,
        "serverSide": true,
        "searching": false,
        "ordering": false,
        "info": false,
        "pageLength": 5,
        "lengthChange": false,
        "ajax": {
            url: "/Notifications?handler=AllNotifications",
            type: "GET"
        },
        "columns": [{
                data: 'content',
                render: function (data, type, row) {
                    return data;
                }
            },
            {
                data: 'date',
                render: function (data, type, row) {
                    return dateFormat(data, "dddd, mmmm dS, yyyy @ HH:MM:ss",true);;
                }
            }
        ]
    });
    return dataTable;
}

var notificationAction = document.getElementById("btnsendnotification");
if (notificationAction !== null && connection != null) {
    notificationAction.addEventListener("click", function (event) {
        var title = document.getElementById("txttitle").value;
        var link = document.getElementById("txtlink").value;
        var message = tinyMCE.activeEditor.getContent();
        var notification = {
            title: title,
            link: link,
            message: message
        }

        
        connection.invoke("SendNotification", notification).catch(err => console.error(err));

        event.preventDefault();
        dataTable.ajax.reload();
    });
}