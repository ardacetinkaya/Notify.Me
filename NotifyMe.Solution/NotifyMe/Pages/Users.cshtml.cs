using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NotifyMe.Data;
using NotifyMe.Services;

namespace NotifyMe.Pages
{
    [Authorize]
    public class Users : BasePage
    {
        private IVisitorService _visitors;
        public Users(IServiceProvider provider, IConfiguration configuration)
        {
            _visitors = (IVisitorService)provider.GetService(typeof(IVisitorService));
        }
        
        public JsonResult OnGetUsersAsync(int draw, int start, int length)
        {
            var totalCount = _visitors.GetTotalVisitorCount();
            var connections = _visitors.GetVisitors(start,length);
            
            dynamic response = new
            {
                Data = connections,
                Draw = draw,
                RecordsTotal = totalCount,
                RecordsFiltered = totalCount,
            };

            return new JsonResult(response);
        }
    }

}