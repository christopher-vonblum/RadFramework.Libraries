using System;
using System.Linq;
using System.Reflection;

namespace RadFramework.Libraries.Reflection.Caching.Queries
{
    public static class AttributeLocationQueries
    {
        public static Attribute[] GetInheritedAttributes(ICustomAttributeProvider location)
        {
            return location.GetCustomAttributes(true).Select(a => (Attribute) a).ToArray();
        }
        public static Attribute[] GetAttributes(ICustomAttributeProvider location)
        {
            return location.GetCustomAttributes(false).Select(a => (Attribute) a).ToArray();
        }
    }
}