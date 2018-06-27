using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace Versioning.Constraints
{
    public class HeaderConstraint : IActionConstraint
    {
        const string HeaderFormat = "application/v{0}+json";
        public IEnumerable<string> Versions { get; private set; }
        public Lazy<IEnumerable<string>> VersionHeaderValues { get; private set; }

        public HeaderConstraint(IEnumerable<string> versions)
        {
            Versions = versions;
            VersionHeaderValues = new Lazy<IEnumerable<string>>(() => Versions.Select(x => string.Format(HeaderFormat, x)));
        }
        public int Order => 1;

        public bool Accept(ActionConstraintContext context)
        {
            if (context.RouteContext.HttpContext.Request.Headers.TryGetValue("Accept", out var val))
                return VersionHeaderValues.Value.Any(x => x == val);
                
            return false;
        }
    }
}