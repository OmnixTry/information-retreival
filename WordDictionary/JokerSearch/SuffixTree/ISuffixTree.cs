using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordDictionary.JokerSearch.SuffixTree
{
	internal interface ISuffixTree
	{
		ISet<int> Search(string word);
	}
}
