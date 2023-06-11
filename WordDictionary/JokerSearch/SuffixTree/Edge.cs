using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WordDictionary.JokerSearch.SuffixTree
{
	public class Edge
	{
		public string Label { get; set; }
		public Node Dest { get; internal set; }

		public Edge(string label, Node dest)
		{
			Label = label;
			Dest = dest;
		}
	}
}
