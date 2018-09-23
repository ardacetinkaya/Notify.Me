using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NotifyMe.Services
{
    public interface IVisitorService
    {

        List<NotifyMe.Data.Models.User> GetUsers();
        int GetActiveVisitorCount();
        int GetTotalVisitorCount();
        List<NotifyMe.Data.Models.Connection> GetVisitors(int start, int length = 10);
        void LetInVisitor(string connectionId, string name = "", string url = "");
        void LetOutVisitor(string connectionId);
        List<Data.Models.Message> GetUserMessages(string name);

        Task<DateTime> GetLastConnection();
    }
}
