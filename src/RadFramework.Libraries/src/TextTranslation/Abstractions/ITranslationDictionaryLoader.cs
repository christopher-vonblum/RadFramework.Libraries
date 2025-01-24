namespace RadFramework.Libraries.TextTranslation.Abstractions
{
    public interface ITranslationDictionaryLoader
    {
        IEnumerable<TranslationDictionary> LoadDictionaries();
    }
}