
using FB2Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Xml;
using WordDictionary.DocumentReaders.Entity;
using WordDictionary.WordReaders.Contract;

namespace WordDictionary.DocumentReaders.Impl
{
	public class Fb2DocumentReader : IDocumentReader
	{
		public string[] SupportedFormats => new[] { "fb2" };
		public async Task<DocumentWords> ReadDocument(string fileName)
		{
			var file = await ReadFb2(fileName);

			var flattenedParagraphs = FlattenBook(file);
			//var uniqueWords = GetUniqueWords(flattenedParagraphs, out int wordCount);

			return new DocumentWords()
			{
				//UniqueWords = uniqueWords,
				FlattenedParagraphs = flattenedParagraphs,
				//WordCount = wordCount
			};
		}

		private async Task<FB2File> ReadFb2(string fileName)
		{
			using (FileStream fsSource = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			{
				// setup
				var readerSettings = new XmlReaderSettings
				{
					DtdProcessing = DtdProcessing.Ignore
				};
				var loadSettings = new XmlLoadSettings(readerSettings);
				
				// reading
				FB2File file = await new FB2Reader().ReadAsync(fsSource, loadSettings);

				return file;
			}
		}

		private string[] FlattenBook(FB2File file)
		{
			var regex = new Regex("[a-zA-Z][.][a-zA-Z]");
			var beforeFiltering = file.Bodies.SelectMany(b => b.Sections).SelectMany(s => s.Content).Select(c => {
				if(c is FB2Library.Elements.SectionItem)
				{
					return string.Concat((c as FB2Library.Elements.SectionItem).Content.Select(c => c.ToString()));
				}
				return c.ToString();
			}).ToArray();
			return beforeFiltering.Where(e => e != "\n" && !regex.IsMatch(e)).ToArray();
		}


	}
}
