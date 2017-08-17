using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SocketLogin.Database;
using SocketLogin.Middleware;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketLogin.Services
{
    public class LinkResult
    {
        public int UserId { get; set; }
        public Guid Unique { get; set; }
        public string Protected { get; set; }

        public LinkResult(int userId, Func<Guid, string> dataCreator)
        {
            this.UserId = userId;
            this.Unique = Guid.NewGuid();
            this.Protected = dataCreator(Unique);
        }
    }

    public class NotifyResult
    {
        public string Message { get; set; }
        public bool Status { get; set; }
    }

    public class AuthService
    {
        private const string PURPOSE = "magic";
        private IDataProtector Protector { get; set; }
        private IMemoryCache Cache { get; set; }
        private IHttpContextAccessor Http { get; set; }
        private DatabaseContext Db { get; set; }

        public AuthService(IMemoryCache cache, IDataProtectionProvider provider, IHttpContextAccessor accessor, DatabaseContext db)
        {
            this.Cache = cache;
            this.Protector = provider.CreateProtector(PURPOSE);
            this.Db = db;
            this.Http = accessor;
        }

        public async Task<LinkResult> CreateLink(string email)
        {
            var user = await Db.Users.FirstOrDefaultAsync(x => x.Email == email.ToString());

            if (user != null)
            {
                var result = new LinkResult(user.Id, guid => Protector.Protect($"{user.Id},{guid}"));
                Cache.Set(result.Unique, result.UserId, DateTimeOffset.Now.AddMinutes(2));

                return result;
            }
            else
                throw new ArgumentException($"Argument: ${nameof(email)} is not valid");
        }

        public async Task<NotifyResult> NotifyWebSocket(string token)
        {
            var unprotected = Protector.Unprotect(token).Split(',');
            var userId = int.Parse(unprotected[0]);
            var uniqueToken = Guid.Parse(unprotected[1]);

            if (Cache.TryGetValue<int>(uniqueToken, out var cacheUserId) && userId == cacheUserId)
            {
                var socket = WebSocketMiddleware.Sockets.FirstOrDefault(x => x.Unique == uniqueToken).Socket;
                var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(token));
                await socket.SendAsync(buffer, System.Net.WebSockets.WebSocketMessageType.Text, true, CancellationToken.None);

                return new NotifyResult { Message = "Everything ok", Status = true };
            }
            else
                return new NotifyResult { Message = "Wrong token", Status = false };
        }

        public async Task<bool> LoginViaToken(string token)
        {
            var unprotected = Protector.Unprotect(token).Split(',');
            var userId = int.Parse(unprotected[0]);
            var uniqueToken = Guid.Parse(unprotected[1]);

            if (Cache.TryGetValue<int>(uniqueToken, out var cacheUserId) && userId == cacheUserId)
            {
                var identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, userId.ToString()),
                }, CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                await Http.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return true;
            }
            return false;
        }
    }
}
