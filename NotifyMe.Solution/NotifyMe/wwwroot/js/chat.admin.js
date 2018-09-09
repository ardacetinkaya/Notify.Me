$(document).ready(function () {
    $('#messagesList').on('click', 'li', function () {
        $("#txtmessage").val('@' + $(this).find("strong").html() + ': ');
        $("#txtmessage").focus();
    });
});