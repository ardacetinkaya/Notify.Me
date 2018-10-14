using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NotifyMe.Services;

namespace NotifyMe.Pages
{

    [Authorize]
    public class IndexModel : BasePage
    {

        private readonly IHubContext<Notify> _hub;
        private readonly IVisitorService _visitors;
        private readonly ITemplateService _templateService;
        private readonly ILogger<IndexModel> _logger;

        public List<Data.Models.User> Users { get; private set; }

        public List<string> Messages { get; private set; }
        public IndexModel(IHubContext<Notify> hub, IServiceProvider provider, IConfiguration configuration,ILogger<IndexModel> logger)
        {
            _hub = hub;
            _visitors = (IVisitorService)provider.GetService(typeof(IVisitorService));
            _templateService = (ITemplateService)provider.GetService(typeof(ITemplateService));
            _logger = logger;
            Users = _visitors.GetUsers();
        }

        public async void OnGetAsync(string status = "")
        {
            if (!string.IsNullOrEmpty(status))
            {
                var userName = User.Identity.Name;
                if (!string.IsNullOrEmpty(userName))
                {
                    //Another way to say hello from codebehind of a Razor page
                    await _hub.Clients.All.SendAsync("SayHello", $"{status.ToLower()}");
                }

            }
            ViewData["Version"] = Version;

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
