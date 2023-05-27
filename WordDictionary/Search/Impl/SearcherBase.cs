using LemmaSharp.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordDictionary.DictionaryCreation.Entity;
using WordDictionary.Search.Entity;

namespace WordDictionary.Search.Impl
{
    public abstract class SearcherBase <TOperand> where TOperand : IEnumerable
    {
		private Lemmatizer lemmatizer;
		protected BoolSearchContext<TOperand> context;


		public SearcherBase(Lemmatizer lemmatizer)
		{
			this.lemmatizer = lemmatizer;
		}

		public int[] SearchOnMatrix(string querry, DocumentDictionary dictionary)
		{
			context = new BoolSearchContext<TOperand>() { DocumentDictionary = dictionary };
			var operations = ParseQuerry(querry);

			foreach (var operation in operations)
			{
				switch (operation.Operation)
				{
					case BoolOperationEnum.None:
						ProcessNone(operation);
						break;
					case BoolOperationEnum.And:
						ProcessAnd(operation);
						break;
					case BoolOperationEnum.Or:
						ProcessOr(operation);
						break;
				}
			}

			return ElicitResultingDocuments();
		}

		private List<SearchOperation> ParseQuerry(string querry)
		{
			var splited = querry.Split(' ');
			List<SearchOperation> operations = new List<SearchOperation>();

			bool isFirstOperation = GetOperationFormText(splited.First(), out var firstOp);
			if (isFirstOperation && firstOp != BoolOperationEnum.Not) throw new Exception($"Operation {splited[0]} can't be in first place");
			if (isFirstOperation)
			{
				operations.Add(new SearchOperation() { Word = splited[1], Operation = BoolOperationEnum.None, IsInverted = true });
			}
			else
			{
				operations.Add(new SearchOperation() { Word = splited[0], Operation = BoolOperationEnum.None });
			}

			for (int i = 1; i < splited.Length; i += 2)
			{
				bool isInverted = false;
				var isOperation = GetOperationFormText(splited[i], out var operation);

				if (!isOperation)
					throw new Exception($"{splited[i]} is not a valid Operation");

				if (GetOperationFormText(splited[i + 1], out var secondOperation))
				{
					i++;
					if (secondOperation != BoolOperationEnum.Not) throw new Exception("Can't have 2 consecutive operations!");
					isInverted = true;
				}

				operations.Add(new SearchOperation() { Word = splited[i + 1], Operation = operation, IsInverted = isInverted });
			}
			return operations;
		}

		private bool GetOperationFormText(string text, out BoolOperationEnum operation)
		{
			switch (text)
			{
				case "AND":
					operation = BoolOperationEnum.And;
					break;
				case "OR":
					operation = BoolOperationEnum.Or;
					break;
				case "NOT":
					operation = BoolOperationEnum.Not;
					break;
				default:
					operation = BoolOperationEnum.None;
					return false;
			}

			return true;
		}

		protected virtual void ProcessNone(SearchOperation operation)
		{
			var row = GetWordRow(operation.Word);
			context.CurrentResult = row;
			if (operation.IsInverted)
				InvertArray(row);
		}
		protected virtual void LoadOperand(SearchOperation operation)
		{
			var row = GetWordRow(operation.Word);
			context.SecondOperand = row;
			if (operation.IsInverted)
				context.SecondOperand = InvertArray(row);
		}

		protected abstract int[] ElicitResultingDocuments();

		protected abstract void ProcessAnd(SearchOperation operation);

		protected abstract void ProcessOr(SearchOperation operation);

		protected abstract TOperand InvertArray(TOperand arr);

		protected virtual TOperand GetWordRow(string word)
		{
			if (!context.DocumentDictionary.Dictionary.Contains(word))
			{
				throw new Exception($"The word {word} was not found in the dictionary");
			}

			return default(TOperand);
		}
	}
}
