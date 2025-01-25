using System.Reflection;
using RadFramework.Libraries.Serialization.Json.ContractSerialization;
using RadFramework.Libraries.TextTranslation.Abstractions;

namespace RadFramework.Libraries.TextTranslation.Loaders
{
    public class TranslationDictionaryEmbeddedResourceLoader : ITranslationDictionaryLoader
    {
        private readonly Assembly _assembly;
        private readonly string _resourceName;

        public TranslationDictionaryEmbeddedResourceLoader(Assembly assembly, string resourceName)
        {
            _assembly = assembly;
            _resourceName = resourceName;
        }
        
        public IEnumerable<TranslationDictionary> LoadDictionaries()
        {
            using (StreamReader sr = new StreamReader(_assembly.GetManifestResourceStream(_resourceName)))
            {
                return (IEnumerable<TranslationDictionary>)JsonContractSerializer.Instance.DeserializeFromJsonString(typeof(TranslationDictionary[]), sr.ReadToEnd());
            }
        }
    }
}