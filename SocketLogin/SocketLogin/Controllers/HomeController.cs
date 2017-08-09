using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
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

        public HomeController(IDataProtectionProvider provider, DatabaseContext db)
        {
            this.Protector = provider.CreateProtector("magic");
            this.Db = db;
        }

        [HttpGet]
        public IActionResult Index()
            => View();

        [HttpGet("login/{token}")]
        public async Task<IActionResult> LoginToken(string token)
        {
            var unprotected = Protector.Unprotect(token).Split(',');
            var email = unprotected[0];
            var id = Guid.Parse(unprotected[1]);

            var socket = WebSocketMiddleware.Sockets.FirstOrDefault(x => x.Email == email && x.Id == id).Socket;
            var buff = new ArraySegment<byte>(Encoding.UTF8.GetBytes(token));
            await socket.SendAsync(buff, System.Net.WebSockets.WebSocketMessageType.Text, true, CancellationToken.None);

            return Ok(new
            {
                Message = "Everything ok"
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm]string token)
        {
            var unprotected = Protector.Unprotect(token).Split(',');
            var email = unprotected[0];
            var id = Guid.Parse(unprotected[1]);

            var identity = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Name, id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, email)
            }, "cookie");

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.Authentication.SignInAsync("cookies", principal);

            return RedirectToAction(nameof(Protected));
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
