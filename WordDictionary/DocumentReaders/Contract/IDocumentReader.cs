using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordDictionary.DocumentReaders.Entity;

namespace WordDictionary.WordReaders.Contract
{
	public interface IDocumentReader
	{
		string[] SupportedFormats { get; }
		Task<DocumentWords> ReadDocument(string fileName);
	}
}
