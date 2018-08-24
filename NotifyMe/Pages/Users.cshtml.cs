using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
    }

}