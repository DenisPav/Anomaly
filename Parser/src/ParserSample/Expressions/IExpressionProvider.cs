using ParserSample.Filters;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ParserSample.Expressions
{
    public interface IExpressionProvider<TEntity>
    {
        Expression<Func<TEntity, bool>> CreateFilterExpression(IEnumerable<FilterDefinition> filterDefinitions);
    }
}
