using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using NotifyMe.Data;
using NotifyMe.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotifyMe.Services
{
    public class Notify : Hub
    {
        private static readonly Random _random = new Random(50000);
        private static readonly object _syncLock = new object();
        private IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private NotifyDbContext _db;

        public Notify(IServiceProvider provider, IConfiguration configuration)
        {
            _serviceProvider = provider;
            _configuration = configuration;
            _db = (NotifyDbContext)_serviceProvider.GetService(typeof(NotifyDbContext));
        }


        public override async Task OnConnectedAsync()
        {
            var name = Context.User.Identity.Name;


            if (string.IsNullOrEmpty(name))
            {
                lock (_syncLock)
                {
                    var userId = _random.Next(0, 50000);
                    name = $"WebUser{userId.ToString()}";
                }
            }
            else
            {
                name = _configuration["HostUser:Name"];
                await Clients.All.SendAsync("SayHello", "I'm online");

            }
            string connectionId = Context.ConnectionId ?? Guid.NewGuid().ToString();


            var user = _db.Users.Where(u => u.UserName == name).FirstOrDefault();

            bool isNew = false;
            if (user == null)
            {
                isNew = true;
                user = new User() { UserName = name };
            }

            user.Connections = new List<Connection>();
            user.Connections.Add(new Connection() { ConnectionID = connectionId, Connected = true, ConnectionDate = DateTime.Now });

            if (isNew)
                _db.Users.Add(user);
            else
                _db.Users.Update(user);

            await _db.SaveChangesAsync();


            await Clients.Caller.SendAsync("GiveName", name);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {

            var connection = _db.Connections.Where(c => c.ConnectionID == Context.ConnectionId).FirstOrDefault();

          
            if (connection != null)
            {
                connection.Connected = false;
                connection.DisconnectionDate = DateTime.Now;
                _db.Connections.Update(connection);
                await _db.SaveChangesAsync();

            }
            
            await base.OnDisconnectedAsync(exception);
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
                receiver = _configuration["HostUser:Name"];



            var currentConnection = _db.Connections.Where(c => c.ConnectionID == Context.ConnectionId).FirstOrDefault();
            var receiverConnections = new List<string>();

            var receiverUser = _db.Users.Where(u => u.UserName == receiver).Select(u => u.Id).ToList();

            if (receiverUser != null)
            {
                receiverConnections.AddRange(_db.Connections.Where(c => receiverUser.Contains(c.UserId) && c.Connected)
                    .Select(s => s.ConnectionID).ToList());
            }
            receiverConnections.Add(currentConnection.ConnectionID);


            var readOnly = receiverConnections.AsReadOnly();
            var messageContainer = CreateMessage(user, message);
            await Clients.Clients(readOnly).SendAsync("ReceiveMessage", user, messageContainer);
        }

        public async Task SendNotification(string message)
        {
            var messageContainer = CreateMessage("You", message);
            await Clients.All.SendAsync("ReceiveNotification", messageContainer);
        }


        private string CreateMessage(string user, string message)
        {

            var image = "http://placehold.it/50/FA6F57/fff&text=WU";//Some custom image for WebUser


            if (user == _configuration["HostUser:Name"])
            {
                image = _configuration["HostUser:Image"];
            }

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
