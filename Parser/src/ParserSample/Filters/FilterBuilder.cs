using System;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace ParserSample.Filters
{
    public class FilterBuilder<TEntity>
    {
        public FilterEntityDefinition<TEntity> FilterDefinition { get; private set; } = new FilterEntityDefinition<TEntity>();

        public void Define<TTarget>(string name, Expression<Func<TEntity, TTarget>> property)
        {
            var memberInfo = (property.Body as MemberExpression).Member;
            var propertyInfo = memberInfo as PropertyInfo;

            if (FilterDefinition.ParamExpr == null)
            {
                FilterDefinition.ParamExpr = Parameter(typeof(TEntity), nameof(TEntity));
            }

            var memberExpr = MakeMemberAccess(FilterDefinition.ParamExpr, memberInfo);
            var memberType = propertyInfo.PropertyType ?? Nullable.GetUnderlyingType(propertyInfo.PropertyType);

            FilterDefinition.PropertyDefinitions.Add(name, new FilterPropertyDefinition
            {
                MemberExpression = memberExpr,
                MemberType = memberType
            });
        }
    }
}
