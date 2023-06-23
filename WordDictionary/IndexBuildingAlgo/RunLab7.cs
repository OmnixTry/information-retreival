using LemmaSharp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordDictionary.DictionaryCreation.Impl;
using WordDictionary.LuceneSearch;

namespace WordDictionary.IndexBuildingAlgo
{
	public class RunLab7
	{
		Lemmatizer lemmatizer;

		public RunLab7()
		{
			string lemmFilePath = "H:\\Programming\\MAG Semester 3\\Information Retreival\\01Dictionary\\WordDictionary\\WordDictionary\\bin\\Debug\\net6.0\\full7z-mlteast-en.lem";
			using (var stream = File.OpenRead(lemmFilePath))
			{
				lemmatizer = new Lemmatizer(stream);
			}
		}
		public async Task Run(string[] fileNames)
		{
			var indexBuilder = new BsBiIndexBuilder(lemmatizer);
			
			indexBuilder.BuildIndex(fileNames);
		}

		public async Task RunLargeDb()
		{
			string[] allfiles = Directory.GetFiles("H:\\words\\Programming Sample Data Fanfics\\FanFictions(2600).English.language", "*.fb2", SearchOption.AllDirectories);

			var indexBuilder = new BsBiIndexBuilder(lemmatizer);
			
			//indexBuilder.BuildIndex(allfiles.Take(10).ToArray());

			indexBuilder.BuildIndex(allfiles.ToArray());

		}
	}
}
