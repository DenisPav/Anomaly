using ParserSample.Filters;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace ParserSample.Parsers
{
    public class FilterOperationDefinition
    {
        public string String { get; set; }
        public Func<Expression, Expression, BinaryExpression> OperationExprFactory { get; set; }
    }

    public class FilterLogicalOperationDefinition
    {
        public string String { get; set; }
        public Func<Expression, Expression, BinaryExpression> LogicalOperationExprFactory { get; set; }
    }

    public static class FilterParserConfiguration
    {
        public static readonly IEnumerable<FilterOperationDefinition> Operations = new[] {
            new FilterOperationDefinition
            {
                String = GreaterThanStr,
                OperationExprFactory = GreaterThan
            },
            new FilterOperationDefinition
            {
                String = LessThanStr,
                OperationExprFactory = LessThan
            },
            new FilterOperationDefinition
            {
                String = EqualStr,
                OperationExprFactory = Equal
            },
            new FilterOperationDefinition
            {
                String = NotEqualStr,
                OperationExprFactory = NotEqual
            }
        };

        public const string GreaterThanStr = ">";
        public const string LessThanStr = "<";
        public const string EqualStr = "=";
        public const string NotEqualStr = "!=";

        public static Func<Expression, Expression, BinaryExpression> GetExpressionFactory(string operationStr) 
            => Operations.Single(operation => operation.String == operationStr)
            .OperationExprFactory;

        public static readonly IEnumerable<FilterLogicalOperationDefinition> LogicalOperations = new[] {
            new FilterLogicalOperationDefinition
            {
                String = "AND",
                LogicalOperationExprFactory = And
            },
            new FilterLogicalOperationDefinition
            {
                String = "OR",
                LogicalOperationExprFactory = Or
            }
        };

        public static Func<Expression, Expression, BinaryExpression> GetLogicalExpressionFactory(string operationStr)
            => LogicalOperations.Single(logicalOperation => logicalOperation.String == operationStr)
            .LogicalOperationExprFactory;
    }

    public interface IFilterParser<TEntity>
    {
        IEnumerable<FilterDefinition> Parse(string filterQuery);
    }

    public class FilterParser<TEntity> : IFilterParser<TEntity>
    {
        readonly FilterBuilder<TEntity> FilterBuilder;

        IEnumerable<string> Properties => FilterBuilder.FilterDefinition.PropertyDefinitions.Keys;
        IEnumerable<FilterOperationDefinition> Operations => FilterParserConfiguration.Operations;
        IEnumerable<FilterLogicalOperationDefinition> LogicalOperations => FilterParserConfiguration.LogicalOperations;

        TextParser<TextSpan> PropertyParser { get; set; }
        TextParser<TextSpan> OperationParser { get; set; }
        TextParser<char[]> ValueParser { get; set; }
        TextParser<TextSpan> LogicalOperationParser { get; set; }

        public FilterParser(
            FilterBuilder<TEntity> filterBuilder)
        {
            FilterBuilder = filterBuilder;

            CreateParsers();
        }

        private void CreateParsers()
        {
            var propertyParsers = Properties.Select(prop => Span.EqualTo(prop))
                .ToList();

            PropertyParser = propertyParsers.Aggregate((TextParser<TextSpan>)null, (accumulator, propertyParser) =>
            {
                if (accumulator == null)
                    return propertyParser;

                return accumulator.Or(propertyParser);
            });

            var operationParsers = Operations.Select(operation => Span.EqualTo(operation.String))
                .ToList();

            OperationParser = operationParsers.Aggregate((TextParser<TextSpan>)null, (accumulator, operationParser) =>
            {
                if (accumulator == null)
                    return operationParser;

                return accumulator.Or(operationParser);
            });

            var logicalOperationParsers = LogicalOperations.Select(operation => Span.EqualTo(operation.String))
                .ToList();

            var combinedLogicalOperationParsers = logicalOperationParsers.Aggregate((TextParser<TextSpan>)null, (accumulator, logicalOperationParser) =>
            {
                if (accumulator == null)
                    return logicalOperationParser;

                return accumulator.Or(logicalOperationParser);
            });

            LogicalOperationParser = combinedLogicalOperationParsers.OptionalOrDefault(new TextSpan(""));
            ValueParser = Character.Digit.Many();
        }

        public IEnumerable<FilterDefinition> Parse(string filterQuery)
        {
            var sanitizedQuery = filterQuery.Replace(" ", "")
                .Trim();
            var splittedQuery = sanitizedQuery.Split(LogicalOperations.Select(operation => operation.String).ToArray(), StringSplitOptions.RemoveEmptyEntries);

            var dynamicParser = (from parserDefinition in (
                                    from prop in PropertyParser
                                    from operation in OperationParser
                                    from value in ValueParser
                                    from logicalOperation in LogicalOperationParser
                                    select new FilterDefinition(prop.ToStringValue(), operation.ToStringValue(), int.Parse(value), logicalOperation.ToStringValue())
                                    )
                                    .Repeat(splittedQuery.Count())
                                 select parserDefinition);

            return dynamicParser.Parse(sanitizedQuery);
        }
    }
}
