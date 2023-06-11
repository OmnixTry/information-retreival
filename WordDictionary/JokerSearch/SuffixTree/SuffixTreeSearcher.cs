using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordDictionary.JokerSearch.SuffixTree
{
	internal class SuffixTreeSearcher
	{
		private readonly SuffixTreeIndex index;

		public SuffixTreeSearcher(SuffixTreeIndex index)
		{
			this.index = index;
		}

		public List<(int documentId, List<string> segments)> Search(string suffix)
		{
			var answers = new List<(int documentId, ISet<int> positions)>();
			foreach (var doc in index.Documents)
			{
				var res = index.SuffixTrees[doc.Id].Search(suffix);
				if(res.Count == 0) continue;
				answers.Add((doc.Id, res));
			}
			
			var answersFound = new List<(int documentId, List<string> segments)>();

			foreach (var answer in answers)
			{
				var doc = index.Documents.First(d => d.Id == answer.documentId);
				var newAns = (answer.documentId, new List<string>());
				foreach (var position in answer.positions)
				{
					newAns.Item2.Add(string.Join(' ', doc.AllWords.Skip(position-3).Take(6)));
				}

				answersFound.Add(newAns);
			}

			return answersFound;
		}
	}
}
