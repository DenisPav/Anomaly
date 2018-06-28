using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Versioning.Constraints;

namespace Versioning.Swagger
{
    public class HeaderOperationFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            var paths = new List<KeyValuePair<string, PathItem>>();

            context.ApiDescriptions.ToList()
                .GroupBy(x => x.ActionDescriptor.AttributeRouteInfo.Template)
                .ToList()
                .ForEach(group =>
                {
                    var description = group.FirstOrDefault();

                    KeyValuePair<string, PathItem> path = KeyValuePair.Create<string, PathItem>(null, null);


                    var constraint = description.ActionDescriptor.ActionConstraints.OfType<HeaderConstraint>()
                                           .FirstOrDefault();

                    if (constraint != null && constraint.Versions.Contains(swaggerDoc.Info.Version.Split("v").Last()))
                    {
                        if (path.Value == null)
                            path = swaggerDoc.Paths.FirstOrDefault(docPath => docPath.Key == $"/{description.ActionDescriptor.AttributeRouteInfo.Template}");
                    }

                    if (path.Value != null)
                    {
                        var props = path.Value
                            .GetType()
                            .GetProperties()
                            .Where(prop => !group.Select(y => y.HttpMethod.ToLower()).Contains(prop.Name.ToLower()) && GetVerbs().Contains(prop.Name))
                            .ToList();

                        foreach (var prop in props)
                            prop.SetValue(path.Value, null);

                        paths.Add(path);
                    }
                });

            swaggerDoc.Paths.Clear();
            paths.ForEach(x => swaggerDoc.Paths.Add(x.Key, x.Value));
        }

        private IEnumerable<string> GetVerbs()
            => typeof(HttpMethods).GetFields()
                .Select(prop => prop.Name);
    }
}