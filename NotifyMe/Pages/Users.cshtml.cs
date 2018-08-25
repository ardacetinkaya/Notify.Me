using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NotifyMe.Data;

namespace NotifyMe.Pages
{
    [Authorize]
    public class Users : PageModel
    {
        private NotifyDbContext _db;
        public Users(IServiceProvider provider, IConfiguration configuration)
        {
            _db = (NotifyDbContext)provider.GetService(typeof(NotifyDbContext));
        }

        public void OnGet()
        {

        }

        public  JsonResult OnGetUsersAsync(int draw, int start, int length)
        {
            var connections = _db.Connections.Include(i=>i.User)
                                        
                                        .OrderByDescending(o=>o.ConnectionDate)
                                        .Skip(start)
                                        .Take(length).ToList();

            dynamic response = new
            {
                Data = connections,
                Draw=draw,
                RecordsTotal = connections.Count+10,
                RecordsFiltered = connections.Count+10,
            };
            
            return new JsonResult(response);
        }
    }

}