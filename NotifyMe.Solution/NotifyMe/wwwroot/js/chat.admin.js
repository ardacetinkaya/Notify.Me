$(document).ready(function () {
    tinymce.init({
        selector: 'textarea',
        plugins:["table"]
        // plugins: [
        //     "advcode advlist anchor autolink codesample colorpicker contextmenu image imagetools",
        //     " lists link linkchecker media mediaembed noneditable powerpaste preview",
        //     " table template textcolor wordcount"
        // ]
    });
});


$('#btnhostmessage').on('click', function (event) {
    var user = 'Arda';
    var messageText = $("#txtmessage").val();
    messageText += tinyMCE.activeEditor.getContent();
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

