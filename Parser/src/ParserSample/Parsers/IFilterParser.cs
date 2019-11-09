using ParserSample.Filters;
using System.Collections.Generic;

namespace ParserSample.Parsers
{
    public interface IFilterParser<TEntity>
    {
        IEnumerable<FilterDefinition> Parse(string filterQuery);
    }
}
