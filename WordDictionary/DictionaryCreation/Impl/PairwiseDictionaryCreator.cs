using LemmaSharp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordDictionary.DictionaryCreation.Entity;

namespace WordDictionary.DictionaryCreation.Impl
{
	public class PairwiseDictionaryCreator : DictionaryCreatorBase
	{
		public PairwiseDictionaryCreator(Lemmatizer lemmatizer) : base(lemmatizer)
		{

		}

		public async Task<DocumentDictionary> CreateDictionary(params string[] fileNames)
		{
			var documents = await IdDocuments(fileNames);

			var dictionary = new DocumentDictionary();
			dictionary.Documents = documents;

			foreach (var document in documents)
			{
				document.UniqueWords = BreakParagraphsIntoPairs(document);
			}

			dictionary.Dictionary = CombineDocumentsIntoDictionary(documents);

			dictionary.Index = CombineDocumentsIntoIndex(dictionary.Dictionary, documents);

			return dictionary;
		}

		private Dictionary<string, LinkedList<int>> CombineDocumentsIntoIndex(ISet<string> dict, List<DocumentEntity> entities)
		{
			var dictionary = new Dictionary<string, LinkedList<int>>(dict.Count, StringComparer.InvariantCultureIgnoreCase);

			foreach (var word in dict)
			{
				if(word != null)
					dictionary[word] = new LinkedList<int>();
			}

			foreach (var docEntity in entities)
			{
				foreach (var word in docEntity.UniqueWords)
				{
					dictionary[word].AddLast(docEntity.Id);
				}
			}

			return dictionary;
		}

		protected string[] BreakParagraphsIntoPairs(DocumentEntity document)
		{
			var allWords = AllWords(document.DocumentWords);
			var len = allWords.Length - 1;
			var pairs = new string[len];

			for (int i = 0; i < allWords.Length - 1; i++)
			{
				var pair = $"{allWords[i]} {allWords[i + 1]}";
				pairs[i] = pair;
			}

			return pairs.Distinct().ToArray();
		}
	}
}
