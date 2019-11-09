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
        const string GreaterThanStr = ">";
        const string GreaterThanOrEqualStr = ">=";
        const string LessThanStr = "<";
        const string LessThanOrEqualStr = "<=";
        const string EqualStr = "=";
        const string NotEqualStr = "!=";
        public const char TickValueChar = '\'';

        public static readonly IEnumerable<FilterOperationDefinition> Operations = new[] {
            new FilterOperationDefinition
            {
                String = GreaterThanOrEqualStr,
                OperationExprFactory = GreaterThanOrEqual
            },
            new FilterOperationDefinition
            {
                String = LessThanOrEqualStr,
                OperationExprFactory = LessThanOrEqual
            },
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

        public static Func<Expression, Expression, BinaryExpression> GetExpressionFactory(string operationStr)
            => Operations.Single(operation => operation.String == operationStr)
            .OperationExprFactory;

        const string AndStr = "AND";
        const string OrStr = "OR";
        
        public static readonly IEnumerable<FilterLogicalOperationDefinition> LogicalOperations = new[] {
            new FilterLogicalOperationDefinition
            {
                String = AndStr,
                LogicalOperationExprFactory = And
            },
            new FilterLogicalOperationDefinition
            {
                String = OrStr,
                LogicalOperationExprFactory = Or
            }
        };

        public static Func<Expression, Expression, BinaryExpression> GetLogicalExpressionFactory(string operationStr)
            => LogicalOperations.Single(logicalOperation => logicalOperation.String == operationStr)
            .LogicalOperationExprFactory;
    }
}
