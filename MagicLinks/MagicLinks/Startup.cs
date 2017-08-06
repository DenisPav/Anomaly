using MagicLinks.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Authorization;

namespace MagicLinks
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection();
            services.AddDbContext<DatabaseContext>(opts => opts.UseInMemoryDatabase());
            services.AddRouting();
            services.AddAuthorization(opts => opts.AddPolicy("testPolicy", builder => builder.RequireAuthenticatedUser()));
            services.AddMemoryCache();
        }

        public void Configure(IApplicationBuilder app, DatabaseContext context)
        {
            context.CreateUsers().Wait();
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "AuthScheme",
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                ExpireTimeSpan = TimeSpan.FromDays(10),
            });

            app.UseDeveloperExceptionPage();

            var router = new RouteBuilder(app);

            router.MapGet("magic/{email}", async (HttpContext ctx) =>
            {
                var email = ctx.GetRouteValue("email").ToString();
                var db = ctx.RequestServices.GetRequiredService<DatabaseContext>();
                var protector = ctx.RequestServices.GetRequiredService<IDataProtectionProvider>().CreateProtector("magic");
                var cache = ctx.RequestServices.GetRequiredService<IMemoryCache>();
                var user = await db.Users.FirstOrDefaultAsync(x => x.Email == email);

                if (user != null)
                {
                    var data = protector.Protect(user.Email);
                    var magic = $"{Program.URL}login/{data}";

                    cache.Remove(user.Email);
                    cache.GetOrCreate(user.Email, entry => data);

                    await ctx.Response.WriteAsync(magic);
                }
                else
                {
                    await ctx.Response.WriteAsync("");
                }
            });

            router.MapGet("login/{token}", async (HttpContext ctx) =>
            {
                var protector = ctx.RequestServices.GetRequiredService<IDataProtectionProvider>().CreateProtector("magic");
                var db = ctx.RequestServices.GetRequiredService<DatabaseContext>();
                var email = protector.Unprotect(ctx.GetRouteValue("token").ToString());
                var cache = ctx.RequestServices.GetRequiredService<IMemoryCache>();

                if (cache.Get(email) != null)
                {
                    var user = await db.Users.FirstOrDefaultAsync(x => x.Email == email);

                    var claims = new List<Claim> {
                         new Claim(ClaimTypes.Name, Guid.NewGuid().ToString()),
                         new Claim(ClaimTypes.NameIdentifier, user.Email)
                    };
                    var identity = new ClaimsIdentity(claims, "cookies");
                    var principal = new ClaimsPrincipal(identity);

                    await ctx.Authentication.SignInAsync("AuthScheme", principal);
                    await ctx.Response.WriteAsync("Done signing in");
                }

                await ctx.Response.WriteAsync("");
            });

            router.MapGet("protected", async (HttpContext ctx) =>
            {
                var authService = ctx.RequestServices.GetRequiredService<IAuthorizationService>();
                if (await authService.AuthorizeAsync(ctx.User, "testPolicy"))
                {
                    await ctx.Response.WriteAsync("Protected resource");
                }
                else
                {
                    await ctx.Response.WriteAsync("Please login in order to view this");
                }
            });

            app.UseRouter(router.Build());
        }
    }
}
