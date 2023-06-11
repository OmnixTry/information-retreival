using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WordDictionary.DocumentReaders.Entity;
using WordDictionary.Search.Entity;
using WordDictionary.Search.Impl;

namespace WordDictionary.JokerSearch.Combinations
{
	internal class CombinationIndexSearch
	{
		private readonly CombinationIndex index;
		private readonly InvertedIndexSearcher indexSearcher;

		public CombinationIndexSearch(CombinationIndex index, InvertedIndexSearcher indexSearcher)
		{
			this.index = index;
			this.indexSearcher = indexSearcher;
		}

		public async Task<int[]> SearchDocuments(string jokeredString)
		{
			jokeredString += '|';
			var rotated = RotateStringForJoker(jokeredString);

			var words = new HashSet<int>();
			foreach (var combination in index.Index)
			{
				if (combination.combination.StartsWith(rotated))
				{
					words.Add(combination.wordId);
				}
			}

			foreach (var word in words)
			{
				Console.WriteLine(index.Words[word]);
			}

			var operations = words.Select(w => new SearchOperation() 
			{ 
				IsInverted = false, 
				Operation = BoolOperationEnum.Or, 
				Word = index.Words[w] 
			}).ToList();
			operations.First().Operation = BoolOperationEnum.None;
			var res = indexSearcher.SearchOnMatrix(operations, index.DocumentDictionary);

			return res;
		}

		public string RotateStringForJoker(string jokerString)
		{
			var chars = new char[jokerString.Length];
			jokerString.CopyTo(chars);
			
			while (chars.Last() != '*')
			{
				char buff = chars[chars.Length - 1];
				for (int i = chars.Length - 2; i >= 0; i--)
				{
					chars[i + 1] = chars[i];
				}
				chars[0] = buff;
			}

			return string.Concat(chars).Substring(0, chars.Length-1);
		}
	}
}
