using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NotifyMe.Common;
using NotifyMe.Data;
using NotifyMe.Data.Models;

namespace NotifyMe.Services
{
    public class MessageService : IMessageService
    {
        private NotifyDbContext _db;
        private readonly ILogger<MessageService> _logger;
        private readonly IHostingEnvironment _hosting;
        private readonly ITemplateService _templateService;

        public MessageService(IServiceProvider provider, IConfiguration configuration, IHostingEnvironment hosting, ILogger<MessageService> logger)
        {
            _logger = logger;
            _db = (NotifyDbContext)provider.GetService(typeof(NotifyDbContext));
            _templateService = (ITemplateService)provider.GetService(typeof(ITemplateService));
            _hosting = hosting;

        }

        public bool SaveMessage(string connectionId, Message message)
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

        public string CreateMessage(string templateName, string message, string from, string friendlyName, string image)
        {
            try
            {
                var template = _templateService.GetTemplate(templateName);
                if (template == null) return $"No template is found with given name:{templateName}";
                return template.Create(message, from, friendlyName, image, DateTimeOffset.Now, "");

            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Unable to find message templates. {ex.Message}");
            }

            return string.Empty;

        }
    }
}