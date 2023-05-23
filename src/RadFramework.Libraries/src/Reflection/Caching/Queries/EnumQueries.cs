using System;
using System.Collections.Generic;

namespace RadFramework.Libraries.Reflection.Caching.Queries
{
    public static class EnumQueries
    {
        public static string[] GetEnumValues(Type @enum)
        {
            List<string> stringVals = new List<string>();

            foreach (object value in @enum.GetEnumValues())
            {
                stringVals.Add(value.ToString());
            }

            return stringVals.ToArray();;
        }
    }
}