using ParserSample.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

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
        public Dictionary<string, FilterPropertyDefinition> PropertyDefinitions = new Dictionary<string, FilterPropertyDefinition>();
    }

    public class FilterBuilder<TEntity>
    {
        public FilterEntityDefinition<TEntity> FilterDefinition { get; set; } = new FilterEntityDefinition<TEntity>();

        public void Define<TTarget>(string name, Expression<Func<TEntity, TTarget>> property)
        {
            var memberInfo = (property.Body as MemberExpression).Member;
            var propertyInfo = memberInfo as PropertyInfo;

            if (FilterDefinition.ParamExpr == null)
            {
                FilterDefinition.ParamExpr = Parameter(typeof(TEntity), nameof(TEntity));
            }

            var memberExpr = MakeMemberAccess(FilterDefinition.ParamExpr, memberInfo);
            FilterDefinition.PropertyDefinitions.Add(name, new FilterPropertyDefinition
            {
                MemberExpression = memberExpr,
                MemberType = propertyInfo.PropertyType ?? Nullable.GetUnderlyingType(propertyInfo.PropertyType)
            });
        }
    }

    public interface IFilterConfiguration<TEntity>
    {
        void Configure(FilterBuilder<TEntity> configuration);
    }

    public abstract class FilterConfiguration<TEntity> : IFilterConfiguration<TEntity>
    {
        public abstract void Configure(FilterBuilder<TEntity> configuration);
    }

    public class PostConfiguration : FilterConfiguration<Post>
    {
        public override void Configure(FilterBuilder<Post> configuration)
        {
            configuration.Define("Id", post => post.Id);
            configuration.Define("Count", post => post.Count);
            configuration.Define("Date", post => post.CreationDate);
            configuration.Define("Title", post => post.Name);
        }
    }

    public class FilterContainer<TEntity>
    {
        public readonly FilterConfiguration<TEntity> FilterConfiguration;
        public readonly FilterBuilder<TEntity> FilterBuilder;
        public FilterContainer(
            FilterBuilder<TEntity> filterBuilder,
            FilterConfiguration<TEntity> filterConfiguration)
        {
            FilterBuilder = filterBuilder;
            FilterConfiguration = filterConfiguration;

            FilterConfiguration.Configure(FilterBuilder);
        }
    }
}
