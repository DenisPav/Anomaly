using GraphQLHotChoco.GraphQLMiddlewares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GraphQLHotChoco.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplyFields<T>(this IQueryable<T> source, IEnumerable<FieldWrapper> fieldWrappers)
        {
            fieldWrappers.Select(x => x.Nested)
                .ToList()
                .ForEach(item =>
                {
                    var names = item.Select(x => x.Name)
                        .ToArray();

                    source = source.SelectMembers(names);
                });

            return source;
        }

        public static IQueryable<T> SelectMembers<T>(this IQueryable<T> source, params string[] memberNames)
        {
            var parameter = Expression.Parameter(typeof(T), "e");
            var bindings = memberNames
                .Select(name => Expression.PropertyOrField(parameter, name))
                .Select(member => Expression.Bind(member.Member, member));
            var body = Expression.MemberInit(Expression.New(typeof(T)), bindings);
            var selector = Expression.Lambda<Func<T, T>>(body, parameter);
            return source.Select(selector);
        }
    }
}
