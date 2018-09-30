"use strict";

Vue.component('chat', {
    template: `<div id="content-wrap" style="padding:10px;">
                    <div class="panel panel-primary">
                        <div class="panel-heading" id="accordion">
                            <span class="glyphicon glyphicon-comment"></span> Chat
                            <div class="btn-group pull-right">
                                <span id="onlinehost"></span>
                            </div>
                        </div>
                        
                        <div class="panel-collapse collapse in chatparent" id="collapseOne">
                            <div class="chatlay">
                                <div class="content">
                                    <p class="radius"><img src="./images/chat-support.png" width="70px" style="display:inline" /></p>
                                    <input id="txtusername" type="text" v-on:keyup.enter="letsstart" maxlength="10" placeholder="What's your name?"></input><br />
                                    <button @click.prevent="letsstart" class="btn btn-info btn-sm" id="btnchatstart">Let's start...</button><br />
                                </div>
                            </div>
                            <div class="panel-body" id="chatcontent" style="height:250px;overflow-y:scroll">
                                <ul id="messagesList" class="chat" v-for="item in items" >
                                    <li class="left clearfix" v-html="item"></li>
                                </ul>
                            </div>
                            <div class="panel-footer">
                                <div>
                                    <input class="form-control input-sm" type="hidden" value="visitor" id="txtuser"/>
                                    <input class="form-control input-sm" type="hidden" value="" id="txtfriendlyname"/>
                                    <input id="txtmessage" v-on:keyup.enter="sendMessage" maxlength="250" type="text" class="form-control input-sm" placeholder="Type your message here..." />
                                    <span class="input-group-btn" style="text-align:right;padding-top:5px">
                                        <button @click.prevent="sendMessage"  class="btn btn-info btn-sm" id="btnsendmessage">
                                            Send
                                        </button>
                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>`,
    props: ["accessKey", "warning"],
    data: function () {
        return {
            connecition: null,
            access: this.accessKey,
            licensed: false,
            items: []
        };
    },
    created: function () {

        this.licensed = true;
        console.log("Component is created.Please wait for connection.");
        console.log("Connecting...");

        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/Ntfctn?key=" + this.accessKey)
            .configureLogging(signalR.LogLevel.Information)
            .build();

        this.connection.serverTimeoutInMilliseconds = 300000;


    },
    mounted: function () {
        var self = this;
        if (this.licensed) {
            self.connection.onclose(function () {
                $(".chatlay").show();
                console.log('Connecition is closed');
            });
            self.connection.on("GiveName", function (name) {
                $("#txtuser").val(name);
                $(".chatlay").hide();

            });
            self.connection.on("SayHello", function (name) {
                $("#onlinehost").html(name);
            });
            self.connection.on("ReceiveMessage", function (user, message) {
                try {
                    if (component.$refs.box) {
                        component.$refs.box.addMessage(message);
                    } else {
                        console.error("Invalid ref value for component. Please keep 'ref=\"box\"' attribute as default.");
                    }
                } catch (error) {
                    console.log(error.message);
                }

            });


        }
    },
    methods: {
        letsstart: function () {
            var self = this;
            var isconnected = false;
            self.connection.start().then(() => {
                console.log("Component is connected");
                var username = $("#txtusername").val();
                $("#txtfriendlyname").val(username);
            }).catch(err => {
                console.log(err);
                $(".chatlay").show();
            });
        },
        sendMessage: function () {
            try {
                var self = this;
                var user = $("#txtuser").val();
                var friendlyUname =$("#txtfriendlyname").val();
                console.log(friendlyUname);
                var messageText = $("#txtmessage").val();
                $("#txtmessage").val('');
                var elem = $("chatcontent");
                elem.scrollTop += 1000;
                var privateMessage = {
                    username: user,
                    friendlyusername:friendlyUname,
                    message: messageText
                }
                if (messageText) {
                    self.connection.invoke("SendPrivateMessage", privateMessage).catch(err => console.error(err));
                }

            } catch (error) {
                console.log(error.message);
            }

        },
        addMessage: function (test) {
            var self = this;
            self.items.push(test);
            console.log("Component's addMessage is fired.");
        }

    }
});