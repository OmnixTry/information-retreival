using LemmaSharp.Classes;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordDictionary.DictionaryCreation.Impl;
using WordDictionary.JokerSearch.ThreeGramIndex;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System.Diagnostics;
using LuceneDirectory = Lucene.Net.Store.Directory;

namespace WordDictionary.LuceneSearch
{
	internal class RunLab6
	{
		Lemmatizer lemmatizer;

		public RunLab6()
		{
			string lemmFilePath = "H:\\Programming\\MAG Semester 3\\Information Retreival\\01Dictionary\\WordDictionary\\WordDictionary\\bin\\Debug\\net6.0\\full7z-mlteast-en.lem";
			using (var stream = File.OpenRead(lemmFilePath))
			{
				lemmatizer = new Lemmatizer(stream);
			}
		}
		public async Task Run(string[] fileNames)
		{
			var dictCreator = new DictionaryCreator(lemmatizer);
			using var luceneAdapter = new LuceneIndexAdapter(dictCreator);

			await luceneAdapter.LoadFilesIntoIndex(fileNames);

			while(true)
			{
				Console.WriteLine("\n\nInput search String:");
				luceneAdapter.FullTextSearch(Console.ReadLine());
			}
		}
	}
}
