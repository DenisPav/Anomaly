using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Versioning.Attributes;

namespace Versioning.Conventions
{
    public class HeaderVersionConvention : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            var versions = action.Attributes.OfType<VersionAttribute>()
                .Select(x => x.Version);

            if (!versions.Any())
                return;

            foreach (var selector in action.Selectors)
            {
                selector.ActionConstraints.Add(new HeaderConstraint(versions.ToArray()));
            }
        }

        
    }

    public class HeaderControllerVersionConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var versions = controller.Attributes.OfType<VersionAttribute>()
                .Select(x => x.Version);

            if (!versions.Any())
                return;

            foreach (var selector in controller.Selectors)
            {
                selector.ActionConstraints.Add(new HeaderConstraint(versions.ToArray()));
            }
        }
    }
}