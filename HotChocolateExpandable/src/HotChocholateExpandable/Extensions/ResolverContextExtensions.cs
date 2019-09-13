using HotChocholateExpandable.GraphQL.Middlewares;
using HotChocolate.Resolvers;
using Microsoft.Extensions.DependencyInjection;

namespace HotChocholateExpandable.Extensions
{
    public static class ResolverContextExtensions
    {
        public static TService GetService<TService>(this IResolverContext ctx)
            => (ctx.ContextData[FreshServiceScopeMiddleware.Scope] as IServiceScope).ServiceProvider.GetRequiredService<TService>();

    }
}
