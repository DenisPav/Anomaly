using Microsoft.AspNetCore.Http;
using SocketLogin.Database;
using SocketLogin.Services;
using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace SocketLogin.Middleware
{
    public class WebSocketMiddleware
    {
        private RequestDelegate Next { get; set; }
        private AuthService AuthService { get; set; }

        public static ConcurrentBag<SocketWrapper> Sockets { get; set; } = new ConcurrentBag<SocketWrapper>();

        public class SocketWrapper
        {
            public int Id { get; set; }
            public Guid Unique { get; set; }
            public WebSocket Socket { get; set; }

            public static SocketWrapper Create(int id, Guid unique, WebSocket socket)
                => new SocketWrapper { Socket = socket, Id = id, Unique = unique };
        }

        public WebSocketMiddleware(RequestDelegate next, AuthService authService)
        {
            this.Next = next;
            this.AuthService = authService;
        }

        public async Task Invoke(HttpContext ctx, DatabaseContext db)
        {
            if (ctx.WebSockets.IsWebSocketRequest && ctx.Request.Query.TryGetValue("email", out var email))
            {
                var result = await AuthService.CreateLink(email);

                var socket = await ctx.WebSockets.AcceptWebSocketAsync();

                var wrapper = SocketWrapper.Create(result.UserId, result.Unique, socket);
                Sockets.Add(wrapper);

                var url = $"{Program.URL}/login/{result.Protected}";
                Console.WriteLine(url);

                while (wrapper.Socket.State == WebSocketState.Open) { await Task.Delay(5000); }
            }
            else
            {
                await Next(ctx);
            }
        }
    }
}
