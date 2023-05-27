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
    public class BoolSearcher : SearcherBase<bool[]>
    {
        public BoolSearcher(Lemmatizer lemmatizer) : base(lemmatizer)
        {
        }

        protected override void ProcessAnd(SearchOperation operation)
        {
            LoadOperand(operation);
            for (int i = 0; i < context.CurrentResult.Length; i++)
            {
                context.CurrentResult[i] = context.CurrentResult[i] && context.SecondOperand[i];
            }
        }

		protected override void ProcessOr(SearchOperation operation)
        {
            LoadOperand(operation);
            for (int i = 0; i < context.CurrentResult.Length; i++)
            {
                context.CurrentResult[i] = context.CurrentResult[i] || context.SecondOperand[i];
            }
        }
		protected override int[] ElicitResultingDocuments()
        {
            var docs = new List<int>();
            for (int i = 0; i < context.CurrentResult.Length; i++)
            {
                if (context.CurrentResult[i])
                    docs.Add(i);
            }
            return docs.ToArray();
        }
		/*
		protected override void LoadOperand(SearchOperation operation)
        {
            var row = GetWordRow(operation.Word);
            context.SecondOperand = row;
            if (operation.IsInverted)
				context.SecondOperand = InvertArray(row);
        }
        */
		protected override bool[] InvertArray(bool[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = !arr[i];
            }

            return arr;
        }

        protected override bool[] GetWordRow(string word)
        {
            base.GetWordRow(word);

            int index = context.DocumentDictionary.WordMatrix.WordRows[word];
            return context.DocumentDictionary.WordMatrix.Matrix[index];
        }

    }
}
