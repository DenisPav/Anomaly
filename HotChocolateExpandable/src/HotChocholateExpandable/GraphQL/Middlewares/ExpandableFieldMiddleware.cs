using HotChocholateExpandable.Database;
using HotChocholateExpandable.Extensions;
using HotChocholateExpandable.Models;
using HotChocolate.Resolvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotChocholateExpandable.GraphQL.Middlewares
{
    public class ExpandableFieldMiddleware<TModel>
    {
        readonly FieldDelegate Next;

        public ExpandableFieldMiddleware(FieldDelegate next)
        {
            Next = next;
        }

        public async Task InvokeAsync(IMiddlewareContext ctx)
        {
            await Next(ctx).ConfigureAwait(false);

            if(ctx.ContextData.TryGetValue(FieldCollectingMiddleware.DataKey, out var collectedFields))
            {
                var fields = collectedFields as IEnumerable<FieldWrapper>;
                var castedResult = ctx.Result as IQueryable;

                if(castedResult != null)
                {
                    var casted = ctx.Result as IQueryable<TModel>;
                    var selected = casted.CreateSelection(fields);

                    ctx.Result = selected;
                }
            }

        }
    }
}
