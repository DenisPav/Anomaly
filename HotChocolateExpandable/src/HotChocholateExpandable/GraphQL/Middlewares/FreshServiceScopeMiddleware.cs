using HotChocolate.Resolvers;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace HotChocholateExpandable.GraphQL.Middlewares
{
    public class FreshServiceScopeMiddleware
    {
        public const string Scope = nameof(Scope);

        readonly FieldDelegate Next;

        public FreshServiceScopeMiddleware(FieldDelegate next) => Next = next;

        public async Task InvokeAsync(IMiddlewareContext ctx)
        {
            using (var scope = ctx.Service<IServiceScopeFactory>().CreateScope())
            {
                ctx.ContextData[Scope] = scope;
                await Next(ctx);
            }
        }
    }
}
