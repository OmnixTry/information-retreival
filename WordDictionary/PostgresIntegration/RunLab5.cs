using LemmaSharp.Classes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using WordDictionary.DictionaryCreation.Impl;
using WordDictionary.DocumentReaders.Entity;
using WordDictionary.PostgresIntegration.EF;
using WordDictionary.PostgresIntegration.Entity;
using static System.Reflection.Metadata.BlobBuilder;

namespace WordDictionary.PostgresIntegration
{
	public class RunLab5
	{
		Lemmatizer lemmatizer;

		public RunLab5()
		{
			string lemmFilePath = "H:\\Programming\\MAG Semester 3\\Information Retreival\\01Dictionary\\WordDictionary\\WordDictionary\\bin\\Debug\\net6.0\\full7z-mlteast-en.lem";
			using (var stream = File.OpenRead(lemmFilePath))
			{
				lemmatizer = new Lemmatizer(stream);
			}
		}
		public async Task LoadIntoDb(string[] fileNames)
		{
			var dictCreator = new DictionaryCreator(lemmatizer);
			var docs = new List<DocumentWords>();

			foreach (var file in fileNames)
			{
				docs.Add(await dictCreator.ProcessDocument(file));
			}

			var authors = docs.Select(d => d.Author).Distinct().Select((a, i)=> new Author() { fullname = a, id = i + 3}).ToList();
			var books = new List<Book>();

			for (int i = 0; i < docs.Count; i++)
			{
				var auth = authors.FirstOrDefault(a => a.fullname == docs[i].Author);
				books.Add(new Book() { id = i+1, name = docs[i].BookName, fulltext = string.Join(" ", docs[i].FlattenedParagraphs), authorid = auth.id });
			}

			using (LibraryDbContext db = new LibraryDbContext())
			{
				db.author.AddRange(authors);
				db.book.AddRange(books);
				db.SaveChanges();
			}

		}
		public async Task Run()
		{
			using (LibraryDbContext db = new LibraryDbContext())
			{
				var bookRepo = new BookRepository(db);
				while (true)
				{
					Console.WriteLine("Input text to Search in book: ");
					var books = bookRepo.SearchBookByText(Console.ReadLine());
					Console.WriteLine("Found such books:");
					foreach (var book in books)
					{
						Console.WriteLine($"{book.name}");
					}
				}
				
				

			}
		}
	}
}
