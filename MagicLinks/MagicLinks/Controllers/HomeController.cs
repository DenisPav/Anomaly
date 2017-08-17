using MagicLinks.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MagicLinks.Controllers
{
    [Route("~/")]
    public class HomeController : Controller
    {
        private IDataProtector Protector { get; set; }
        private IMemoryCache Cache { get; set; }
        private DatabaseContext Db { get; set; }

        public HomeController(DatabaseContext db, IDataProtectionProvider protectionProvider, IMemoryCache cache)
        {
            this.Db = db;
            this.Protector = protectionProvider.CreateProtector("magic");
            this.Cache = cache;
        }

        [HttpGet()]
        public IActionResult Index()
            => Ok(new
            {
                Magic = $"{Program.URL}magic/email@gmail.com"
            });

        [HttpGet("magic/{email}")]
        public async Task<IActionResult> MagicLink(string email)
        {
            if (await Db.Users.AnyAsync(x => x.Email == email))
            {
                var data = Protector.Protect(email);
                var magic = $"{Program.URL}login/{data}";

                Cache.Remove(email);
                Cache.GetOrCreate(email, entry => data);

                return Ok(new
                {
                    MagicLink = magic
                });
            }
            else
            {
                return NotFound(new
                {
                    Message = "Email not found"
                });
            }
        }

        [HttpGet("login/{token}")]
        public async Task<IActionResult> MagicLogin(string token)
        {
            var email = Protector.Unprotect(token);

            if (Cache.Get(email) != null)
            {
                var user = await Db.Users.FirstOrDefaultAsync(x => x.Email == email);

                var claims = new List<Claim> {
                         new Claim(ClaimTypes.Name, user.Id.ToString()),
                         new Claim(ClaimTypes.NameIdentifier, user.Email)
                };

                var identity = new ClaimsIdentity(claims, "cookies");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return Ok(new
                {
                    Message = "Loged in successfully"
                });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "Something went wrong"
                });
            }
        }

        [HttpGet(nameof(Protected))]
        [Authorize(Policy = "testPolicy")]
        public IActionResult Protected()
         => Ok(new
         {
             Message = "Route with heavy protection"
         });
    }
}
