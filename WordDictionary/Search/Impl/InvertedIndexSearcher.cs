using LemmaSharp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordDictionary.Search.Entity;

namespace WordDictionary.Search.Impl
{
	public class InvertedIndexSearcher : SearcherBase<LinkedList<int>>
	{
		public InvertedIndexSearcher(Lemmatizer lemmatizer) : base(lemmatizer)
		{
		}

		protected override int[] ElicitResultingDocuments()
		{
			return context.CurrentResult.ToArray();
		}

		protected override LinkedList<int> InvertArray(LinkedList<int> arr)
		{
			var otherDocs = context.DocumentDictionary.Documents.Where(d => !arr.Contains(d.Id)).Select(d => d.Id).ToArray();
			return new LinkedList<int>(otherDocs);
		}

		protected override void ProcessAnd(SearchOperation operation)
		{
			LoadOperand(operation);
			var left = context.CurrentResult.First;
			var right = context.SecondOperand.First;

			var newResult = new LinkedList<int>();
			while (left != null && right != null)
			{
				if(left.Value == right.Value)
				{
					newResult.AddLast(left.Value);
					left = left.Next;
					right = right.Next;
				}
				else if (left.Value < right.Value) {
					left = left.Next;
				}
				else
				{
					right = right.Next;
				}
			}

			context.CurrentResult = newResult;
		}

		protected override void ProcessOr(SearchOperation operation)
		{
			LoadOperand(operation);
			var left = context.CurrentResult.First;
			var right = context.SecondOperand.First;

			var newResult = new LinkedList<int>();
			//newResult.AddLast(Math.Min(left.Value, right.Value));
			
			//if(left.Value != right.Value) 
			//	newResult.AddLast(Math.Max(left.Value, right.Value));

			while (left != null && right != null)
			{
				if (left.Value == right.Value)
				{
					newResult.AddLast(left.Value);
					left = left.Next;
					right = right.Next;
				}
				else if (left.Value < right.Value)
				{
					newResult.AddLast(left.Value);
					left = left.Next;
				}
				else if (left.Value > right.Value)
				{
					newResult.AddLast(right.Value);
					right = right.Next;						
				}
			}

			while (left != null)
			{
				newResult.AddLast(left.Value);
				left = left.Next;
			}

			while (right != null)
			{
				newResult.AddLast(right.Value);
				right = right.Next;
			}

			context.CurrentResult = newResult;
		}

		protected override LinkedList<int> GetWordRow(string word)
		{
			base.GetWordRow(word);
			return context.DocumentDictionary.Index[word];
		}
	}
}
