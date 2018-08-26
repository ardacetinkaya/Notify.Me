using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        private static readonly Random _random = new Random(10);
        private static readonly object _syncLock = new object();
        private IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private NotifyDbContext _db;

        private readonly ILogger<Notify> _logger;
        private readonly IVisitorService _visitor;
        private readonly IMessageService _message;

        public Notify(IServiceProvider provider, IConfiguration configuration, ILogger<Notify> logger)
        {
            _serviceProvider = provider;
            _configuration = configuration;
            _db = (NotifyDbContext)_serviceProvider.GetService(typeof(NotifyDbContext));
            _visitor = (IVisitorService)_serviceProvider.GetService(typeof(IVisitorService));
            _message = (IMessageService)_serviceProvider.GetService(typeof(IMessageService));
            _logger = logger;

        }

        public override async Task OnConnectedAsync()
        {
            var name = Context.User.Identity.Name;
            var response = Context.GetHttpContext().Response;
            var fromUrl = response.Headers["Access-Control-Allow-Origin"];

            if (string.IsNullOrEmpty(name))
            {
                lock (_syncLock)
                {
                    var userId = _random.Next(0, 10);
                    name = $"WebUser{userId.ToString()}";
                }
            }
            else
            {
                name = _configuration["HostUser:Name"];
                await Clients.All.SendAsync("SayHello", "I'm online");

            }
            _visitor.LetInVisitor(Context.ConnectionId ?? Guid.NewGuid().ToString(), name, fromUrl);

            _logger.LogInformation($"{name} is connected");
            await Clients.Caller.SendAsync("GiveName", name);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {

            _visitor.LetOutVisitor(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
        public async Task SendPrivateMessage(ChatMessage message)
        {
            if (string.IsNullOrEmpty(message.Message)) return;

            var receiver = string.Empty;
            if (message.Message.StartsWith('@'))
            {
                var index = message.Message.IndexOf(":");
                receiver = message.Message.Substring(0, index + 1).TrimStart('@').TrimEnd(':');
                message.Message = message.Message.Substring(index + 1);
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
            var messageContainer = CreateMessage(message.Username, message.Message, receiver);

            if (!string.IsNullOrEmpty(receiver))
            {
                var result = _message.SaveMessage(Context.ConnectionId,new Message()
                {
                    Content = message.Message,
                    RawContent = JsonConvert.SerializeObject(message),
                    ToUser = receiver,
                    FromUser = message.Username,
                    Date = DateTime.Now,
                    Type = MessageType.Chat.ToString()
                });
                if (result)
                    await Clients.Clients(readOnly).SendAsync("ReceiveMessage", message.Username, messageContainer);
            }
        }

        [Authorize]
        public async Task SendNotification(NotificationMessage message)
        {
            if (message != null)
            {
                message.Date = DateTimeOffset.Now;
                var messageContainer = @"<div class='modal fade' id='centralModalInfo' tabindex='-1' role='dialog' aria-labelledby='myModalLabel' aria-hidden='true'>
<div class='modal-dialog modal-side modal-top-right' role='document'>
    <div class='modal-content'>
        <div class='modal-header'>
            <p class='heading lead'>{0}</p>

            <button type='button' class='close' data-dismiss='modal' aria-label='Close'>
                <span aria-hidden='true' class='white-text'>&times;</span>
            </button>
        </div>

        <div class='modal-body'>
            <div class='text-center' id='notificationcontent'>
                <i class='fa fa-check fa-4x mb-3 animated rotateIn'></i>
                <p>{1}</p>
            </div>
        </div>
        <div class='modal-footer justify-content-center'>
            {2}
            <a type='button' class='btn btn-outline-primary waves-effect' data-dismiss='modal'>Ok, thanks...</a>
        </div>
    </div>
</div>
</div>";

                var link = string.Empty;
                if (!string.IsNullOrEmpty(message.Link))
                {
                    link = $"<a type='button' class='btn btn-primary' hrep='{message.Link}'>Go<i class='fa fa-diamond ml-1'></i></a>";
                }
                messageContainer = string.Format(messageContainer,
                    message.Title,
                    message.Message,
                    link);
                var messageContent = JsonConvert.SerializeObject(message);
                var notificationMessage = new Message()
                {
                    Content = message.Message,
                    RawContent = messageContent,
                    ToUser = "Everyone",
                    FromUser = _configuration["HostUser:Name"],
                    Date = DateTime.Now,
                    Type = MessageType.Notification.ToString()

                };
                if (_message.SaveMessage(Context.ConnectionId,notificationMessage))
                {
                    await Clients.All.SendAsync("ReceiveNotification", messageContainer);
                    _logger.LogInformation($"Notification message {message.Title} is sent.");
                }
            }
        }

        

        private string CreateMessage(string from, string message, string to = "")
        {
            var image = "http://placehold.it/50/FA6F57/fff&text=WU";//Some custom image for WebUser

            if (from == _configuration["HostUser:Name"])
            {
                image = _configuration["HostUser:Image"];
                from = $"{from}->{to}";
            }
            var messageContainer = _message.GetMessageTemplate(message,from,image);

            return messageContainer;
        }
    }

}
