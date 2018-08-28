using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using NotifyMe.Services;

namespace NotifyMe.Pages
{
    [Authorize]
    public class Chat : PageModel
    {
        private IVisitorService _visitors;

        public List<Data.Models.User> Users { get; private set; }

         public List<Data.Models.Message> Messages { get; private set; }
        public Chat(IServiceProvider provider)
        {
            _visitors = (IVisitorService)provider.GetService(typeof(IVisitorService));
        }

        public void OnGet()
        {
            Users = _visitors.GetUsers();
        }

        public void OnGetUserChatAsync(int userId)
        {
            Users = _visitors.GetUsers();
            Messages= _visitors.GetUserMessages(userId);
        }
    }
}