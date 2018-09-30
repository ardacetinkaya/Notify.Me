using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public List<User> GetUsers()
        {
            var users = _db.Users.ToList();

            return users;
        }

        public List<NotifyMe.Data.Models.Connection> GetVisitors(int start, int length = 10)
        {
            try
            {
                var connections = _db.Connections.Include(i => i.User)
                        .OrderByDescending(o => o.Connected).ThenByDescending(d => d.ConnectionDate)
                        .Skip(start)
                        .Take(length).ToList();

                return connections;
            }
            catch (System.Exception ex)
            {

                _logger.LogError(ex,ex.Message);
            }

            return new List<NotifyMe.Data.Models.Connection>();


        }

        public bool HasVisitorAccess(string host, string key)
        {
            try
            {
                var hasAccess = _db.ApplicationFeatures.Where(h => h.URL.Equals(host) && h.Key.Equals(key)).FirstOrDefault();
                if (hasAccess != null) return true;
            }
            catch (System.Exception ex)
            {

                _logger.LogError(ex, ex.Message);
            }


            return false;
        }

        public void LetInVisitor(string connectionId, string name = "", string url = "")
        {
            try
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
            catch (System.Exception ex)
            {

                _logger.LogError(ex, ex.Message);
            }


        }

        public void LetOutVisitor(string connectionId)
        {
            try
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
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

        }

        public List<Message> GetUserMessages(string name)
        {
            try
            {
                var messages = _db.Message.Where(u => u.FromUser == name || u.ToUser == name).ToList();
                return messages;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Unable to get user's message(s). Detail: {ex.Message}");
            }

            return new List<Message>();
        }

        async Task<DateTime> IVisitorService.GetLastConnection()
        {
            DateTime lastConnection = await _db.Connections.OrderBy(o => o.ConnectionDate).Take(1).Select(s => s.ConnectionDate).FirstAsync();
            return lastConnection;
        }

    }
}