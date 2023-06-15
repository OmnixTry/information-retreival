using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordDictionary.PostgresIntegration.Entity;

namespace WordDictionary.PostgresIntegration.EF
{
	internal class LibraryDbContext : DbContext
	{
		public DbSet<Author> author { get; set; }
		public DbSet<Book> book { get; set; }
		public LibraryDbContext()
		{

		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			//base.OnConfiguring(optionsBuilder);
			optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=bookLibrary;Username=postgres;Password=123");
		}
	}
}
