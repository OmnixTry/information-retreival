using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordDictionary.IndexBuildingAlgo.Entity
{
	public class BlockItem
	{
		public BlockItem(string word, int id)
		{
			Word = word;
			DocId = id;
		}
		public BlockItem(string row)
		{
			var parts = row.Split();
			Word = parts[0];
			DocId = int.Parse(parts[1]);
		}

		public string Word { get; set; }
		public int DocId { get; set; }
		public int Qty { get; set; }

		public override string ToString()
		{
			return $"{Word} {DocId}";
		}
	}
}
