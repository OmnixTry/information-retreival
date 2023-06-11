using FB2Library.Elements;
using LemmaSharp.Classes;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WordDictionary.DictionaryCreation.Entity;

namespace WordDictionary.DictionaryCreation.Impl
{
	public class PositionalDictionaryCreator : DictionaryCreatorBase
	{
		public PositionalDictionaryCreator(Lemmatizer lemmatizer) : base(lemmatizer)
		{
		}

		public async Task<PositionalIndex> CreateDictionary(params string[] fileNames)
		{
			var index = new PositionalIndex();
			var documents = await ReadPositionalDocuments(true, fileNames);
			index.Documents = documents;

			var dictionarySet = CombineDocumentsIntoDictionary(documents);
			index.Dictionary = dictionarySet;
			index.Index = CombineDocumentsIntoPoitionalIndex(dictionarySet, documents); 
			
			return index;
		}

		public async Task<List<DocumentEntity>> ReadPositionalDocuments(bool index, params string[] fileNames)
		{
			var documents = await IdDocuments(fileNames);

			foreach (var document in documents)
			{
				var allWords = AllWords(document.DocumentWords);

				document.AllWords = allWords;
				document.UniqueWords = UniqueWords(document.DocumentWords, out int wordCount);

				if(index)
				{
					(string key, int[] positions)[] indexedWords = allWords.Select((word, i) => (word, i))
					.GroupBy(w => w.word)
					.Select(g => (g.Key, g.Select(t => t.i).ToArray()))
					.ToArray();
					document.IndexedWords = indexedWords;
				}				
			}

			return documents;
		}

		protected Dictionary<string, LinkedList<(int docId, int[] positions)>> CombineDocumentsIntoPoitionalIndex(ISet<string> dict, List<DocumentEntity> documents)
		{
            var dictionary = new Dictionary<string, LinkedList<(int docId, int[] positions)>>(dict.Count, StringComparer.InvariantCultureIgnoreCase);

			foreach (var word in dict)
			{
				dictionary[word] = new LinkedList<(int docId, int[] positions)>();
			}

			foreach (var docEntity in documents)
			{
				foreach (var word in docEntity.IndexedWords)
				{
					dictionary[word.word].AddLast((docEntity.Id, word.positions));
				}
			}

			return dictionary;
		}
	}
}
