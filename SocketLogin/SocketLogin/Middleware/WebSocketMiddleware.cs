using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
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
            public Guid Id { get; set; }
            public string Email { get; set; }
            public WebSocket Socket { get; set; }

            public static SocketWrapper Create(string email, WebSocket socket)
                => new SocketWrapper { Email = email, Socket = socket, Id = Guid.NewGuid() };
        }

        public WebSocketMiddleware(RequestDelegate next, IMemoryCache cache, IDataProtectionProvider provider)
        {
            this.Next = next;
            this.Cache = cache;
            this.Protector = provider.CreateProtector("magic");
        }

        public async Task Invoke(HttpContext ctx)
        {
            if (ctx.WebSockets.IsWebSocketRequest)
            {
                var socket = await ctx.WebSockets.AcceptWebSocketAsync();
                ctx.Request.Query.TryGetValue("email", out var email);
                var wrapper = SocketWrapper.Create(email, socket);
                Sockets.Add(wrapper);

                var data = Protector.Protect($"{wrapper.Email},{wrapper.Id}");
                var url = $"{Program.URL}/login/{data}";
                Console.WriteLine(url);

                while (wrapper.Socket.State == WebSocketState.Open) { }
            }
            else
            {
                await Next(ctx);
            }
        }
    }
}
