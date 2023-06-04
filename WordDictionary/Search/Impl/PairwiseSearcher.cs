using LemmaSharp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordDictionary.DictionaryCreation.Entity;
using WordDictionary.Search.Entity;

namespace WordDictionary.Search.Impl
{
	public class PairwiseSearcher
	{
		private readonly InvertedIndexSearcher boolSearcher;
		private readonly Lemmatizer lemmatizer;

		public PairwiseSearcher(InvertedIndexSearcher boolSearcher, Lemmatizer lemmatizer)
		{
			this.boolSearcher = boolSearcher;
			this.lemmatizer = lemmatizer;
		}

		public int[] SearchDocuments(string phraseToSearch, DocumentDictionary dictionary)
		{
			var words = phraseToSearch.Split(' ')
				.Select(w => lemmatizer.Lemmatize(w.ToLower()))
				.ToArray();

			var commands = new List<SearchOperation>();
			for (int i = 0; i < words.Length-1; i++)
			{
				var pair = $"{words[i]} {words[i + 1]}";
				if(i == 0)
					commands.Add(new SearchOperation() { Word = pair, Operation = BoolOperationEnum.None });
				else
					commands.Add(new SearchOperation() { Word = pair, Operation = BoolOperationEnum.And });
			}

			return boolSearcher.SearchOnMatrix(commands, dictionary);
		}
	}
}
