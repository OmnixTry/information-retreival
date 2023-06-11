using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WordDictionary.Search.Entity;
using WordDictionary.Search.Impl;

namespace WordDictionary.JokerSearch.ThreeGramIndex
{
	internal class KGramSearcher
	{
		private readonly InvertedIndexSearcher searcher;
		private readonly KGramIndexDictionaryCreator dictionaryCreator;
		private readonly ThreeGramIndex index;

		public KGramSearcher(InvertedIndexSearcher searcher, KGramIndexDictionaryCreator dictionaryCreator, ThreeGramIndex index) 
		{
			this.searcher = searcher;
			this.dictionaryCreator = dictionaryCreator;
			this.index = index;
		}

		public async Task<int[]> Search(string jokeredString)
		{
			var cleanSplit = jokeredString.Split('*');
			jokeredString = $"||{jokeredString}||";
			string[] splitByJoker = jokeredString.Split('*');

			var threegrams = splitByJoker.Where(s => s.Length >=3 ).SelectMany(s => dictionaryCreator.SplitIntoThreeGrams(s, false)).ToArray();

			var left = index.KGramIndex[threegrams[0]];
			LinkedList<string> right;
			for (int i = 1; i < threegrams.Length; i++)
			{
				right = index.KGramIndex[threegrams[i]];
				left = LookForWords(left, right);
			}

			var res = left.Where(w => ContainsAllParts(w, cleanSplit)).ToList();

			Console.WriteLine("Found Words:");
			foreach (var item in res)
			{
				Console.WriteLine(item);
			}

			var commands = res.Select(w => new SearchOperation() { Operation = BoolOperationEnum.Or, Word = w }).ToList();
			commands.First().Operation = BoolOperationEnum.None;

			return searcher.SearchOnMatrix(commands, index.DocumentDictionary);
		}

		public LinkedList<string> LookForWords(LinkedList<string> leftList, LinkedList<string> rightList)
		{
			var left = leftList.First;
			var right = rightList.First;

			var newResult = new LinkedList<string>();
			while (left != null && right != null)
			{
				if (left.Value == right.Value)
				{
					newResult.AddLast(left.Value);
					left = left.Next;
					right = right.Next;
				}
				else if (string.Compare(left.Value, right.Value, true) < 0) // <
				{
					left = left.Next;
				}
				else
				{
					right = right.Next;
				}
			}

			return newResult;
		}

		private bool ContainsAllParts(string word, string[] parts)
		{
			foreach (var part in parts) 
			{
				if (!word.Contains(part)) return false;
			}
			return true;
		}
	}
}
