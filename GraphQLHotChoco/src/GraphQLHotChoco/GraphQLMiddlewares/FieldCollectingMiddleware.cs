using HotChocolate.Language;
using HotChocolate.Resolvers;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQLHotChoco.GraphQLMiddlewares
{
    public class FieldCollectingMiddleware
    {
        public const string DataKey = "fieldsToResolve";
        readonly FieldDelegate Next;

        public FieldCollectingMiddleware(FieldDelegate next)
        {
            Next = next;
        }

        public async Task InvokeAsync(IMiddlewareContext ctx)
        {
            var collected = ctx.FieldSelection
                        .SelectionSet
                        .Selections
                        .Select(selection => selection as FieldNode)
                        .Select(GetFields)
                        .ToList();

            ctx.ContextData[DataKey] = collected;

            await Next(ctx);
        }

        private FieldWrapper GetFields(FieldNode fieldNode)
        {
            var fieldName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(fieldNode.Name.Value);

            var wrapper = new FieldWrapper
            {
                Name = fieldName
            };

            if (fieldNode.SelectionSet != null)
            {
                fieldNode.SelectionSet
                    .Selections
                    .Select(x => x as FieldNode)
                    .Select(nestedFieldNode => GetFields(nestedFieldNode))
                    .ToList()
                    .ForEach(nestedFieldDef => wrapper.Nested.Add(nestedFieldDef));
            }

            return wrapper;
        }
    }

    public class FieldWrapper
    {
        public string Name { get; set; }
        public List<FieldWrapper> Nested { get; set; } = new List<FieldWrapper>();
    }
}
