using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordDictionary.DictionaryCreation.Entity;

namespace WordDictionary.JokerSearch.Combinations
{
	internal class CombinationIndex
	{
		public DocumentDictionary DocumentDictionary { get; set; }

		public List<(string combination, int wordId)> Index { get; set; }

		public List<string> Words { get; set; }
	}
}
