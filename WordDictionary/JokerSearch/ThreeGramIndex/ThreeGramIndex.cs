
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordDictionary.DictionaryCreation.Entity;

namespace WordDictionary.JokerSearch.ThreeGramIndex
{
	internal class ThreeGramIndex
	{
		public DocumentDictionary DocumentDictionary {  get; set; }
		public Dictionary<string, LinkedList<string>> KGramIndex { get; set; } = new Dictionary<string, LinkedList<string>>();
	}
}
