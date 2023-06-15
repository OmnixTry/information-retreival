using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordDictionary.PostgresIntegration.Entity
{
	internal class Book
	{
		public int id { get; set; }
		public string name { get; set; }
		public string fulltext { get; set; }
		public int authorid { get; set; }
		public Author author { get; set; }
	}
}
