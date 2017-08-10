using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SocketLogin.Database;
using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace SocketLogin.Middleware
{
    public class WebSocketMiddleware
    {
        private RequestDelegate Next { get; set; }
        private IMemoryCache Cache { get; set; }
        private IDataProtector Protector { get; set; }

        public static ConcurrentBag<SocketWrapper> Sockets { get; set; } = new ConcurrentBag<SocketWrapper>();

        public class SocketWrapper
        {
            public int Id { get; set; }
            public Guid Unique { get; set; }
            public WebSocket Socket { get; set; }

            public static SocketWrapper Create(int id, WebSocket socket)
                => new SocketWrapper { Socket = socket, Id = id, Unique = Guid.NewGuid() };
        }

        public WebSocketMiddleware(RequestDelegate next, IMemoryCache cache, IDataProtectionProvider provider)
        {
            this.Next = next;
            this.Cache = cache;
            this.Protector = provider.CreateProtector("magic");
        }

        public async Task Invoke(HttpContext ctx, DatabaseContext db)
        {
            if (ctx.WebSockets.IsWebSocketRequest && ctx.Request.Query.TryGetValue("email", out var email))
            {
                var user = await db.Users.FirstOrDefaultAsync(x => x.Email == email.ToString());

                if (user != null)
                {
                    var socket = await ctx.WebSockets.AcceptWebSocketAsync();

                    var wrapper = SocketWrapper.Create(user.Id, socket);
                    Sockets.Add(wrapper);

                    var data = Protector.Protect($"{wrapper.Id},{wrapper.Unique}");
                    Cache.Set(wrapper.Unique, wrapper.Id, DateTimeOffset.Now.AddMinutes(2));

                    var url = $"{Program.URL}/login/{data}";
                    Console.WriteLine(url);

                    while(wrapper.Socket.State == WebSocketState.Open) { await Task.Delay(5000); }
                }
            }
            else
            {
                await Next(ctx);
            }
        }
    }
}
