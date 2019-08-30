using HotChocolate;
using HotChocolate.Language;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotChocholateExpandable.GraphQL.Middlewares
{
    public class FieldCollectingMiddleware
    {
        public const string DataKey = "fieldsToResolve";
        readonly FieldDelegate Next;

        public FieldCollectingMiddleware(FieldDelegate next) => Next = next;

        public async Task InvokeAsync(IMiddlewareContext ctx, ISchema schema)
        {
            ctx.ContextData[DataKey] = GetFields(ctx, schema, ctx.FieldSelection.SelectionSet, ctx.Field.Type.InnerType() as ObjectType);

            await Next(ctx);
        }

        private IEnumerable<FieldWrapper> GetFields(IMiddlewareContext ctx, ISchema schema, SelectionSetNode selectionSet, ObjectType currentObjectType)
        {
            var selections = ctx.CollectFields(currentObjectType, selectionSet);

            if (selections.Any())
            {
                foreach (var selection in selections)
                {
                    var fieldNode = (selection.Selection as FieldNode);
                    var fieldName = currentObjectType.Fields
                        .FirstOrDefault(field => field.Name == fieldNode.Name.Value)
                        ?.Member
                        ?.Name;

                    var wrapper = new FieldWrapper
                    {
                        Name = fieldName ?? fieldNode.Name.Value,
                        Nested = selection.Selection.SelectionSet != null
                            ? GetFields(ctx, schema, selection.Selection.SelectionSet, selection.Field.Type.InnerType() as ObjectType).ToList()
                            : Enumerable.Empty<FieldWrapper>().ToList()
                    };

                    yield return wrapper;
                }
            }
        }
    }

    public class FieldWrapper
    {
        public string Name { get; set; }
        public List<FieldWrapper> Nested { get; set; }
    }
}
