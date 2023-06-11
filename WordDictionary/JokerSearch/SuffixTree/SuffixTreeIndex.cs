using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordDictionary.DictionaryCreation.Entity;

namespace WordDictionary.JokerSearch.SuffixTree
{
	internal class SuffixTreeIndex
	{
		public List<DocumentEntity> Documents { get; set; }
		public Dictionary<int, SuffixTree> SuffixTrees { get; set;} = new Dictionary<int, SuffixTree>();
	}
}
