using Microsoft.AspNetCore.Mvc;
using ParserSample.Expressions;
using ParserSample.Filters;
using ParserSample.Models;
using ParserSample.Parsers;
using System;
using System.Linq;

namespace ParserSample.Controllers
{
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        readonly IFilterParser<Post> FilterParser;
        readonly IExpressionProvider<Post> ExpressionProvider;

        public TestController(
            IFilterParser<Post> filterParser,
            IExpressionProvider<Post> expressionProvider)
        {
            FilterParser = filterParser;
            ExpressionProvider = expressionProvider;
        }

        [HttpGet]
        public IActionResult Get([FromQuery]FilterRequestModel model)
        {
            //sample: Id = '11c43ee8-b9d3-4e51-b73f-bd9dda66e29c' AND Date > '2019-11-05' AND Title LIKE 'Maggie Cruz' AND Count <= 133
            var query = model.Filter;

            var filterDefinitions = FilterParser.Parse(query);
            var filterExpression = ExpressionProvider.CreateFilterExpression(filterDefinitions);

            var fakeData = Enumerable.Repeat(1, 500)
                .Select((_, index) => new Post
                {
                    Id = Guid.Empty,
                    Count = index + index,
                    Name = index + index.ToString(),
                    CreationDate = DateTime.Now.AddDays(index)
                })
                .ToList()
                .Concat(new[] {
                    new Post
                    {
                        Id = Guid.Parse("11c43ee8-b9d3-4e51-b73f-bd9dda66e29c"),
                        Count = 133,
                        Name = "Maggie Cruz",
                        CreationDate = DateTime.Now
                    }
                })
                .AsQueryable();

            var filtered = fakeData.Where(filterExpression)
                .ToList();

            return Ok(filtered);
        }
    }
}
