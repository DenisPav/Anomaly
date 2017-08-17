using MagicLinks.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;

namespace MagicLinks
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection();
            services.AddDbContext<DatabaseContext>(opts => opts.UseInMemoryDatabase(nameof(MagicLinks)));
            services.AddAuthorization(opts => opts.AddPolicy("testPolicy", builder => builder.RequireAuthenticatedUser()));
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(opts =>
                {
                    opts.ExpireTimeSpan = TimeSpan.FromDays(10);
                    opts.Events = new CookieAuthenticationEvents
                    {
                        OnRedirectToLogin = ctx => Task.CompletedTask
                    };
                });
            services.AddMemoryCache();
            services.AddMvcCore()
                .AddJsonFormatters()
                .AddAuthorization();
        }

        public void Configure(IApplicationBuilder app, DatabaseContext context)
        {
            context.CreateUsers().Wait();

            app.UseAuthentication();
            app.UseDeveloperExceptionPage();
            app.UseMvc();
        }
    }
}
