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
    public class DictionaryCreator
    {
        private IDocumentReader[] readers = new[]
        {
            new Fb2DocumentReader()
        };

        private readonly char[] splitters = new char[] { ' ', '\u002B', '\u00A0', '\u003E', '\u003C' };
        private Lemmatizer lemmatizer;

        public DictionaryCreator(Lemmatizer lemmatizer)
        {
            this.lemmatizer = lemmatizer;
        }

        public async Task<DocumentDictionary> CreateDictionary(params string[] fileNames)
        {
            var docs = fileNames.Select((fName, i) => new DocumentEntity(i, fName)).ToList();

            var wordsFromAll = new DocumentWords[fileNames.Length];
            for (int i = 0; i < docs.Count; i++)
            {
                docs[i].DocumentWords = wordsFromAll[i] = await ProcessDocument(docs[i].FileName);
                docs[i].UniqueWords = UniqueWords(wordsFromAll[i], out int wordCount).OrderBy(s => s).ToArray();
                docs[i].WordCount = wordCount;
            }

            var dicationary = CombineDocumentsIntoDictionary(docs);
            var index = CombineDocumentsIntoIndex(dicationary, docs);
            var matrix = CombineDocumentsIntoMatrix(dicationary, docs);

            var docDict = new DocumentDictionary()
            {
                Documents = docs,
                Dictionary = dicationary,
                Index = index,
                WordMatrix = matrix
            };

            return docDict;
        }

        private HashSet<string> CombineDocumentsIntoDictionary(List<DocumentEntity> entities)
        {
            var allWords = entities.SelectMany(d => d.UniqueWords)
                .Distinct(StringComparer.InvariantCultureIgnoreCase)
                .OrderBy(s => s)
                .ToArray();

            var dict = new HashSet<string>(allWords);

            return dict;
        }

        private Dictionary<string, LinkedList<int>> CombineDocumentsIntoIndex(ISet<string> dict, List<DocumentEntity> entities)
        {
            var dictionary = new Dictionary<string, LinkedList<int>>(dict.Count, StringComparer.InvariantCultureIgnoreCase);

            foreach (var word in dict)
            {
                dictionary[word] = new LinkedList<int>();
            }

            foreach (var docEntity in entities)
            {
                foreach (var word in docEntity.UniqueWords)
                {
                    dictionary[word].AddLast(docEntity.Id);
                }
            }

            return dictionary;
        }

        private WordMatrix CombineDocumentsIntoMatrix(ISet<string> dict, List<DocumentEntity> entities)
        {
            WordMatrix wordMatrix = new WordMatrix()
            {
                Matrix = new bool[dict.Count][],
                WordRows = new Dictionary<string, int>(),
            };

            // create mapping between words and columns in the matrix
            var allWordsArr = dict.OrderBy(s => s).ToArray();
            for (int i = 0; i < dict.Count; i++)
            {
                wordMatrix.WordRows[allWordsArr[i]] = i;
                wordMatrix.Matrix[i] = new bool[entities.Count];
			}

            foreach (var entity in entities)
            {
                foreach (var word in entity.UniqueWords)
                {
                    var row = wordMatrix.WordRows[word];
                    wordMatrix.Matrix[row][entity.Id] = true;
                }
            }

            return wordMatrix;
        }
        private string[] UniqueWords(DocumentWords doc, out int wordCount)
        {
            var allWords = doc.FlattenedParagraphs.Select(p => RemovePunctuation(p))
                .SelectMany(s => s.Split(splitters))
                .Select(s => lemmatizer.Lemmatize(s.ToLower().Trim()))
                .ToArray();

            wordCount = allWords.Length;

            return allWords
                .Distinct(StringComparer.InvariantCultureIgnoreCase)
                .ToArray();
        }

        private async Task<DocumentWords> ProcessDocument(string fileName)
        {
            var reader = GetReader(fileName);
            return await reader.ReadDocument(fileName);
        }

        private IDocumentReader GetReader(string fileName)
        {
            string format = fileName.Split('.').Last();
            var reader = readers.FirstOrDefault(r => r.SupportedFormats.Contains(format));
            if (reader == null)
            {
                throw new Exception($"Format {format} is not supported.");
            }

            return reader;
        }

        private string RemovePunctuation(string paragraph)
        {
            var filtered = new string(paragraph.Where(c => !char.IsPunctuation(c)).ToArray());

            return Encoding.ASCII.GetString(Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding(Encoding.ASCII.EncodingName, new EncoderReplacementFallback(string.Empty), new DecoderExceptionFallback()), Encoding.UTF8.GetBytes(filtered)));
        }
    }
}
