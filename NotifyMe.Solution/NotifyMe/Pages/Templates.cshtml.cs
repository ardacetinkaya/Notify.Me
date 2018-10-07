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
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using NotifyMe.Common;
using NotifyMe.Data;
using NotifyMe.Services;

namespace NotifyMe.Pages
{
    [Authorize]
    public class Templates : BasePage
    {
        private readonly IHostingEnvironment _environment;
        private readonly ITemplateService _templateService;
        
        public Templates(IHostingEnvironment environment,IServiceProvider provider)
        {
            _environment = environment;
            _templateService = (ITemplateService)provider.GetService(typeof(ITemplateService));
            
        }
        [BindProperty]
        public IFormFile Upload { get; set; }

        public void OnGet()
        {

            TemplateList = _templateService.Templates;
        }

        public IEnumerable<IBaseTemplate> TemplateList { get; private set;}

        public async Task OnPostAsync()
        {
            var file = Path.Combine(_environment.ContentRootPath, "Plugins", Upload.FileName);
            System.IO.File.Delete(file);
            
            using (var fileStream = new FileStream(file, FileMode.Create))
            {
                await Upload.CopyToAsync(fileStream);
            }
            _templateService.Load();
        }
    }
}