using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ParserSample.Filters
{
    public class FilterPropertyDefinition
    {
        public MemberExpression MemberExpression { get; set; }
        public Type MemberType { get; set; }
    }

    public class FilterEntityDefinition<TEntity>
    {
        public ParameterExpression ParamExpr = null;
        public Dictionary<string, FilterPropertyDefinition> PropertyDefinitions { get; private set; } = new Dictionary<string, FilterPropertyDefinition>();
    }
}
