using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordDictionary.DictionaryCreation.Impl;

namespace WordDictionary.JokerSearch.Combinations
{
	internal class CombinationIndexDictionaryCreator
	{
		private readonly DictionaryCreator dictionaryCreator;

		public CombinationIndexDictionaryCreator(DictionaryCreator dictionaryCreator)
		{
			this.dictionaryCreator = dictionaryCreator;
		}

		public async Task<CombinationIndex> GenerateIndex(params string[] fileNames)
		{
			var index = new CombinationIndex();
			index.DocumentDictionary = await dictionaryCreator.CreateDictionary(fileNames);
			index.Words = index.DocumentDictionary.Dictionary.ToList();
			index.Words.Sort();
			index.Index = new List<(string combination, int wordId)>();

			for (int i = 0; i < index.Words.Count; i++)
			{
				index.Index.AddRange(GetCombinations(index.Words[i], i));
			}

			return index;
		}

		private IEnumerable<(string combination, int wordId)> GetCombinations(string word, int wordId)
		{
			word += "|";

			var chars = new char[word.Length];
			word.CopyTo(chars);
			var words = new string[chars.Length];
			for (int i = 0; i < chars.Length; i++)
			{
				words[i] = string.Concat(chars);
				char buff = chars[chars.Length - 1];
				for (int j = chars.Length - 2; j >= 0; j--)
				{
					chars[j + 1] = chars[j];
				}
				chars[0] = buff;
			}

			return words.Select(w => (w, wordId)).ToList();
		}
	}
}
