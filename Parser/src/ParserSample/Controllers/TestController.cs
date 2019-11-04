using Microsoft.AspNetCore.Mvc;
using ParserSample.Expressions;
using ParserSample.Filters;
using ParserSample.Models;
using ParserSample.Parsers;
using System.Linq;

namespace ParserSample.Controllers
{
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        readonly FilterContainer<Post> FilterContainer;
        readonly IFilterParser<Post> FilterParser;
        readonly IExpressionProvider<Post> ExpressionProvider;

        public TestController(
            FilterContainer<Post> filterContainer,
            IFilterParser<Post> filterParser,
            IExpressionProvider<Post> expressionProvider)
        {
            FilterContainer = filterContainer;
            FilterParser = filterParser;
            ExpressionProvider = expressionProvider;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var query = "Id = 5 OR Id = 12";
            
            var filterDefinitions = FilterParser.Parse(query);
            var filterExpression = ExpressionProvider.CreateFilterExpression(filterDefinitions);

            var fakeData = Enumerable.Repeat(1, 500)
                .Select((_, index) => new Post
                {
                    Id = index,
                    Count = index + index,
                    Name = index + index.ToString()
                })
                .ToList()
                .AsQueryable();

            var filtered = fakeData.Where(filterExpression)
                .ToList();

            return Ok(filtered);
        }
    }
}
