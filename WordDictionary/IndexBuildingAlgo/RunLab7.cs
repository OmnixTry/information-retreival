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
	}
}
