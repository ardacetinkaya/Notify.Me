using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotifyMe.Data;
using Microsoft.Extensions.Configuration;
using NotifyMe.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;

namespace NotifyMe
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddHealthChecks()
                    .AddCheck<CustomCheck>();
            
            services.AddLogging();
            services.AddTransient<IVisitorService, VisitorService>();
            services.AddTransient<IMessageService, MessageService>();
            services.AddSingleton<ITemplateService, TemplateService>();

            services.AddDbContext<NotifyDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddCors(options => options.AddPolicy("CorsPolicy",
                builder =>
                {
                    builder.AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowAnyOrigin()
                    .AllowCredentials();
                }));

            services.AddSignalR(con =>
            {
                con.KeepAliveInterval = TimeSpan.FromMinutes(5);
                con.EnableDetailedErrors = true;
                con.HandshakeTimeout = TimeSpan.FromMinutes(5);

            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHealthChecks("/IsHealthy" ,new HealthCheckOptions()
            {
                ResponseWriter = WriteAsJson
            });
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();
            app.UseCors("CorsPolicy");
            app.UseSignalR(routes =>
            {
                routes.MapHub<Notify>("/Ntfctn");
            });
            app.UseMvc();

        }

        private static Task WriteAsJson(HttpContext httpContext,CompositeHealthCheckResult result)
        {
            httpContext.Response.ContentType = "application/json";
            var json = JsonConvert.SerializeObject(result,Formatting.Indented);

            return httpContext.Response.WriteAsync(json);
        }
    }
}
