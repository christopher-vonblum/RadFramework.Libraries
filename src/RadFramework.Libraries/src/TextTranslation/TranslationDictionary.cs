using System.Globalization;

namespace RadFramework.Libraries.TextTranslation
{
    public class TranslationDictionary
    {
        public CultureInfo Culture { get; set; }
        public Dictionary<string, string> Dictionary { get; set; }
    }
}