using WordDictionary.DictionaryCreation.Entity;

namespace WordDictionary.DocumentReaders.Contract
{
    public interface IDictionarySaver
    {
		Task<DocumentDictionary> ReadFile(string fileName);
        Task SaveFile(DocumentDictionary document, string fileName);
    }
}