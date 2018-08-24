using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace NotifyMe.Pages
{
    public class DemoModel : PageModel
    {
        public string Message { get; set; }

        private ILogger<DemoModel> _logger;
        public DemoModel(ILogger<DemoModel> logger)
        { 
            _logger = logger;

        }
        public void OnGet()
        {

        }
    }
}
