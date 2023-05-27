using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WordDictionary.DictionaryCreation.Entity;
using WordDictionary.DocumentReaders.Contract;

namespace WordDictionary.DocumentReaders.Impl
{
	internal class TxtDictSaver : IDictionarySaver
	{
		public async Task SaveFile(DocumentDictionary document, string fileName)
		{
			var wholeValue = string.Join('\n', document.Dictionary);
			await File.WriteAllTextAsync(fileName, wholeValue);
		}

		public async Task<DocumentDictionary> ReadFile(string fileName)
		{
			var allText = await File.ReadAllTextAsync(fileName);
			var dict = new SortedSet<string>(allText.Split('\n'));

			return new DocumentDictionary() 
			{ 
				Dictionary = dict
			};
		}
	}
}
