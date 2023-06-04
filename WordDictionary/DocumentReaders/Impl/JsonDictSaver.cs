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
    public class JsonDictSaver : IDictionarySaver
	{
		public async Task SaveFile(DocumentDictionary document, string fileName)
		{
			var options = new JsonSerializerOptions
			{
				Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
			};

			string jsonValue = JsonSerializer.Serialize(document, options);
			await File.WriteAllTextAsync(fileName, jsonValue);
		}

		public async Task SaveFile(object document, string fileName)
		{
			var options = new JsonSerializerOptions
			{
				Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
			};

			string jsonValue = JsonSerializer.Serialize(document, options);
			await File.WriteAllTextAsync(fileName, jsonValue);
		}

		public async Task<DocumentDictionary> ReadFile(string fileName)
		{
			var allText = await File.ReadAllTextAsync(fileName);
			var dict = JsonSerializer.Deserialize<DocumentDictionary>(allText);
			return dict;
		}
	}
}
