using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using NotifyMe.Services;

namespace NotifyMe.Pages
{
    [Authorize]
    public class Chat : PageModel
    {
        private readonly IVisitorService _visitors;
        private readonly ITemplateService _templateService;
        private readonly ILogger<Chat> _logger;

        public List<Data.Models.User> Users { get; private set; }

        public List<string> Messages { get; private set; }
        public Chat(IServiceProvider provider, ILogger<Chat> logger)
        {
            _visitors = (IVisitorService)provider.GetService(typeof(IVisitorService));
            _templateService = (ITemplateService)provider.GetService(typeof(ITemplateService));
            _logger = logger;
            Users = _visitors.GetUsers();
        }

        public void OnGet()
        {

        }

        public void OnGetUserChatAsync(int userId)
        {

            var user = Users.Where(u => u.Id == userId).FirstOrDefault();

            if (user != null)
            {
                var messages = _visitors.GetUserMessages(user.UserName);

                Messages = new List<string>();
                foreach (var message in messages)
                {
                    var messageString = _templateService.GetTemplate("Base Chat")
                                                        .Create(message.Content
                                                            , message.FromUser
                                                            , string.Empty
                                                            , "http://placehold.it/50/FA6F57/fff&text=WU"
                                                            , message.Date
                                                            , message.ToUser);

                    Messages.Add(messageString);
                }

                _logger.LogInformation($"Messages:{Messages.Count}");
            }


        }
    }
}