using LemmaSharp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordDictionary.DictionaryCreation.Impl;
using WordDictionary.JokerSearch.Combinations;
using WordDictionary.JokerSearch.SuffixTree;
using WordDictionary.JokerSearch.ThreeGramIndex;
using WordDictionary.Search.Impl;

namespace WordDictionary.JokerSearch
{
	internal class RunLab4
	{
		Lemmatizer lemmatizer;

		public RunLab4()
		{
			string lemmFilePath = "H:\\Programming\\MAG Semester 3\\Information Retreival\\01Dictionary\\WordDictionary\\WordDictionary\\bin\\Debug\\net6.0\\full7z-mlteast-en.lem";
			using (var stream = File.OpenRead(lemmFilePath))
			{
				lemmatizer = new Lemmatizer(stream);
			}
		}

		public async Task Run1(string[] fileNames)
		{
			var positionalDictCreator = new PositionalDictionaryCreator(lemmatizer);
			var suffixBuilder = new SuffixTreeBuilder(positionalDictCreator);
			var index = await suffixBuilder.BuildSuffixTreesForFiles(fileNames);
			var searcher = new SuffixTreeSearcher(index);
			var result = searcher.Search("fier");

			foreach (var item in result)
			{
				Console.WriteLine(item.documentId);
				foreach (var segment in item.segments)
				{
					Console.WriteLine(segment);
				}
			}
		}

		public async Task Run2(string[] fileNames)
		{
			var regularDictCreator = new DictionaryCreator(lemmatizer);
			var dictCreator = new CombinationIndexDictionaryCreator(regularDictCreator);
			var index = await dictCreator.GenerateIndex(fileNames);
			var invertedIndexSearcher = new InvertedIndexSearcher(lemmatizer);
			var combinationSearcher = new CombinationIndexSearch(index, invertedIndexSearcher);

			var res = await combinationSearcher.SearchDocuments("fu*re");

			Console.WriteLine("Found in documents:");
			for (var i = 0; i < res.Length; i++)
			{
				Console.WriteLine(res[i]);
			}
		}

		public async Task Run3(string[] fileNames)
		{
			var regularDictCreator = new DictionaryCreator(lemmatizer);
			var dictCreator = new KGramIndexDictionaryCreator(regularDictCreator);
			var index = await dictCreator.BuildIndex(fileNames);
			var invertedIndexSearcher = new InvertedIndexSearcher(lemmatizer);
			var combinationSearcher = new KGramSearcher(invertedIndexSearcher, dictCreator, index);

			var res = await combinationSearcher.Search("f*t*e");

			Console.WriteLine("Found in documents:");
			for (var i = 0; i < res.Length; i++)
			{
				Console.WriteLine(res[i]);
			}
		}
	}
}
