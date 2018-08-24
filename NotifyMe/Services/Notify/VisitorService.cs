using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NotifyMe.Data;

namespace NotifyMe.Services
{
    public class VisitorService
    {
        private IServiceProvider _provider;
        private ILogger<VisitorService> _logger;
        private NotifyDbContext _db;
        public VisitorService(IServiceProvider provider, IConfiguration configuration, ILogger<VisitorService> logger)
        {
            _provider = provider;
            _logger = logger;
            _db = (NotifyDbContext)_provider.GetService(typeof(NotifyDbContext));
        }
        public int GetActiveVisitorCount() => _db.Connections.Where(c => c.Connected).Count();
    }
}