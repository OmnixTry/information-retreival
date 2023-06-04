using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordDictionary.Search.Entity
{
	public class PositionalSearchOperation
	{
		public string Word1 { get; set; }
		public string Word2 { get; set; }
		public int Distance { get; set; }
	}
}
