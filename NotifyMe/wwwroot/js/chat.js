"use strict";
var connection = new signalR.HubConnection("/Ntfctn",
    {
        logger: signalR.LogLevel.Information
    });




connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    li.classList.add("left");
    li.classList.add("clearfix");
    li.innerHTML = message;
    document.getElementById("messagesList").appendChild(li);
});

document.getElementById("btnsendmessage").addEventListener("click", function (event) {
    var user = document.getElementById("txtuser").value;
    var message = document.getElementById("txtmessage").value;
    document.getElementById("txtmessage").value = '';
    var elem = document.getElementById('chatcontent');
    elem.scrollTop += 1000;
    connection.invoke("SendPrivateMessage", user, message).catch(function (err) {
        return console.error;
    });
    event.preventDefault();
});

connection.on("GiveName", function (name) {
    var userName = document.getElementById("txtuser");
    userName.value = name;
});

//connection.on("ReceiveNotification", function (message) {
//    var li = document.createElement("li");
//    li.classList.add("left");
//    li.classList.add("clearfix");
//    li.innerHTML = message;
//    document.getElementById("generalnotification").appendChild(li);
//});
var notification = document.getElementById("btnsendnotification");

if (notification != null) {

    document.getElementById("btnsendnotification").addEventListener("click", function (event) {

        var message = document.getElementById("txtnotification").value;
        document.getElementById("txtnotification").value = '';
        var elem = document.getElementById('generalnotification');
        elem.scrollTop += 1000;
        connection.invoke("SendNotification", message).catch(function (err) {
            return console.error;
        });
        event.preventDefault();
    });
}

document.getElementById("txtmessage").addEventListener("keyup", function (event) {

    event.preventDefault();

    if (event.keyCode === 13) {
        document.getElementById("btnsendmessage").click();
    }
});

connection.start().catch(function (err) {
    return console.error;
});