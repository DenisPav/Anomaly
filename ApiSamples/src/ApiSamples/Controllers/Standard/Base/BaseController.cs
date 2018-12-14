using Microsoft.AspNetCore.Mvc;

namespace ApiSamples.Controllers.Standard.Base
{
    [ApiController]
    [Route("v1/[controller]")]
    public abstract class BaseController : Controller
    {
    }
}
