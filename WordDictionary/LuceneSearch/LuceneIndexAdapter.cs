using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordDictionary.DictionaryCreation.Impl;
using LuceneDirectory = Lucene.Net.Store.Directory;
using WordDictionary.DocumentReaders.Entity;
using Lucene.Net.QueryParsers.Classic;

namespace WordDictionary.LuceneSearch
{
	public class LuceneIndexAdapter : IDisposable
	{
		private readonly DictionaryCreator dictionaryCreator;
		private const LuceneVersion luceneVersion = LuceneVersion.LUCENE_48;

		private List<DocumentWords> documents = new List<DocumentWords>();
		private LuceneDirectory indexDir;
		private StandardAnalyzer standardAnalyzer;
		private IndexWriter writer;
		public LuceneIndexAdapter(DictionaryCreator dictionaryCreator)
		{
			this.dictionaryCreator = dictionaryCreator;
		}

		public async Task LoadFilesIntoIndex(string[] fileNames)
		{
			foreach (var file in fileNames) 
			{
				documents.Add(await dictionaryCreator.ProcessDocument(file));
			}
			InitLucene();

			foreach (var document in documents) 
			{ 
				AddDocumentToIndex(document);
			}
			writer.Commit();
		}

		public void FullTextSearch(string textToSearch, int topQuantity = 3)
		{
			using DirectoryReader reader = writer.GetReader(applyAllDeletes: true);
			IndexSearcher searcher = new IndexSearcher(reader);

			QueryParser parser = new MultiFieldQueryParser(luceneVersion, new[] { "bookBody", "author", "bookName" }, standardAnalyzer);
			Query query = parser.Parse(textToSearch);
			TopDocs topDocs = searcher.Search(query, n: topQuantity);         //indicate we want the first 3 results

			Console.WriteLine($"Matching results: {topDocs.TotalHits}");

			for (int i = 0; i < topDocs.ScoreDocs.Length; i++)
			{
				//read back a doc from results
				Document resultDoc = searcher.Doc(topDocs.ScoreDocs[i].Doc);

				string bookName = resultDoc.Get("bookName");
				string author = resultDoc.Get("author");
				Console.WriteLine($"Search Result {i + 1}: {author} - {bookName}");
			}
		}

		private void InitLucene()
		{
			//Open the Directory using a Lucene Directory class
			string indexName = "books_index";
			string indexPath = Path.Combine(Environment.CurrentDirectory, indexName);

			indexDir = FSDirectory.Open(indexPath);

			//Create an analyzer to process the text 
			standardAnalyzer = new StandardAnalyzer(luceneVersion);

			//Create an index writer
			IndexWriterConfig indexConfig = new IndexWriterConfig(luceneVersion, standardAnalyzer);
			indexConfig.OpenMode = OpenMode.CREATE;                             // create/overwrite index
			writer = new IndexWriter(indexDir, indexConfig);			
		}

		private void AddDocumentToIndex(DocumentWords entity)
		{
			Document doc = new Document();
			doc.Add(new TextField("bookBody", string.Join(' ', entity.FlattenedParagraphs), Field.Store.YES));
			doc.Add(new StringField("author", entity.Author, Field.Store.YES));
			doc.Add(new StringField("bookName", entity.BookName, Field.Store.YES));
			writer.AddDocument(doc);
		}

		public void Dispose()
		{
			indexDir.Dispose();
		}
	}
}
