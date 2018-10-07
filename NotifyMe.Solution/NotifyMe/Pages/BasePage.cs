using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
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
    public class BasePage : PageModel
    {
        public string Version { get; private set; }
        public string OperatingSystem { get; private set; }
        public BasePage()
        {
            var framework = Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName;

            var features = new
            {
                OS = System.Runtime.InteropServices.RuntimeInformation.OSDescription,
                AspDotnetVersion = framework
            };

            Version = features.AspDotnetVersion;
            OperatingSystem = features.OS;

        }

        public override void OnPageHandlerExecuted(Microsoft.AspNetCore.Mvc.Filters.PageHandlerExecutedContext context)
        {
            ViewData["Version"]=Version;
        }


    }
}