using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.EntityFrameworkCore;
using SocketLogin.Database;
using SocketLogin.Middleware;

namespace SocketLogin
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddDbContext<DatabaseContext>(opts => opts.UseInMemoryDatabase());
            services.AddAuthorization(opts => opts.AddPolicy("testPolicy", builder => builder.RequireAuthenticatedUser()));
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, DatabaseContext db)
        {
            db.Users.AddRange(new Models.User
            {
                Email = "first@gmail.com"
            });
            db.SaveChanges();

            app.UseDeveloperExceptionPage();
            app.UseWebSockets();
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "cookies",
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                ExpireTimeSpan = TimeSpan.FromHours(10)
            });

            app.UseMiddleware<WebSocketMiddleware>();
            app.UseMvc();
        }
    }
}
