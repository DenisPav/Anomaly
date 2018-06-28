using Microsoft.AspNetCore.Mvc;
using Versioning.Attributes;

namespace Versioning.Controllers
{
    [Route("[controller]")]
    [Version("2")]
    public class UserController : Controller
    {
        [HttpPost()]
        public IActionResult Name()
        {
            return Ok(new { });
        }

        [HttpDelete("delete")]
        public IActionResult Delete()
        {
            return Ok();
        }

        [HttpPut]
        public IActionResult NotMapped()
        {
            return Ok();
        }
    }
}