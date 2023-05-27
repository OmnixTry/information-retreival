using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordDictionary.DictionaryCreation.Entity
{
	public class DocumentDictionary
	{
		public List<DocumentEntity> Documents { get; set; }
		public SortedSet<string> Dictionary { get; set; }
		public Dictionary<string, LinkedList<int>> Index { get; set; }
	}
}
