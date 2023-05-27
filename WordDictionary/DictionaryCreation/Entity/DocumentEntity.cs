using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordDictionary.DocumentReaders.Entity;

namespace WordDictionary.DictionaryCreation.Entity
{
	public class DocumentEntity
	{
		public DocumentEntity(int id, string fileName)
		{
			Id = id;
			FileName = fileName;
		}

		public int Id { get; set; }
		public string FileName { get; set; }


		[NonSerialized]
		public DocumentWords DocumentWords;
		[NonSerialized]
		public string[] UniqueWords;
		public int WordCount { get; set; }
	}
}
