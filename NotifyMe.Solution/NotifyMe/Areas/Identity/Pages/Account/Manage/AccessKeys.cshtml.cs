using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NotifyMe.Data.Models;
using NotifyMe.Pages;
using NotifyMe.Services;

namespace NotifyMe.Areas.Identity.Pages.Account.Manage
{
    public class AccessKeys : BasePage
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        private readonly AccountManageService _accountService;

        public AccessKeys(ILogger<AccountManageService> logger, IServiceProvider provider,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;

            _accountService = new AccountManageService(logger, provider, signInManager, userManager);
        }

        public IList<ApplicationFeature> Features { get; set; }

        public string URL { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var id = _userManager.GetUserId(User);

            Features = await _accountService.GetApplicationFeature(User);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string url)
        {
            var result = await _accountService.SaveAccessURLForUser(url, User);
            if (result)
                StatusMessage = "URL is saved.";
            else
                StatusMessage = "Can not save the URL";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRevokeAsync(int id)
        {
            var result = await _accountService.RevokeAccess(id, User);
            if (result)
                StatusMessage = "Access is revoked.";
            else
                StatusMessage = "Can not revoke access...Please try again!";

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostGenerateAsync(int id)
        {
            var result = await _accountService.ReGenerate(id, User);
            if (result)
                StatusMessage = "Access key is generated.";
            else
                StatusMessage = "Can not generate a key...";
            return RedirectToPage();
        }

    }
}
