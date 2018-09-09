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
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly NotifyDbContext _db;
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
            var isHostConnected = false;
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
                await Clients.All.SendAsync("SayHello", "online");
                isHostConnected = true;

            }
            _visitor.LetInVisitor(Context.ConnectionId ?? Guid.NewGuid().ToString(), name, fromUrl);

            _logger.LogInformation($"{name} is connected.");

            await Clients.Caller.SendAsync("GiveName", name);
            if (!isHostConnected)
            {
                await SendWelcomeMessage(new ChatMessage()
                {
                    Message = "How can I help you?",
                    Username = _configuration["HostUser:Name"]
                });
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _visitor.LetOutVisitor(Context.ConnectionId);
            _logger.LogInformation($"User is disconnected. Detail: {exception.Message}");
            await base.OnDisconnectedAsync(exception);
        }
        public async Task SendWelcomeMessage(ChatMessage message)
        {
            var messageContainer = CreateMessage(MessageType.Chat,message.Username,message.Message);
            await Clients.Caller.SendAsync("ReceiveMessage", message.Username, messageContainer);
        }
        public async Task SendPrivateMessage(ChatMessage message)
        {
            if (string.IsNullOrEmpty(message.Message)) return;

            var receiver = string.Empty;
            message.Date = DateTimeOffset.Now;
            if (message.Message.StartsWith('@'))
            {
                var index = message.Message.IndexOf(":");
                receiver = message.Message.Substring(0, index + 1).TrimStart('@').TrimEnd(':');
                message.Message = message.Message.Substring(index + 1);
            }
            else
            {
                receiver = _configuration["HostUser:Name"];
            }

            var currentConnection = _db.Connections.Where(c => c.ConnectionID == Context.ConnectionId).FirstOrDefault();
            var receiverConnections = new List<string>();

            var receiverUser = _db.Users.Where(u => u.UserName == receiver).Select(u => u.Id).ToList();

            if (receiverUser != null)
            {
                receiverConnections.AddRange(_db.Connections.Where(c => receiverUser.Contains(c.UserId) && c.Connected)
                    .Select(s => s.ConnectionID).ToList());
            }
            receiverConnections.Add(currentConnection.ConnectionID);

            
            var messageContainer = CreateMessage(MessageType.Chat, message.Username, message.Message, receiver);

            if (!string.IsNullOrEmpty(receiver))
            {
                var result = _message.SaveMessage(Context.ConnectionId, new Message()
                {
                    Content = message.Message,
                    RawContent = JsonConvert.SerializeObject(message),
                    ToUser = receiver,
                    FromUser = message.Username,
                    Date = message.Date.DateTime,
                    Type = MessageType.Chat.ToString()
                });
                if (result)
                {
                    var readOnly = receiverConnections.AsReadOnly();
                    await Clients.Clients(readOnly).SendAsync("ReceiveMessage", message.Username, messageContainer);
                }
            }
        }

        [Authorize]
        public async Task SendNotification(NotificationMessage message)
        {
            if (message != null)
            {
                message.Date = DateTimeOffset.Now;
                var messageContainer = CreateMessage(MessageType.Notification, _configuration["HostUser:Name"], message.Message);

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
                if (_message.SaveMessage(Context.ConnectionId, notificationMessage))
                {
                    await Clients.All.SendAsync("ReceiveNotification", messageContainer);
                    _logger.LogInformation($"Notification message {message.Title} is sent.");
                }
            }
        }



        private string CreateMessage(MessageType type, string from, string message, string to = "", string title = "", string link = "")
        {
            var messageContainer = string.Empty;
            switch (type)
            {
                case MessageType.Chat:
                    var image = "http://placehold.it/50/FA6F57/fff&text=WU";//Some custom image for WebUser

                    if (from == _configuration["HostUser:Name"])
                    {
                        image = _configuration["HostUser:Image"];
                        if(!string.IsNullOrEmpty(to))
                            from = $"{from} -> {to}";
                    }
                    messageContainer = _message.GetMessageTemplate("Base Chat", message, from, image);
                    break;

                case MessageType.Notification:
                    messageContainer = _message.GetMessageTemplate("Base Notification", message, from, "");
                    
                    var messageLink = string.Empty;
                    if (!string.IsNullOrEmpty(link))
                    {
                        messageLink = $"<a type='button' class='btn btn-primary' hrep='{link}'>Go<i class='fa fa-diamond ml-1'></i></a>";
                    }
                    messageContainer = string.Format(messageContainer,
                        title,
                        message,
                        messageLink);
                    break;
                default:
                    break;
            }
            return messageContainer;

        }
    }

}
