using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Versioning.Attributes;
using Versioning.Constraints;

namespace Versioning.Conventions
{
    public class HeaderControllerVersionConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var versions = controller.Attributes.OfType<VersionAttribute>()
                .Select(x => x.Version);

            if (!versions.Any())
                return;

            foreach (var action in controller.Actions)
                foreach (var selector in action.Selectors)
                    selector.ActionConstraints.Add(new HeaderConstraint(versions));
        }
    }
}