using Microsoft.AspNetCore.Mvc;
using SocketLogin.Services;
using System.Threading.Tasks;

namespace SocketLogin.Controllers
{
    public class AccountController : Controller
    {
        private AuthService AuthService { get; set; }

        public AccountController(AuthService authService)
        {
            this.AuthService = authService;
        }

        [HttpGet("login/{token}")]
        public async Task<IActionResult> LoginToken(string token)
        {
            var result = await AuthService.NotifyWebSocket(token);

            if (result.Status)
                return Ok(new { Message = result.Message });
            return BadRequest(new { Message = result.Message });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm]string token)
        {
            var result = await AuthService.LoginViaToken(token);

            if (result)
                return RedirectToRoute(nameof(HomeController.Protected));
            else
                return BadRequest(new
                {
                    Message = "Wrong token"
                });
        }
    }
}
