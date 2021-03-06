using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Versioning.Attributes;
using Versioning.Constraints;

namespace Versioning.Conventions
{
    public class HeaderActionVersionConvention : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            var versions = action.Attributes.OfType<VersionAttribute>()
                .Select(x => x.Version);

            if (!versions.Any())
                return;

            foreach (var selector in action.Selectors)
            {
                var headerConstraints = selector.ActionConstraints.OfType<HeaderConstraint>();

                if (!headerConstraints.Any())
                    selector.ActionConstraints.Add(new HeaderConstraint(versions));
                else
                {
                    var constraint = headerConstraints.FirstOrDefault();
                    selector.ActionConstraints.Remove(constraint);
                    selector.ActionConstraints.Add(new HeaderConstraint(constraint.Versions.Concat(versions)));
                }
            }
        }
    }
}