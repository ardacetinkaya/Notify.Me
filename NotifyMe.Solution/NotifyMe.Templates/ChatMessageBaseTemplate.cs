using System;
using System.Composition;
using NotifyMe.Common;

namespace NotifyMe.Templates
{
    [Export("Templates", typeof(IBaseTemplate))]
    public class ChatMessageBaseTemplate : IBaseTemplate
    {
        public string Name => "Base Chat";

        public string Create(string message, string from, string image, DateTimeOffset date, string to)
        {
            string dateTime = string.Empty;
            TimeSpan diff = DateTime.Now.Subtract(date.DateTime);
            if (diff.Days >= 1)
                dateTime = date.DateTime.ToString("dd.MM.yyyy hh:mm");
            else
                dateTime = DateTime.Now.ToShortTimeString();

            var messageContainer = "<span class=\"chat-img pull-left\">"
               + $"          <img src=\"{image}\" alt=\"User\" class=\"img-circle\" />"
               + "     </span>"
               + "     <div class=\"chat-body clearfix\">"
               + "         <div class=\"header\">"
               + $"             <small class=\"text-muted\"><span class=\"glyphicon glyphicon-time\" title=\"{date.DateTime.ToShortTimeString()}\"></span>{dateTime}</small>"
               + $"             <strong class=\"pull-right primary-font\">{from}</strong>"
               + "        </div>"
               + $"         <p>{message}"
               + "         </p>"
               + "     </div>";

            return messageContainer;

        }
    }
}
