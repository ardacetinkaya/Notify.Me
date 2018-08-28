using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using NotifyMe.Services;

namespace NotifyMe.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {

        private readonly IHubContext<Notify> _hub;
        public string Message { get; private set; }

        public IndexModel(IHubContext<Notify> hub,IServiceProvider provider, IConfiguration configuration)
        {
            _hub = hub;
        }

        public async void OnGetAsync(string status="")
        {
            
            Message = "1";
            if (!string.IsNullOrEmpty(status))
            {
                var userName = User.Identity.Name;
                if (!string.IsNullOrEmpty(userName))
                {
                    //Another way to say hello from codebehind of a Razor page
                    await _hub.Clients.All.SendAsync("SayHello", $"I'm {status}");
                }

            }


        }

       
    }
}
