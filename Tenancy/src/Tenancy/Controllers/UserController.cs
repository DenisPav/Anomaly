using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Tenancy.Db;

namespace Tenancy.Controllers
{
    public class UserController : BaseController
    {
        readonly DatabaseContext Db;

        public UserController(
            DatabaseContext db    
        )
        {
            Db = db;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetTenantUsersAsync()
        {
            return Ok(await Db.Users.ToListAsync());
        }
    }
}
