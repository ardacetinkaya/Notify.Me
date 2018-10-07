"use strict";
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/Ntfctn?KEY=321")
    .configureLogging(signalR.LogLevel.Information)
    .build();
connection.serverTimeoutInMilliseconds = 300000;

connection.on("ReceiveMessage", function (user, to, message) {
    //TODO: Fix this silly thing
    if (to === 'Arda') to = user;
    $("div[role='tabpanel'][class*='active']").find("." + to).append("<li class='left clearfix'>" + message + "</li>");
});

connection.on("GiveName", function (name) {
    $("#txtuser").val(name);
});

connection.on("SayHello", function (name) {
    $("#onlinehost").html(name);
    $('#chatheader').removeClass();
    $('#chatheader').addClass("chat-header");
    $('#chatheader').addClass(name);
});

connection.on("ReceiveNotification", function (message) {
    $("#notifypopup").html(message);
    $('#centralModalInfo').modal('show');
});

connection.onclose(function () {
    alert("Please refresh connections...");
    $("#nav-tabs").find(".active").remove();
});

connection.on("iamin", function (name) {
    $("div[class*='active']").removeClass('active');
    $("li[class*='active']").removeClass('active');
    $("#txtmessage").val("#" + name + ":");
    $("#nav-tabs").append(`<li role="presentation" class="active" name="` + name + `">
                                <a href="#` + name + `" aria-controls="` + name + `" role="tab" data-toggle="tab">
                                    <img class="img-circle" src="http://placehold.it/50/FA6F57/fff&text=` + name + `" />
                                </a>
                            </li>`);

    $('#tabs-collapse').append(`<div role="tabpanel" class="tab-pane fade in active" id="` + name + `">
                                    <div class="tab-inner">
                                        <div class="panel panel-primary">
                                            <div class="panel-heading" >
                                                <div class="row">
                                                    <div class="col-md-5">
                                                        <span class="glyphicon glyphicon-comment"></span> Chat with ` + name + `
                                                    </div>
                                                    <div class="col-md-7">
                                                        <div style="margin:1px 1px" class="text-right">
                                                            <button class="btn  btn-xs btn-success arstatus" data-status="Online" id="btnonline">Online</button>
                                                            <button class="btn btn-xs btn-danger arstatus" data-status="Busy" id="btnbusy">Busy</button>
                                                            <button class="btn btn-xs btn-warning arstatus" data-status="Away" id="btnaway">Away</button>
                                                            <button class="btn btn-xs btn-default arstatus" data-status="Offline" id="btnoffline">Offline</button>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="panel-collapse collapse in">
                                                <div class="panel-body" id="chatcontent">
                                                    <ul class="chat admin ` + name + `">

                                                    </ul>
                                                </div>
                                                <div class="panel-footer">

                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <input type="hidden" value="` + name + `"  class="usernametext" />
                                </div>`);

    $("#nav-tabs li[class='active'] a").on("click", function () {
        $("#txtmessage").val($(this).attr("href") + ": ");
    });

});
connection.on("iamout", function (name) {
    console.log(name);
});
connection.start().catch(err => console.error(err));