using System;
using System.Composition;
using NotifyMe.Common;

namespace NotifyMe.Templates
{
    [Export("Templates",typeof(IBaseTemplate))]
    public class ChatMessageBaseTemplate : IBaseTemplate
    {
        public string Name => "Base Chat";

        public string Create(string message, string from, string image)
        {
            var messageContainer = "<span class=\"chat-img pull-left\">"
               + $"          <img src=\"{image}\" alt=\"User\" class=\"img-circle\" />"
               + "     </span>"
               + "     <div class=\"chat-body clearfix\">"
               + "         <div class=\"header\">"
               + $"             <small class=\"text-muted\"><span class=\"glyphicon glyphicon-time\"></span>{DateTime.Now.ToShortTimeString()}</small>"
               + $"             <strong class=\"pull-right primary-font\">{from}</strong>"
               + "        </div>"
               + $"         <p>{message}"
               + "         </p>"
               + "     </div>";

            return messageContainer;

        }
    }
}
