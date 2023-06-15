using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordDictionary.PostgresIntegration.Entity;

namespace WordDictionary.PostgresIntegration.EF
{
	internal class BookRepository
	{
		private readonly LibraryDbContext dbContext;

		public BookRepository(LibraryDbContext dbContext) 
		{
			this.dbContext = dbContext;
		}

		public Book GetBook(int id)
		{
			return dbContext.book.FirstOrDefault(b => b.id == id);
		}

		public List<Book> GetAllBooks()
		{
			return dbContext.book.ToList();
		}

		public List<Book> SearchBookByText(string text)
		{
			var param = new NpgsqlParameter("@text", text);
			//var books = dbContext.book.FromSqlRaw($"SELECT * FROM book WHERE fulltext @@ '{text}'").Include(b => b.author).ToList();
			var books = dbContext.book.FromSqlRaw($"SELECT * FROM book WHERE fulltext @@ @text", param).Include(b => b.author).ToList();
			return books;
		}
	}
}
