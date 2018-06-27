using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace Versioning.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class VersionAttribute : Attribute
    {
        public string Version { get; private set; }
        public VersionAttribute(string version)
        {
            this.Version = version;
        }
    }

    public class HeaderConstraint : IActionConstraint
    {
        public IEnumerable<string> Versions { get; private set; }
        public Lazy<IEnumerable<string>> VersionHeaderValues { get; private set; }

        public HeaderConstraint(params string[] versions)
        {
            Versions = versions;
            VersionHeaderValues = new Lazy<IEnumerable<string>>(() => Versions.Select(x => $"application/v{x}+json"));
        }
        public int Order => 1;

        public bool Accept(ActionConstraintContext context)
        {
            if (context.RouteContext.HttpContext.Request.Headers.TryGetValue("Accept", out var val))
            {
                return VersionHeaderValues.Value.Any(x => x == val);
            }
            return false;
        }
    }
}