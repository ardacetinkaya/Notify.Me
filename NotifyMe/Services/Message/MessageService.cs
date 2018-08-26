using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NotifyMe.Data;
using NotifyMe.Data.Models;

namespace NotifyMe.Services
{
    public class MessageService:IMessageService
    {
        private NotifyDbContext _db;
        private ILogger<MessageService> _logger;
        public MessageService(IServiceProvider provider, IConfiguration configuration, ILogger<MessageService> logger)
        {

            _logger = logger;
            _db = (NotifyDbContext)provider.GetService(typeof(NotifyDbContext));
        }
        public bool SaveMessage(string connectionId,Message message)
        {
            try
            {
                var currentConnection = _db.Connections.Where(c => c.ConnectionID == connectionId && c.Connected).FirstOrDefault();
                if (currentConnection != null)
                {
                    if (currentConnection.Messages == null) currentConnection.Messages = new List<Message>();

                    message.Date = DateTime.Now;
                    currentConnection.Messages.Add(message);

                    _db.Connections.Update(currentConnection);
                    _db.SaveChanges();
                    return true;
                }

            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return false;
        }
    }
}