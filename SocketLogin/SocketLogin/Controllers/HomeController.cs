using Microsoft.AspNetCore.Authentication.Cookies;
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
        [HttpGet]
        public IActionResult Index()
            => View();

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
