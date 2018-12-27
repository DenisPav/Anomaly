using System;

namespace Tenancy.Common.DI
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TransientServiceAttribute : Attribute
    {

    }
}
