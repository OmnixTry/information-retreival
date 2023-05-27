using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordDictionary.DictionaryCreation.Entity
{
	public class WordMatrix
	{
		public Dictionary<string, int> WordRows { get; set; }
		public bool[][] Matrix { get; set; }
	}
}
