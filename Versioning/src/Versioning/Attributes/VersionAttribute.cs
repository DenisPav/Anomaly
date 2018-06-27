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
}