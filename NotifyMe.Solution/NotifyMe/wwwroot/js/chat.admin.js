$(document).ready(function () {
    $('#messagesList').on('click', 'li', function () {
        $("#txtmessage").val('@' + $(this).find("strong").html() + ': ');
        $("#txtmessage").focus();
    });

    tinymce.init({
        selector: 'textarea',
        plugins: [
            "advcode advlist anchor autolink codesample colorpicker contextmenu image imagetools",
            " lists link linkchecker media mediaembed noneditable powerpaste preview",
            " table template textcolor wordcount"
        ]
    });
});


$('#btnhostmessage').on('click', function (event) {
    $("#txtuser").val()
    var user = $("#txtuser").val();
    var messageText = $("#txtmessage").val();
    messageText += tinyMCE.activeEditor.getContent();
    $("#txtmessage").val('');
    var elem = document.getElementById('chatcontent');
    elem.scrollTop += 1000;
    var privateMessage = {
        username: user,
        message: messageText
    }
    if (messageText) {
        connection.invoke("SendPrivateMessage", privateMessage).catch(err => console.error(err));
    }
    event.preventDefault();
});