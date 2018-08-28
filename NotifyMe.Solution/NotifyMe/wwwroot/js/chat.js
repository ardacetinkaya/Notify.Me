"use strict";
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/Ntfctn")
    .configureLogging(signalR.LogLevel.Information)
    .build();
connection.serverTimeoutInMilliseconds = 300000;

connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    li.classList.add("left");
    li.classList.add("clearfix");
    li.innerHTML = message;
    document.getElementById("messagesList").appendChild(li);
});

connection.on("GiveName", function (name) {
    $("#txtuser").val(name)
});

connection.on("SayHello", function (name) {
    $("#onlinehost").html(name);
});

connection.on("ReceiveNotification", function (message) {
    $("#notifypopup").html(message);
    $('#centralModalInfo').modal('show');
});
$(document).ready(function () {
    $('#messagesList').on('click', 'li', function () {
        $("#txtmessage").val('@' + $(this).find("strong").html() + ': ');
        $("#txtmessage").focus();
    });

    $( "#txtmessage" ).keyup(function( event ) {
    
    }).keydown(function( event ) {
      if ( event.which == 13 ) {
        event.preventDefault();
        document.getElementById("btnsendmessage").click();
      }
    });
});

var messageAction = document.getElementById("btnsendmessage");
if (messageAction !== null) {
    messageAction.addEventListener("click", function (event) {
        $("#txtuser").val()
        var user =  $("#txtuser").val();
        var messageText = $("#txtmessage").val();
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
}

connection.start().catch(err => console.error(err));