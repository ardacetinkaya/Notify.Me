
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using NotifyMe.Data;
using NotifyMe.Data.Models;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace NotifyMe.Services
{
    public class AccountManageService
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<AccountManageService> _logger;
        private readonly NotifyDbContext _db;

        public AccountManageService(ILogger<AccountManageService> logger, IServiceProvider provider, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _db = (NotifyDbContext)provider.GetService(typeof(NotifyDbContext));

        }

        public async Task<List<ApplicationFeature>> GetApplicationFeature(ClaimsPrincipal user)
        {
            var s = await _userManager.GetUserAsync(user);

            var features = _db.ApplicationFeatures.Where(f => f.ApplicationUserId == s.Id && !f.IsRevoked).ToList();

            return features;
        }

        private string GenerateKey()
        {
            Guid g = Guid.NewGuid();
            string key = Convert.ToBase64String(g.ToByteArray()).Replace("=", "").Replace("+", "");
            return key;
        }

        public async Task<bool> SaveAccessURLForUser(string url, ClaimsPrincipal user)
        {
            try
            {
                if (!string.IsNullOrEmpty(url))
                {

                    var u = await _userManager.GetUserAsync(user);

                    int count = await GetAccessCount(user);
                    if (count >= 5) throw new InvalidOperationException("Access limit is full.");

                    _db.ApplicationFeatures.Add(new ApplicationFeature()
                    {
                        ApplicationUserId = u.Id,
                        URL = url,
                        Key = GenerateKey(),
                        CreateDate = DateTimeOffset.Now
                    });

                    _db.SaveChanges();
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"Unable to save URL. Detail: {ex.Message}");
            }

            return false;

        }

        public async Task<int> GetAccessCount(ClaimsPrincipal user)
        {
            var u = await _userManager.GetUserAsync(user);
            var accessCount = _db.ApplicationFeatures.Where(f => f.ApplicationUserId == u.Id
                                                         && !f.IsRevoked).Count();
            return accessCount;
        }
        public async Task<bool> RevokeAccess(int id, ClaimsPrincipal user)
        {
            try
            {
                var u = await _userManager.GetUserAsync(user);

                var access = _db.ApplicationFeatures.Where(f => f.ApplicationUserId == u.Id
                                                         && f.Id == id).FirstOrDefault();
                if (access != null)
                {
                    access.RevokeDate = DateTimeOffset.Now;
                    access.IsRevoked = true;
                }

                _db.ApplicationFeatures.Update(access);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"Unable to revoke access. Detail:{ex.Message}");
            }


            return false;
        }

        public async Task<bool> ReGenerate(int id, ClaimsPrincipal user)
        {
            try
            {
                var u = await _userManager.GetUserAsync(user);

                var access = _db.ApplicationFeatures.Where(f => f.ApplicationUserId == u.Id
                                                         && f.Id == id).FirstOrDefault();
                if (access != null)
                {
                    access.Key = GenerateKey();
                }

                _db.ApplicationFeatures.Update(access);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"Unable to regenerate access key. Detail:{ex.Message}");
            }


            return false;
        }
    }
}