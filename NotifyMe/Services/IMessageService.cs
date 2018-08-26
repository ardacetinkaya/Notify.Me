using NotifyMe.Data.Models;

namespace NotifyMe.Services
{
    public interface IMessageService
    {
        bool SaveMessage(string connectionId, Message message);
        string GetMessageTemplate(string message, string from, string image);
    }
}