using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotifyMe.Services
{
    public class Notify : Hub
    {
        private static readonly Random _random = new Random(1234);

        private readonly static ConnectionMap<string> _connectedUsers = new ConnectionMap<string>();
        public override Task OnConnectedAsync()
        {
            var name = Context.User.Identity.Name;
            if (string.IsNullOrEmpty(name))
            {
                lock (_random)
                {
                    var userId = _random.Next(1000, 1234);
                    name = $"WebUser{userId.ToString()}";
                }
            }
            else
            {
                name = "You";//This is just for giving a static name, not needed.
            }

            _connectedUsers.Add(name, Context.ConnectionId ?? Guid.NewGuid().ToString());

            Clients.Caller.SendAsync("GiveName", name);
            return base.OnConnectedAsync();
        }


        public async Task SendMessage(string user, string message)
        {
            var messageContainer = CreateMessage(user, message);
            await Clients.All.SendAsync("ReceiveMessage", user, messageContainer);
        }

        public async Task SendPrivateMessage(string user, string message)
        {
            var receiver = string.Empty;
            if (message.StartsWith('@'))
            {
                var index = message.IndexOf(":");
                receiver = message.Substring(0, index + 1).TrimStart('@').TrimEnd(':');
                message = message.Substring(index + 1);
            }
            else
                receiver = "You";


            var users = _connectedUsers.GetConnections(receiver);
            var self = _connectedUsers.GetConnections(user);
            var messageContainer = CreateMessage(user, message);

            var receivers = users.ToList<string>();
            receivers.AddRange(self);

            var readOnly = receivers.AsReadOnly();
            await Clients.Clients(readOnly).SendAsync("ReceiveMessage", user, messageContainer);
        }

        public async Task SendNotification(string message)
        {
            var messageContainer = CreateMessage("You", message);
            await Clients.All.SendAsync("ReceiveNotification", messageContainer);
        }


        private string CreateMessage(string user, string message)
        {

            var image = "http://placehold.it/50/FA6F57/fff&text=WU";


            if (user != "You")
            {
                //var userId = 0;
                //lock (_random)
                //{
                //    userId = _random.Next(1000, 1234);
                //}
                //user = $"WebUser_{userId.ToString()}";
            }
            else
                image = "http://placehold.it/50/FA6F57/fff&text=You";


            var messageContainer = "<span class=\"chat-img pull-left\">"
                           + $"          <img src=\"{image}\" alt=\"User\" class=\"img-circle\" />"
                           + "     </span>"
                           + "     <div class=\"chat-body clearfix\">"
                           + "         <div class=\"header\">"
                           + $"             <small class=\"text-muted\"><span class=\"glyphicon glyphicon-time\"></span>{DateTime.Now.ToShortTimeString()}</small>"
                           + $"             <strong class=\"pull-right primary-font\">{user}</strong>"
                           + "        </div>"
                           + $"         <p>{message}"
                           + "         </p>"
                           + "     </div>";

            return messageContainer;
        }
    }

}
