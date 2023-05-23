namespace RadFramework.Libraries.Core.Extensions
{
    public static class StringExtensions
    {
        public static string EnsureSinglePrefix(this string value, string prefix)
        {
            return value.EnsureNoPrefixes(prefix).EnsurePrefix(prefix);
        }

        public static string EnsurePrefix(this string value, string prefix)
        {
            if (!value.StartsWith(prefix))
            {
                return prefix + value;
            }

            return value;
        }

        public static string EnsureSingleSuffix(this string value, string suffix)
        {
            return value.EnsureNoSuffixes(suffix).EnsureSuffix(suffix);
        }

        public static string EnsureSuffix(this string value, string suffix)
        {
            if (!value.EndsWith(suffix))
            {
                return value + suffix;
            }

            return value;
        }

        public static string EnsureNoPrefix(this string value, string prefix)
        {
            if (value.StartsWith(prefix))
            {
                return value.Substring(prefix.Length);
            }

            return value;
        }

        public static string EnsureNoPrefixes(this string value, string prefix)
        {
            string removePrefixes = value;

            while (removePrefixes.StartsWith(prefix))
            {
                removePrefixes = removePrefixes.EnsureNoPrefix(prefix);
            }

            return removePrefixes;
        }

        public static string EnsureNoSuffix(this string value, string suffix)
        {
            if (value.EndsWith(suffix))
            {
                return value.Substring(0, value.Length - suffix.Length);
            }

            return value;
        }

        public static string EnsureNoSuffixes(this string value, string suffix)
        {
            string removeSuffixes = value;

            while (removeSuffixes.EndsWith(suffix))
            {
                removeSuffixes = removeSuffixes.EnsureNoSuffix(suffix);
            }

            return removeSuffixes;
        }
    }
}