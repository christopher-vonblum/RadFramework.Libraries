using System.Globalization;
using RadFramework.Libraries.TextTranslation.Abstractions;

namespace RadFramework.Libraries.TextTranslation
{
    public class TranslationProvider : ITranslationProvider
    {
        private readonly IEnumerable<CultureInfo> _fallbackCultures;
        private Dictionary<CultureInfo, TranslationDictionary> dictionary;
        
        
        public TranslationProvider(params ITranslationDictionaryLoader[] loaders) : this(new CultureInfo[0], loaders)
        {
        }
        
        public TranslationProvider(IEnumerable<CultureInfo> fallbackCultures, params ITranslationDictionaryLoader[] loaders)
        {
            _fallbackCultures = fallbackCultures;
            
            dictionary = loaders
                .SelectMany(l => l.LoadDictionaries())
                .GroupBy(d => d.Culture)
                .Select(d =>
                    new TranslationDictionary
                    {
                        Culture = d.Key,
                        Dictionary = d
                            .SelectMany(v => v.Dictionary)
                            .ToDictionary(k => k.Key, v => v.Value)
                    })
                .ToDictionary(k => k.Culture);
        }
        
        public string Translate(string key)
        {
            CultureInfo resolved = ResolveCulture(key, CultureInfo.CurrentUICulture);

            if (resolved == null)
            {
                return key;
            }

            return dictionary[resolved].Dictionary[key];
        }
        
        public string Translate(string key, CultureInfo cultureInfo)
        {
            CultureInfo resolved = ResolveCulture(key, cultureInfo);

            if (resolved == null)
            {
                return key;
            }

            return dictionary[resolved].Dictionary[key];
        }

        private CultureInfo ResolveCulture(string key, CultureInfo cultureInfo)
        {
            if (dictionary.ContainsKey(cultureInfo)
                && dictionary[cultureInfo].Dictionary.ContainsKey(key))
            {
                return cultureInfo;
            }

            if (dictionary.ContainsKey(cultureInfo.Parent)
                && dictionary[cultureInfo.Parent].Dictionary.ContainsKey(key))
            {
                return cultureInfo.Parent;
            }

            foreach (var culture in _fallbackCultures)
            {
                if (dictionary.ContainsKey(culture) &&
                    dictionary[culture].Dictionary.ContainsKey(key))
                {
                    return culture;
                }
            }

            return null;
        }
    }
}