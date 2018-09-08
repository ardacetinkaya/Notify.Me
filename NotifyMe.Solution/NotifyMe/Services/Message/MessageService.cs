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
        [ImportMany]
        private IEnumerable<IBaseTemplate> _templates { get; set; }
        public MessageService(IServiceProvider provider, IConfiguration configuration, IHostingEnvironment hosting, ILogger<MessageService> logger)
        {
            _logger = logger;
            _db = (NotifyDbContext)provider.GetService(typeof(NotifyDbContext));
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

        public string GetMessageTemplate(string templateName, string message, string from, string image)
        {
            try
            {
                _templates = null;
                var rootPath = _hosting.ContentRootPath;
                var path = Path.Combine(rootPath, "Plugins");
                _logger.LogInformation($"Path is: {path}");
                var assemblies = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories)
                            .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath)
                            .ToList();
                _logger.LogInformation($"Template files: {assemblies.Count.ToString()}");
                var pluginContainer = new ContainerConfiguration().WithAssemblies(assemblies);
                using (var container = pluginContainer.CreateContainer())
                {
                    _templates = container.GetExports<IBaseTemplate>("Templates");
                }
            }
            catch (System.IO.DirectoryNotFoundException dfe)
            {
                _logger.LogError(dfe.Message);
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Unable to find message templates. {ex.Message}");
            }

            if (_templates == null) return "No plugin folder exists.";

            var template = _templates.Where(t => t.Name == templateName).FirstOrDefault();

            if (template == null) return $"No template is found with given name:{templateName}";
            return template.Create(message, from, image);
        }
    }
}