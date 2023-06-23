using LemmaSharp.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordDictionary.DictionaryCreation.Entity;
using WordDictionary.DocumentReaders.Entity;
using WordDictionary.DocumentReaders.Impl;
using WordDictionary.WordReaders.Contract;

namespace WordDictionary.DictionaryCreation.Impl
{
    public class DictionaryCreatorBase
    {
        private IDocumentReader[] readers = new[]
        {
            new Fb2DocumentReader()
        };

        private readonly char[] splitters = new char[] { ' ', '\u002B', '\u00A0', '\u003E', '\u003C' };
        private Lemmatizer lemmatizer;

        public DictionaryCreatorBase(Lemmatizer lemmatizer)
        {
            this.lemmatizer = lemmatizer;
        }

        protected HashSet<string> CombineDocumentsIntoDictionary(List<DocumentEntity> entities)
        {
            var allWords = entities.SelectMany(d => d.UniqueWords)
                .Distinct(StringComparer.InvariantCultureIgnoreCase)
                .OrderBy(s => s)
                .ToArray();

            var dict = new HashSet<string>(allWords);

            return dict;
        }

        public string[] AllWords(DocumentWords doc)
        {
			var allWords = doc.FlattenedParagraphs.Select(p => RemovePunctuation(p))
				.SelectMany(s => s.Split(splitters))
				.Select(s => lemmatizer.Lemmatize(s.ToLower().Trim()))
                .Where(s => !string.IsNullOrEmpty(s) && s != null)
				.ToArray();

            return allWords;
		}

        protected string[] UniqueWords(DocumentWords doc, out int wordCount)
        {
            var allWords = AllWords(doc);

            wordCount = allWords.Length;

            return allWords
                .Distinct(StringComparer.InvariantCultureIgnoreCase)
                .ToArray();
        }

        protected async Task<List<DocumentEntity>> IdDocuments(string[] fileNames) 
        {
			var docs = fileNames.Select((fName, i) => new DocumentEntity(i, fName)).ToList();

            foreach (var doc in docs)
            {
                doc.DocumentWords = await ReadDocument(doc.FileName);
            }

            return docs;
		}


		protected async Task<DocumentWords> ReadDocument(string fileName)
        {
            var reader = GetReader(fileName);
            return await reader.ReadDocument(fileName);
        }



        protected IDocumentReader GetReader(string fileName)
        {
            string format = fileName.Split('.').Last().ToLower();
            var reader = readers.FirstOrDefault(r => r.SupportedFormats.Contains(format));
            if (reader == null)
            {
                throw new Exception($"Format {format} is not supported.");
            }

            return reader;
        }

        protected string RemovePunctuation(string paragraph)
        {
            var filtered = new string(paragraph.Where(c => !char.IsPunctuation(c)).ToArray());

            return Encoding.ASCII.GetString(Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding(Encoding.ASCII.EncodingName, new EncoderReplacementFallback(string.Empty), new DecoderExceptionFallback()), Encoding.UTF8.GetBytes(filtered)));
        }
    }
}
