using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
    public class Templates : PageModel
    {
        private IHostingEnvironment _environment;

        public Templates(IHostingEnvironment environment)
        {
            _environment = environment;
        }
        [BindProperty]
        public IFormFile Upload { get; set; }

        public async Task OnPostAsync()
        {
            var file = Path.Combine(_environment.ContentRootPath, "Plugins", Upload.FileName);
            using (var fileStream = new FileStream(file, FileMode.Create))
            {
                await Upload.CopyToAsync(fileStream);
            }
        }
    }
}