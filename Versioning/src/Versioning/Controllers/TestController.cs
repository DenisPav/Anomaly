using Microsoft.AspNetCore.Mvc;
using Versioning.Attributes;

namespace Versioning.Controllers
{
    [Route("[controller]")]
    public class TestController : Controller
    {
        [HttpGet()]
        [Version("2")]
        [Version("1")]
        public IActionResult Get()
        {
            return Ok(new { Message = "GET action of TestController" });
        }
    }
}