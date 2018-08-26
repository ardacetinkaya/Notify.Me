using System.Collections.Generic;

namespace NotifyMe.Services
{
    public interface IVisitorService
    {

        int GetActiveVisitorCount();
        int GetTotalVisitorCount();
        List<NotifyMe.Data.Models.Connection> GetVisitors(int start, int length = 10);
        void LetInVisitor(string connectionId,string name = "",string url="");
        void LetOutVisitor(string connectionId);
    }
}
