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
    public class Notifications : PageModel
    {

        private readonly IHubContext<Notify> _hub;

        public Notifications(IHubContext<Notify> hub)
        {
            _hub = hub;
        }

        public async void OnGetAsync(string status="")
        {
   

        }

       
    }
}
