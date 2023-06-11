using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordDictionary.DictionaryCreation.Impl;

namespace WordDictionary.JokerSearch.ThreeGramIndex
{
	internal class KGramIndexDictionaryCreator
	{
		private readonly DictionaryCreator dictionaryCreator;

		public KGramIndexDictionaryCreator(DictionaryCreator dictionaryCreator)
		{
			this.dictionaryCreator = dictionaryCreator;
		}

		public async Task<ThreeGramIndex> BuildIndex(params string[] fileNames)
		{
			var index = new ThreeGramIndex();
			index.DocumentDictionary = await dictionaryCreator.CreateDictionary(fileNames);

			var kGramSet = new HashSet<string>();
			foreach (var word in index.DocumentDictionary.Dictionary) 
			{
				var kgrams = SplitIntoThreeGrams(word);
				for(var i = 0; i < kgrams.Length; i++)
				{
					if (!kGramSet.Contains(kgrams[i])) 
					{
						index.KGramIndex[kgrams[i]] = new LinkedList<string>();
					}
					index.KGramIndex[kgrams[i]].AddLast(word);
					kGramSet.Add(kgrams[i]);
				}
			}

			return index;
		}

		public string[] SplitIntoThreeGrams(string word, bool addStarts = true)
		{
			if(addStarts) word = $"||{word}||";

			var resLength = word.Length - 2;
			var res = new string[resLength];

			for(int i = 0; i < resLength; i++)
			{
				res[i] = word.Substring(i, 3);
			}
			
			return res;
		}
	}
}
