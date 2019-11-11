using System.Linq;
using ParserSample.Expressions;
using ParserSample.Parsers;

namespace ParserSample.Filters
{
    public class FilterProvider<TEntity> : IFilterProvider<TEntity>
    {
        readonly IFilterParser<TEntity> FilterParser;
        readonly IExpressionProvider<TEntity> ExpressionProvider;

        public FilterProvider(
            IFilterParser<TEntity> filterParser,
            IExpressionProvider<TEntity> expressionProvider)
        {
            FilterParser = filterParser;
            ExpressionProvider = expressionProvider;
        }

        public IQueryable<TEntity> Apply(IQueryable<TEntity> query, FilterRequestModel filterModel)
        {
            var filterDefinitions = FilterParser.Parse(filterModel.Filter);
            var filterExpression = ExpressionProvider.CreateFilterExpression(filterDefinitions);

            return query.Where(filterExpression);
        }
    }
}
