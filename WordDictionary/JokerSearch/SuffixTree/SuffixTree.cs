using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WordDictionary.JokerSearch.SuffixTree
{
	public class SuffixTree : ISuffixTree
	{
		public int HighestIndex = -1;

		private readonly Node _root = new Node();

		private Node _activeLeaf;

		public SuffixTree()
		{
			_activeLeaf = _root;
		}

		public ISet<int> Search(string word)
		{
			var tmpNode = SearchNode(word);
			if (tmpNode == null)
			{
				return new HashSet<int>();
			}

			var ret = new HashSet<int>();
			tmpNode.GetData(ret);
			return ret;
		}

		private Node SearchNode(string word)
		{
			var currentNode = _root;

			for (var i = 0; i < word.Length; ++i)
			{
				var ch = word[i];
				if (!currentNode.Edges.TryGetValue(ch, out var currentEdge))
				{
					return null;
				}

				var label = currentEdge.Label;
				var lenToMatch = Math.Min(word.Length - i, label.Length);
				if (word.Substring(i, lenToMatch) != label.Substring(0, lenToMatch))
				{
					return null;
				}

				if (label.Length >= word.Length - i)
					return currentEdge.Dest;

				currentNode = currentEdge.Dest;
				i += lenToMatch - 1;
			}

			return null;
		}

		public void Put(string key, int index)
		{
			if (index < HighestIndex)
			{
				throw new IndexOutOfRangeException(
					"The input index must not be less than any of the previously inserted ones. Got " + index +
					", expected at least " + HighestIndex);
			}

			HighestIndex = index;

			_activeLeaf = _root;

			var s = _root;

			var text = "";
			for (var i = 0; i < key.Length; i++)
			{
				(s, text) = Update(s, text, key[i], key.Substring(i), index);
			}

			if (null == _activeLeaf.Suffix && _activeLeaf != _root && _activeLeaf != s)
			{
				_activeLeaf.Suffix = s;
			}
		}

		private (bool, Node) TestAndSplit(Node inputs, string stringPart, char t, string remainder, int value)
		{
			var ret = canonize(inputs, stringPart);
			var s = ret.Item1;
			var str = ret.Item2;

			if (str != "")
			{
				var g = s.Edges[str[0]];
				var label = g.Label;
				if (label.Length > str.Length && label[str.Length] == t)
				{
					return (true, s);
				}

				var newLabel = label.Substring(str.Length);
				if (!label.StartsWith(str, StringComparison.Ordinal)) throw new Exception();

				var r = new Node();
				var newEdge = new Edge(str, r);

				g.Label = newLabel;

				r.Edges[newLabel[0]] = g;
				s.Edges[str[0]] = newEdge;

				return (false, r);
			}

			if (!s.Edges.TryGetValue(t, out var e))
			{
				return (false, s);
			}

			if (remainder == e.Label)
			{
				e.Dest.AddRef(value);
				return (true, s);
			}

			if (remainder.StartsWith(e.Label, StringComparison.Ordinal))
			{
				return (true, s);
			}

			if (e.Label.StartsWith(remainder, StringComparison.Ordinal))
			{
				var newNode = new Node();
				newNode.AddRef(value);

				var newEdge = new Edge(remainder, newNode);

				e.Label = e.Label.Substring(remainder.Length);

				newNode.Edges[e.Label[0]] = e;

				s.Edges[t] = newEdge;

				return (false, s);
			}

			return (true, s);
		}

		private (Node, string) canonize(Node s, string inputstr)
		{
			if (inputstr == "")
			{
				return (s, inputstr);
			}
			else
			{
				var currentNode = s;
				var str = inputstr;
				var g = s.Edges[str[0]];

				while (g != null && str.StartsWith(g.Label, StringComparison.Ordinal))
				{
					str = str.Substring(g.Label.Length);
					currentNode = g.Dest;
					if (str.Length > 0)
					{
						g = currentNode.Edges[str[0]];
					}
				}

				return (currentNode, str);
			}
		}

		private (Node, string) Update(Node inputNode, string stringPart, char newChar, string rest, int value)
		{
			var s = inputNode;
			var tempstr = stringPart + newChar;

			var oldroot = _root;

			var (endpoint, r) = TestAndSplit(s, stringPart, newChar, rest, value);

			while (!endpoint)
			{
				Node leaf;
				if (r.Edges.TryGetValue(newChar, out var tempEdge))
				{
					leaf = tempEdge.Dest;
				}
				else
				{
					leaf = new Node();
					leaf.AddRef(value);
					var newedge = new Edge(rest, leaf);
					r.Edges[newChar] = newedge;
				}
				if (_activeLeaf != _root)
				{
					_activeLeaf.Suffix = leaf;
				}

				_activeLeaf = leaf;

				if (oldroot != _root)
				{
					oldroot.Suffix = r;
				}

				oldroot = r;

				if (null == s.Suffix)
				{
					if (_root != s) throw new Exception("Root node is not s");
					tempstr = tempstr.Substring(1);
				}
				else
				{
					string tempstr1;
					(s, tempstr1) = canonize(s.Suffix, SafeCutLastChar(tempstr));
					tempstr = (tempstr1 + tempstr[tempstr.Length - 1]);
				}
				(endpoint, r) = TestAndSplit(s, SafeCutLastChar(tempstr), newChar, rest, value);
			}
			if (oldroot != _root)
			{
				oldroot.Suffix = r;
			}

			return canonize(s, tempstr);
		}

		private static string SafeCutLastChar(string seq)
		{
			return seq.Length == 0 ? "" : seq.Substring(0, seq.Length - 1);
		}
	}
}
