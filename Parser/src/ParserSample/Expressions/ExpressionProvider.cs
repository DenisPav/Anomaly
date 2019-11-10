using ParserSample.Filters;
using ParserSample.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace ParserSample.Expressions
{
    public class ExpressionProvider<TEntity> : IExpressionProvider<TEntity>
    {
        readonly FilterContainer<TEntity> FilterContainer;

        IDictionary<string, FilterPropertyDefinition> EntityPropertyDefinitions => FilterContainer.FilterBuilder
            .FilterDefinition
            .PropertyDefinitions;

        public ExpressionProvider(
            FilterContainer<TEntity> filterContainer)
        {
            FilterContainer = filterContainer;
        }

        public Expression<Func<TEntity, bool>> CreateFilterExpression(IEnumerable<FilterDefinition> filterDefinitions)
        {
            var entityFilter = filterDefinitions.Select(def => new
            {
                PropertyDefinition = EntityPropertyDefinitions[def.Property],
                def.Logical,
                def.Operation,
                def.Value
            })
            .ToList();

            var expressions = entityFilter.Select(filter =>
            {
                var memberExpression = filter.PropertyDefinition.MemberExpression;
                var constantExpression = Constant(filter.Value);

                Expression GetBinaryExpr(string operation) => FilterParserConfiguration.GetExpressionFactory(operation)(memberExpression, constantExpression);
                return new
                {
                    Expression = GetBinaryExpr(filter.Operation),
                    Filter = filter
                };
            })
            .ToList();

            Expression endingExpr = null;
            for (int i = 0; i < expressions.Count; i++)
            {
                var expr = expressions[i];

                if (i == 0)
                {
                    endingExpr = expr.Expression;
                }
                else
                {
                    endingExpr = FilterParserConfiguration.GetLogicalExpressionFactory(expressions[i - 1].Filter.Logical)(endingExpr, expr.Expression);
                }
            }

            var filterLambda = Lambda(
                endingExpr,
                FilterContainer.FilterBuilder.FilterDefinition.ParamExpr
            ) as Expression<Func<TEntity, bool>>;

            return filterLambda;
        }
    }
}
