using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParserSample.Database;
using ParserSample.Filters;
using ParserSample.Models;
using System.Threading.Tasks;

namespace ParserSample.Controllers
{
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        readonly DatabaseContext Db;
        readonly IFilterProvider<Post> FilterProvider;
        
        public TestController(
            DatabaseContext db,
            IFilterProvider<Post> filterProvider)
        {
            Db = db;
            FilterProvider = filterProvider;
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery]FilterRequestModel model)
        {
            //sample: Id = '11c43ee8-b9d3-4e51-b73f-bd9dda66e29c' AND Date > '2019-11-05' AND Title LIKE 'Maggie Cruz' AND Count <= 133

            var postQueryable = Db.Set<Post>();
            var filteredPosts = await FilterProvider.Apply(postQueryable, model)
                .ToListAsync();

            return Ok(filteredPosts);
        }
    }
}
