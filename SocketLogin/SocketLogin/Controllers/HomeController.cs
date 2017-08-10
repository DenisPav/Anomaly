using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SocketLogin.Database;
using SocketLogin.Middleware;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketLogin.Controllers
{
    [Route("~/")]
    public class HomeController : Controller
    {
        private IDataProtector Protector { get; set; }
        private DatabaseContext Db { get; set; }
        private IMemoryCache Cache { get; set; }

        public HomeController(IDataProtectionProvider provider, IMemoryCache cache, DatabaseContext db)
        {
            this.Protector = provider.CreateProtector("magic");
            this.Db = db;
            this.Cache = cache;
        }

        [HttpGet]
        public IActionResult Index()
            => View();

        [HttpGet("login/{token}")]
        public async Task<IActionResult> LoginToken(string token)
        {
            var unprotected = Protector.Unprotect(token).Split(',');
            var userId = int.Parse(unprotected[0]);
            var uniqueToken = Guid.Parse(unprotected[1]);

            if (Cache.TryGetValue<int>(uniqueToken, out var cacheUserId) && userId == cacheUserId)
            {
                var socket = WebSocketMiddleware.Sockets.FirstOrDefault(x => x.Unique == uniqueToken).Socket;
                var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(token));
                await socket.SendAsync(buffer, System.Net.WebSockets.WebSocketMessageType.Text, true, CancellationToken.None);

                return Ok(new
                {
                    Message = "Everything ok"
                });
            }
            else
            {
                return BadRequest(new
                {
                    Message = "Wrong Token"
                });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm]string token)
        {
            var unprotected = Protector.Unprotect(token).Split(',');
            var userId = int.Parse(unprotected[0]);
            var uniqueToken = Guid.Parse(unprotected[1]);

            if (Cache.TryGetValue<int>(uniqueToken, out var cacheUserId) && userId == cacheUserId)
            {
                var identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, userId.ToString()),
                }, "cookie");

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.Authentication.SignInAsync("cookies", principal);

                return RedirectToAction(nameof(Protected));
            }
            else
            {
                return BadRequest(new
                {
                    Message = "Wrong token"
                });
            }
        }

        [HttpGet("protected", Name = nameof(Protected))]
        [Authorize(Policy = "testPolicy")]
        public IActionResult Protected()
        {
            return Ok(new
            {
                Message = "Protected resource"
            });
        }
    }
}
