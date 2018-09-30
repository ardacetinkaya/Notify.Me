using NotifyMe.Data.Models;

namespace NotifyMe.Services
{
    public interface IMessageService
    {
        bool SaveMessage(string connectionId, Message message);
        string CreateMessage(string templateName,string message, string from,string friendlyName, string image);
    }
}