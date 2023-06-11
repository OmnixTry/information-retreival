using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordDictionary.JokerSearch.SuffixTree
{
	public class Node
	{
		public readonly List<int> Data = new List<int>();
		public readonly Dictionary<char, Edge> Edges = new Dictionary<char, Edge>();
		public Node Suffix { get; set; } = null;

		public void GetData(ISet<int> ret)
		{
			Data.ForEach(x => ret.Add(x));

			foreach (var e in Edges.Values)
			{
				e.Dest.GetData(ret);
			}
		}

		public bool AddRef(int index)
		{
			if (Data.Contains(index))
			{
				return false;
			}

			Data.Add(index);

			var node = Suffix;
			while (node != null)
			{
				if (!node.Data.Contains(index))
				{
					node.Data.Add(index);
					node = node.Suffix;
				}
				else
				{
					break;
				}
			}

			return true;
		}

		public override string ToString()
		{
			return "Node: size:" + Data.Count + " Edges: " + Edges;
		}
	}
}
