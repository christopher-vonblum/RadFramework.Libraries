using RadFramework.Libraries.Serialization.Json;
using RadFramework.Libraries.TextTranslation.Abstractions;

namespace RadFramework.Libraries.TextTranslation.Loaders
{
    public class TranslationDictionaryFileLoader : ITranslationDictionaryLoader
    {
        private readonly string _path;

        public TranslationDictionaryFileLoader(string path)
        {
            _path = path;
        }
        
        public IEnumerable<TranslationDictionary> LoadDictionaries()
        {
            return (IEnumerable<TranslationDictionary>)JsonContractSerializer.Instance.DeserializeFromJsonString(typeof(TranslationDictionary[]), File.ReadAllText(_path));
        }
    }
}