using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WordDictionary.IndexBuildingAlgo.Entity
{
	internal class BlockIndexedItem
	{
		public string Word { get; set; }
		public LinkedList<int> DocumentIds { get; set; }

		public BlockIndexedItem() 
		{ 
			DocumentIds = new LinkedList<int>();
		}

		public BlockIndexedItem(string word, IEnumerable<BlockItem> documentIds) : this()
		{
			Word = word;
			foreach (var item in documentIds.Select(d => d.DocId).Distinct())
			{
				DocumentIds.AddLast(item);
			}
		}

		public BlockIndexedItem(string fileLine) : this()
		{
			var split = fileLine.Split(' ');
			Word = split[0];
			DocumentIds = new LinkedList<int>();
            foreach (var item in split.Skip(1))
            {
				DocumentIds.AddLast(int.Parse(item));
            }
        }
		public void Add(BlockIndexedItem blockItem)
		{
			Word = blockItem.Word;
			foreach (int id in blockItem.DocumentIds)
			{
				DocumentIds.AddLast(id);
			}
		}

		public override string ToString()
		{
			return $"{Word} {String.Join(' ', DocumentIds.OrderBy(i => i))}";
		}
	}
}
