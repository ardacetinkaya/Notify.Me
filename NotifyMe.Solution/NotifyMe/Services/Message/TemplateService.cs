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
    public class TemplateService : ITemplateService
    {
        private readonly ILogger<MessageService> _logger;
        private readonly IHostingEnvironment _hosting;
        [ImportMany]
        public IEnumerable<IBaseTemplate> Templates { get; set; }
        public TemplateService(IServiceProvider provider, IConfiguration configuration, IHostingEnvironment hosting, ILogger<MessageService> logger)
        {
            _logger = logger;
            _hosting = hosting;

            Load();
        }
        public void Load()
        {
            try
            {
                Templates = null;
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
                    Templates = container.GetExports<IBaseTemplate>("Templates");

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
        }

        public IBaseTemplate GetTemplate(string name)
        {
            IBaseTemplate _t = null;
            try
            {
                if (Templates != null)
                    _t = Templates.Where(t => t.Name == name).FirstOrDefault();
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Unable to find '{name}' template. {ex.Message}");
                throw;
            }

            return _t;
        }
    }
}