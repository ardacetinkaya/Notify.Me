"use strict";
var connection = new signalR.HubConnection("/Ntfctn", {
    logger: signalR.LogLevel.Information
});

connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    li.classList.add("left");
    li.classList.add("clearfix");
    li.innerHTML = message;
    document.getElementById("messagesList").appendChild(li);
});

connection.on("GiveName", function (name) {
    if (document.getElementById("txtuser") !== null) {
        var userName = document.getElementById("txtuser");
        userName.value = name;
    }
});

connection.on("SayHello", function (name) {
    var sayHello = document.getElementById("onlinehost");
    if (sayHello !== null) {
        sayHello.innerHTML = name;
    }
});

connection.on("ReceiveNotification", function (message) {
    var notification = document.getElementById("btnnotification");
    if (notification !== null) {
        document.getElementById("notificationdata").innerHTML = message;
        document.getElementById("btnnotification").click();
    }

});
$(document).ready(function () {
    $('#messagesList').on('click','li',function(){
        document.getElementById("txtmessage").value = '@'+$(this).find("strong").html()+': ';
    })
});

var notificationAction = document.getElementById("btnsendnotification");
if (notificationAction !== null) {
    notificationAction.addEventListener("click", function (event) {
        var title = document.getElementById("txttitle").value;
        var link = document.getElementById("txtlink").value;
        var message = document.getElementById("txtnotification").value;
        document.getElementById("txtnotification").value = '';
        document.getElementById("txtlink").value = '';
        document.getElementById("txttitle").value = ''
        var notification = {
            title: title,
            link: link,
            message: message
        }
        var elem = document.getElementById('generalnotification');
        elem.scrollTop += 1000;
        var li = document.createElement("li");
        li.classList.add("left");
        li.classList.add("clearfix");
        li.innerHTML = message;
        elem.appendChild(li);
        connection.invoke("SendNotification", notification).catch(function (err) {
            return console.error;
        });
        event.preventDefault();
    });
}

var messageAction = document.getElementById("btnsendmessage");
if (messageAction !== null) {
    messageAction.addEventListener("click", function (event) {
        var user = document.getElementById("txtuser").value;
        var messageText = document.getElementById("txtmessage").value;
        document.getElementById("txtmessage").value = '';
        var elem = document.getElementById('chatcontent');
        elem.scrollTop += 1000;
        var privateMessage = {
            username: user,
            message: messageText
        }

        connection.invoke("SendPrivateMessage", privateMessage).catch(function (err) {
            return console.error;
        });
        event.preventDefault();
    });
}

var messageInput = document.getElementById("txtmessage");
if (messageInput !== null) {
    messageInput.addEventListener("keyup", function (event) {
        event.preventDefault();
        if (event.keyCode === 13) {
            document.getElementById("btnsendmessage").click();
        }
    });
}
connection.start().catch(function (err) {
    return console.error;
});