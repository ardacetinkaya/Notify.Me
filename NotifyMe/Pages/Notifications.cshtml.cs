using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using NotifyMe.Data;
using NotifyMe.Services;

namespace NotifyMe.Pages
{
    [Authorize]
    public class Notifications : PageModel
    {

        private readonly IHubContext<Notify> _hub;
        private NotifyDbContext _db;
        public Notifications(IHubContext<Notify> hub, IServiceProvider serviceProvider)
        {
            _hub = hub;
            _db = (NotifyDbContext)serviceProvider.GetService(typeof(NotifyDbContext));
        }

        public async Task<JsonResult> OnGetAllNotificationsAsync(int draw, int start, int length)
        {
            var notifications = _db.Message.Where(m => m.Type == MessageType.Notification.ToString())
                                        .OrderByDescending(o=>o.Date)
                                        .Skip(start)
                                        .Take(length).ToList();

            dynamic response = new
            {
                Data = notifications,
                Draw=draw,
                RecordsTotal = notifications.Count+10,
                RecordsFiltered = notifications.Count+10,
            };
            
            return new JsonResult(response);
        }


    }

}
