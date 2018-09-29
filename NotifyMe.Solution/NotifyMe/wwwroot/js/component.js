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
                        <div class="panel-collapse collapse in" id="collapseOne">
                            <div class="panel-body" id="chatcontent" style="height:250px;overflow-y:scroll">
                                <ul id="messagesList" class="chat" v-for="item in items" >
                                    <li class="left clearfix" v-html="item"></li>
                                </ul>
                            </div>
                            <div class="panel-footer">
                                <div>
                                    <input class="form-control input-sm" type="text" value="visitor" id="txtuser" placeholder='Type your nick here...' />
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
    props: ["accessKey"],
    data: function () {
        return {
            connecition: null,
            access: this.accessKey,
            licensed: false,
            items: []
        };
    },
    created: function () {
        //TODO: Check this with server side license check.
        if (this.accessKey != '5DKZeomcebBrIQAKKXd79n4njerHaLRK') {
            console.error("Invalid key for component.");
            return;
        } else {
            this.licensed = true;
            console.log("Component is created.Please wait for connection.");
            console.log("Connecting...");

            this.connection = new signalR.HubConnectionBuilder()
                .withUrl("/Ntfctn?key=" + this.accessKey)
                .configureLogging(signalR.LogLevel.Information)
                .build();

            this.connection.serverTimeoutInMilliseconds = 300000;
        }

    },
    mounted: function () {
        var self = this;
        if (this.licensed) {
            self.connection.onclose(function () {
                console.log('Connecition is closed');
            });
            self.connection.on("GiveName", function (name) {
                $("#txtuser").val(name)

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

            self.connection.start().catch(err => console.error(err));
            console.log("Component is connected");
        }
    },
    methods: {
        sendMessage: function () {
            try {
                var self = this;

                var user = $("#txtuser").val();
                var messageText = $("#txtmessage").val();
                $("#txtmessage").val('');
                var elem = $("chatcontent");
                elem.scrollTop += 1000;
                var privateMessage = {
                    username: user,
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