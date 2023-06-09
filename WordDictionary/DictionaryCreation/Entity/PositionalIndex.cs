﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordDictionary.DictionaryCreation.Entity
{
	public class PositionalIndex
	{
		public List<DocumentEntity> Documents { get; set; }
		public ISet<string> Dictionary { get; set; }
		public Dictionary<string, LinkedList<(int docId, int[] positions)>> Index { get; set; }

	}
}
