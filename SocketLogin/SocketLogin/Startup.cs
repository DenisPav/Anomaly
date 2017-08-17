using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SocketLogin.Database;
using SocketLogin.Extensions;
using SocketLogin.Middleware;
using SocketLogin.Services;
using System;

namespace SocketLogin
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddMemoryCache();
            services.AddDbContext<DatabaseContext>(opts => opts.UseInMemoryDatabase(nameof(SocketLogin)));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<AuthService>();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthDefaults());
            services.AddAuthorization(opts => opts.AddPolicy("testPolicy", builder => builder.RequireAuthenticatedUser()));
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, DatabaseContext db)
        {
            db.EnsureCreated();

            app.UseDeveloperExceptionPage();
            app.UseWebSockets();
            app.UseAuthentication();

            app.UseMiddleware<WebSocketMiddleware>();
            app.UseMvc();
        }

        private Action<CookieAuthenticationOptions> CookieAuthDefaults()
            => opts =>
            {
                opts.ExpireTimeSpan = TimeSpan.FromHours(10);
            };
    }
}
