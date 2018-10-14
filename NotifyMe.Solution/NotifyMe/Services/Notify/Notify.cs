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
using System.Threading;
using System.Threading.Tasks;

namespace NotifyMe.Services
{
    public class Notify : Hub
    {
        private static readonly Random _random = new Random(10);
        private readonly object _syncObject = new object();
        private const int _timeoutWait = 1000; // milliseconds
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
            try
            {
                var name = Context.User.Identity.Name;
                var response = Context.GetHttpContext().Response;
                var fromUrl = response.Headers["Access-Control-Allow-Origin"];
                var isHostConnected = false;
                var key = Context.GetHttpContext().Request.Query["key"];
                _logger.LogDebug($"Connecting with Access Key:{key}...");

                if (!_visitor.HasVisitorAccess(fromUrl, key))
                {
                    throw new HubException("Invalid access key for the origin. Please check your access key");
                }


                if (string.IsNullOrEmpty(name))
                {
                    //Monitor usage example
                    if (Monitor.TryEnter(_syncObject, _timeoutWait))
                    {
                        try
                        {
                            var userId = _random.Next(0, 10);
                            name = $"WebUser{userId.ToString()}";
                        }
                        finally
                        {
                            Monitor.Exit(_syncObject);
                        }
                    }
                    else
                    {
                        _logger.LogDebug("Can not assign name");
                    }
                }
                else
                {
                    name = _configuration["HostUser:Name"];
                    await Clients.All.SendAsync("SayHello", "online");
                    isHostConnected = true;

                }
                _visitor.LetInVisitor(Context.ConnectionId ?? Guid.NewGuid().ToString(), name, fromUrl);
                _logger.LogDebug($"{name} is connected.");

                var host = _db.Connections.Where(c => c.User.UserName == _configuration["HostUser:Name"] && c.Connected)
                                        .OrderByDescending(c => c.ConnectionDate)
                                        .FirstOrDefault();

                await Clients.Caller.SendAsync("GiveName", name);
                if (!isHostConnected)
                {
                    await SendWelcomeMessage(new ChatMessage()
                    {
                        Message = "How can I help you?",
                        Username = _configuration["HostUser:Name"],
                        FriendlyUsername = _configuration["HostUser:Name"],
                    });
                }
                
                if (host != null)
                {
                    _logger.LogDebug($"Host{ host.ConnectionID} is already connected. Informing host...");
                    await Clients.Client(host.ConnectionID).SendAsync("iamin", name);
                }
                await base.OnConnectedAsync();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"Connection failed: {ex.Message}");
                throw ex;
            }

        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                _visitor.LetOutVisitor(Context.ConnectionId);

                var user = _db.Connections.Where(c => Context.ConnectionId == c.ConnectionID && c.Connected)
                          .OrderByDescending(c => c.ConnectionDate)
                          .FirstOrDefault();

                var host = _db.Connections.Where(c => c.User.UserName == _configuration["HostUser:Name"] && c.Connected)
                                          .OrderByDescending(c => c.ConnectionDate)
                                          .FirstOrDefault();
                if (host != null && user != null)
                {
                    _logger.LogDebug($"{host.User.UserName}({host.ConnectionID}) is already connected.");
                    await Clients.Client(host.ConnectionID).SendAsync("iamout", user.User.UserName);
                    _logger.LogDebug($"{user.User.UserName} is disconnected. Detail: {exception?.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error is occured {ex.Message}");
            }
            finally
            {
                await base.OnDisconnectedAsync(exception);
            }
        }
        public async Task SendWelcomeMessage(ChatMessage message)
        {
            var messageContainer = CreateMessage(MessageType.Chat, message);
            await Clients.Caller.SendAsync("ReceiveMessage", message.Username,"", messageContainer);
        }
        public async Task SendPrivateMessage(ChatMessage message)
        {
            try
            {
                if (string.IsNullOrEmpty(message.Message)) return;

                var receiver = string.Empty;
                message.Date = DateTimeOffset.Now;
                if (message.Message.StartsWith('#'))
                {
                    var index = message.Message.IndexOf(":");
                    receiver = message.Message.Substring(0, index + 1).TrimStart('#').TrimEnd(':');
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

                var messageContainer = CreateMessage(MessageType.Chat, message, receiver);

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
                        await Clients.Clients(readOnly).SendAsync("ReceiveMessage", message.Username,receiver, messageContainer);
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"Can not sent message: {ex.Message}");
            }
        }

        [Authorize]
        public async Task SendNotification(NotificationMessage message)
        {
            if (message != null)
            {
                message.Date = DateTimeOffset.Now;
                var messageContainer = CreateMessage(MessageType.Notification, message);

                var notificationMessage = new Message()
                {
                    Content = message.Message,
                    RawContent = JsonConvert.SerializeObject(message),
                    ToUser = "Everyone",
                    FromUser = _configuration["HostUser:Name"],
                    Date = message.Date.DateTime,
                    Type = MessageType.Notification.ToString()

                };
                if (_message.SaveMessage(Context.ConnectionId, notificationMessage))
                {
                    await Clients.All.SendAsync("ReceiveNotification", messageContainer);
                    _logger.LogInformation($"Notification message {message.Title} is sent.");
                }
            }
        }

        private string CreateMessage(MessageType type, BaseMessage message, string to = "")
        {

            var messageContainer = string.Empty;
            switch (type)
            {
                case MessageType.Chat:


                    var image = "http://placehold.it/50/FA6F57/fff&text=WU";//Some custom image for WebUser
                    var chatMessage = message as ChatMessage;

                    if (chatMessage.Username == _configuration["HostUser:Name"])
                    {
                        image = _configuration["HostUser:Image"];
                        if (!string.IsNullOrEmpty(to))
                            chatMessage.FriendlyUsername = $"{chatMessage.Username} -> {to}";

                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(chatMessage.FriendlyUsername))
                            image = $"http://placehold.it/50/FA6F57/fff&text={chatMessage.FriendlyUsername.Substring(0, 1)}";
                        else
                            chatMessage.FriendlyUsername = chatMessage.Username;
                    }

                    messageContainer = _message.CreateMessage("Base Chat", chatMessage.Message, chatMessage.Username, chatMessage.FriendlyUsername, image);
                    break;

                case MessageType.Notification:
                    var notificationMessage = message as NotificationMessage;
                    messageContainer = _message.CreateMessage("Base Notification", notificationMessage.Message, notificationMessage.Username, string.Empty, string.Empty);

                    var messageLink = string.Empty;
                    if (!string.IsNullOrEmpty(notificationMessage.Link))
                    {
                        messageLink = $"<a type='button' class='btn btn-primary' hrep='{notificationMessage.Link}'>Go<i class='fa fa-diamond ml-1'></i></a>";
                    }
                    messageContainer = string.Format(messageContainer,
                        notificationMessage.Title,
                        notificationMessage.Message,
                        messageLink);
                    break;
                default:
                    break;
            }
            return messageContainer;
        }

    }

}
