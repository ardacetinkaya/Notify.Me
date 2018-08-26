using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NotifyMe.Data;
using NotifyMe.Data.Models;

namespace NotifyMe.Services
{
    public class VisitorService : IVisitorService
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

        public int GetTotalVisitorCount() => _db.Connections.Count();

        public List<NotifyMe.Data.Models.Connection> GetVisitors(int start, int length = 10)
        {
            var connections = _db.Connections.Include(i => i.User)
                                    .OrderByDescending(o => o.ConnectionDate)
                                    .Skip(start)
                                    .Take(length).ToList();

            return connections;

        }

        public  void LetInVisitor(string connectionId, string name = "", string url = "")
        {
            var user = _db.Users.Where(u => u.UserName == name).FirstOrDefault();

            var connection = new Connection()
            {
                ConnectionID = connectionId,
                UserAgent = url,
                Connected = true,
                ConnectionDate = DateTime.Now,
                User = user ?? new User() { UserName = name }
            };

            _db.Connections.Add(connection);

             _db.SaveChanges();

        }

        public  void LetOutVisitor(string connectionId)
        {
            if (!string.IsNullOrEmpty(connectionId))
            {
                var connection = _db.Connections.Where(c => c.ConnectionID == connectionId).FirstOrDefault();
                if (connection != null)
                {
                    connection.Connected = false;
                    connection.DisconnectionDate = DateTime.Now;
                    _db.Connections.Update(connection);
                    _db.SaveChanges();

                }
            }
        }
    }
}