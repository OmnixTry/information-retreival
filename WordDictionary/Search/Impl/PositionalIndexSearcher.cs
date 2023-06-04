using LemmaSharp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordDictionary.DictionaryCreation.Entity;
using WordDictionary.Search.Entity;

namespace WordDictionary.Search.Impl
{
	public class PositionalIndexSearcher
	{
		private readonly Lemmatizer lemmatizer;

		public PositionalIndexSearcher(Lemmatizer lemmatizer)
		{
			this.lemmatizer = lemmatizer;
		}

		public List<int> SearchByQuerry(string querry, PositionalIndex index, out List<(int docId, int index)> pp1Index)
		{
			var operations = ParseQuerry(querry);

			List<int> prevDocs = null;

			foreach (var op in operations)
			{
				prevDocs = SearchCommand(operations.First(), index, out pp1Index, prevDocs);
			}

			return SearchCommand(operations.First(), index, out pp1Index);
		}

		public List<int> SearchCommand(PositionalSearchOperation operation, PositionalIndex index, out List<(int docId, int index)> pp1Index, List<int> prevDocs = null)
		{
			var docsWord1 = index.Index[operation.Word1].Where(d => prevDocs == null || prevDocs.Contains(d.docId));
			var docsWord2 = index.Index[operation.Word2].Where(d => prevDocs == null || prevDocs.Contains(d.docId));

			var lessFrequent = docsWord1;
			var moreFrequent = docsWord2;

			List<int> documents = new List<int>();
			pp1Index = new List<(int docId, int index)>();

			if(lessFrequent.Count() > moreFrequent.Count())
			{
				var buff = lessFrequent;
				lessFrequent = moreFrequent;
				moreFrequent = buff;
			}

			using(var p1 = lessFrequent.GetEnumerator()) 
			using(var p2 = moreFrequent.GetEnumerator())
			{
				var hasElementsP1 = p1.MoveNext();
				var hasElementsP2 = p2.MoveNext();

				while(hasElementsP1 && hasElementsP2)
				{
					if (p1.Current.docId == p2.Current.docId)
					{
						var pp1 = p1.Current.positions;
						var pp2 = p2.Current.positions;
						for (var i = 0; i < pp1.Length; i++)
						{
							for (var j = 0; j < pp2.Length; j++)
							{
								if (Math.Abs(pp1[i] - pp2[j]) <= operation.Distance)
								{
									documents.Add(p1.Current.docId);
									pp1Index.Add((p1.Current.docId, pp1[i]));
									break;
								}
								if (pp2[j] > pp1[i])
								{
									break;
								}
							}
						}

						hasElementsP1 = p1.MoveNext();
						hasElementsP2 = p2.MoveNext();
					}
					else if (p1.Current.docId < p2.Current.docId)
					{
						hasElementsP1 = p1.MoveNext();
					}
					else
					{
						hasElementsP2 = p2.MoveNext();
					}
				}
			}

			return documents;
		}

		public PositionalSearchOperation[] ParseQuerry(string querry)
		{
			var split = querry.Split(' ').ToArray();

			var operations = new List<PositionalSearchOperation>();
			for (int i = 0; i < split.Length; i+=2)
			{
				if (i == split.Length - 1) break;
				var operation = new PositionalSearchOperation()
				{
					Word1 = lemmatizer.Lemmatize(split[i]),
					Word2 = lemmatizer.Lemmatize(split[i+2]),
					Distance = int.Parse(split[i+1].Substring(1)),
				};
				operations.Add(operation);
			}

			return operations.ToArray();
		}
	}
}
