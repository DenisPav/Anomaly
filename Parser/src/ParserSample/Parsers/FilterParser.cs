using ParserSample.Filters;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ParserSample.Parsers
{
    public class FilterParser<TEntity> : IFilterParser<TEntity>
    {
        FilterBuilder<TEntity> FilterBuilder;

        IEnumerable<string> Properties => FilterBuilder.FilterDefinition.PropertyDefinitions.Keys;
        IEnumerable<FilterOperationDefinition> Operations => FilterParserConfiguration.Operations;
        IEnumerable<FilterLogicalOperationDefinition> LogicalOperations => FilterParserConfiguration.LogicalOperations;
        TextParser<char?> WhitespaceParser = Character.EqualTo(' ').Optional();

        TextParser<TextSpan> PropertyParser { get; set; }
        TextParser<TextSpan> OperationParser { get; set; }
        TextParser<char[]> ValueParser { get; set; }
        TextParser<TextSpan> LogicalOperationParser { get; set; }

        public FilterParser(
            FilterContainer<TEntity> _,
            FilterBuilder<TEntity> filterBuilder)
        {
            FilterBuilder = filterBuilder;

            CreateParsers();
        }

        private void CreateParsers()
        {
            PropertyParser = CreatePropertyParser();
            OperationParser = CreateOperationParser();
            LogicalOperationParser = CreateLogicalOperationParser();
            ValueParser = CreateValueParser();
        }

        private TextParser<TextSpan> WrapWithOptionalWhitespace(TextParser<TextSpan> parser, string prop)
            => parser.Or(Span.EqualTo(prop).Between(WhitespaceParser, WhitespaceParser));

        private TextParser<TextSpan> CreatePropertyParser()
        {
            var propertyParsers = Properties.Select(prop => WrapWithOptionalWhitespace(Span.EqualTo(prop), prop))
                            .ToList();

            return propertyParsers.Aggregate((TextParser<TextSpan>)null, (accumulator, propertyParser) =>
            {
                if (accumulator == null)
                    return propertyParser;

                return accumulator.Or(propertyParser);
            });
        }

        private TextParser<TextSpan> CreateOperationParser()
        {
            var operationParsers = Operations.Select(operation => WrapWithOptionalWhitespace(Span.EqualTo(operation.String), operation.String))
                            .ToList();

            return operationParsers.Aggregate((TextParser<TextSpan>)null, (accumulator, operationParser) =>
            {
                if (accumulator == null)
                    return operationParser;

                return accumulator.Try()
                    .Or(operationParser);
            });
        }

        private TextParser<TextSpan> CreateLogicalOperationParser()
        {
            var logicalOperationParsers = LogicalOperations.Select(operation => WrapWithOptionalWhitespace(Span.EqualTo(operation.String), operation.String))
                            .ToList();

            var combinedLogicalOperationParsers = logicalOperationParsers.Aggregate((TextParser<TextSpan>)null, (accumulator, logicalOperationParser) =>
            {
                if (accumulator == null)
                    return logicalOperationParser;

                return accumulator.Or(logicalOperationParser);
            });

            return combinedLogicalOperationParsers.OptionalOrDefault(new TextSpan(string.Empty));
        }

        private TextParser<char[]> CreateValueParser()
        {
            var tick = FilterParserConfiguration.TickValueChar;

            return Character.Except(tick)
                .Many()
                .Between(Character.EqualTo(tick), Character.EqualTo(tick))
                .Or(Character.Digit.Many());
        }

        public IEnumerable<FilterDefinition> Parse(string filterQuery)
        {
            var sanitizedQuery = filterQuery.Trim();
            var splittedQuery = sanitizedQuery.Split(LogicalOperations.Select(operation => operation.String).ToArray(), StringSplitOptions.RemoveEmptyEntries);

            Type GetPropertyType(string property) => FilterBuilder.FilterDefinition.PropertyDefinitions[property].MemberType;
            var dynamicParser = (from parserDefinition in (
                                    from prop in PropertyParser
                                    from operation in OperationParser
                                    from value in ValueParser
                                    from logicalOperation in LogicalOperationParser
                                    select new FilterDefinition(prop.ToStringValue(), operation.ToStringValue(), TypeDescriptor.GetConverter(GetPropertyType(prop.ToStringValue())).ConvertFromInvariantString(new string(value)), logicalOperation.ToStringValue())
                                    )
                                    .Repeat(splittedQuery.Count())
                                 select parserDefinition);

            return dynamicParser.Parse(sanitizedQuery);
        }
    }
}
