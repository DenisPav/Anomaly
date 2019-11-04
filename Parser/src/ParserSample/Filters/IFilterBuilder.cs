using ParserSample.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace ParserSample.Filters
{
    public class FilterEntityDefinition<TEntity>
    {
        public ParameterExpression ParamExpr = null;
        public Dictionary<string, MemberExpression> PropertyDefinitions = new Dictionary<string, MemberExpression>();
    }

    public class FilterBuilder<TEntity>
    {
        public FilterEntityDefinition<TEntity> FilterDefinition { get; set; } = new FilterEntityDefinition<TEntity>();

        public void Define<TTarget>(string name, Expression<Func<TEntity, TTarget>> property)
        {
            var memberInfo = (property.Body as MemberExpression).Member;

            if (FilterDefinition.ParamExpr == null)
            {
                FilterDefinition.ParamExpr = Parameter(typeof(TEntity), nameof(TEntity));
            }

            var memberExpr = MakeMemberAccess(FilterDefinition.ParamExpr, memberInfo);
            FilterDefinition.PropertyDefinitions.Add(name, memberExpr);
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
