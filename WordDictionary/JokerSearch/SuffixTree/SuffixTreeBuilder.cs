using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordDictionary.DictionaryCreation.Entity;
using WordDictionary.DictionaryCreation.Impl;

namespace WordDictionary.JokerSearch.SuffixTree
{
	internal class SuffixTreeBuilder
	{
		private readonly PositionalDictionaryCreator dictionaryCreator;

		public SuffixTreeBuilder(PositionalDictionaryCreator dictionaryCreator) 
		{
			this.dictionaryCreator = dictionaryCreator;
		}

		public async Task<SuffixTreeIndex> BuildSuffixTreesForFiles (params string[] fileNames)
		{
			var index = new SuffixTreeIndex();
			var documents = await dictionaryCreator.ReadPositionalDocuments(false, fileNames);
			index.Documents = documents;

			foreach(var document in documents) 
			{
				index.SuffixTrees[document.Id] = GenerateTreeForDoc(document);
			}

			return index;
		}

		public SuffixTree GenerateTreeForDoc(DocumentEntity document)
		{
			var tree = new SuffixTree();

			for (int i = 0; i < document.AllWords.Length; i++)
			{
				tree.Put(document.AllWords[i], i);
			}

			return tree;
		}
	}
}
