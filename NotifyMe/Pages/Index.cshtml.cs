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
        private readonly Notify _notify;

        public string Message { get; private set; }

        public IndexModel(IHubContext<Notify> hub,IServiceProvider provider, IConfiguration configuration)
        {
            _hub = hub;
            //_notify = new Notify(provider,configuration);
        }

        public async void OnGetAsync(string status="")
        {
            
            //Message = _notify.GetConnected().ToString();
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
