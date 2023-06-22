using LemmaSharp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordDictionary.DictionaryCreation.Impl;
using WordDictionary.IndexBuildingAlgo.Entity;

namespace WordDictionary.IndexBuildingAlgo
{
	public class BsBiIndexBuilder : DictionaryCreatorBase
	{
		const int quantityPerBlock = 1000;
		const int blockQty = 3;
		
		BlockItem[] blockItems = new BlockItem[quantityPerBlock];
		int blocksStashed = 1;
		int currentBlockCapacity = 0;

		public BsBiIndexBuilder(Lemmatizer lemmatizer) : base(lemmatizer)
		{
		}

		public void BuildIndex(params string[] fileNames)
		{
			//var documents = fileNames;
			var mergedFile = File.Open(BuildBlockFileName(0), FileMode.Create);
			mergedFile.Dispose();

			for(int i = 0; i < fileNames.Length; i++)
			{
				ProcessFile(fileNames[i], i);
			}
		}

		private void ProcessFile(string fileName, int id)
		{
			var file = ReadDocument(fileName).Result;
			var allWords = AllWords(file).OrderBy(w => w).OrderBy(w => w).ToArray();

			foreach (var word in allWords)
			{
				blockItems[currentBlockCapacity] = new BlockItem(word, id);
				if (currentBlockCapacity > 0 && blockItems[currentBlockCapacity - 1].Word == word) continue;

				currentBlockCapacity++;
				if (currentBlockCapacity == quantityPerBlock)
				{
					StashBlockToFile();
				}
			}
		}
		private void StashBlockToFile()
		{
			var indexedItems = blockItems.GroupBy(b => b.Word)
				.Select(b => new BlockIndexedItem(b.Key, b))
				.OrderBy(w => w.Word)
				.ToArray();

			File.WriteAllLines(BuildBlockFileName(blocksStashed), indexedItems.Select(b => b.ToString()));

			currentBlockCapacity = 0;
			blocksStashed++;
			if(blocksStashed == blockQty)
			{
				MergeBlocks();
			}
		}

		private void LoadNewFile()
		{

		}

		private void MergeBlocks()
		{
			var resultFile = new StreamWriter("tempMergeResult.txt");
			// oppen all files
			var files = new StreamReader[blockQty];
			var currentWordInFile = new BlockIndexedItem[blockQty];
			for (int i = 0; i < blockQty; i++)
			{
				files[i] = new StreamReader(BuildBlockFileName(i));
				files[i].BaseStream.Position = 0;
				currentWordInFile[i] = ReadNext(files[i]);
			}
			
			// while files are not empty
			while(currentWordInFile.Any(w => w!=null))
			{
				// find files that have same minimum word
				string minWord = currentWordInFile.Where(w => w != null).Min(w => w.Word);
				var minFiles = currentWordInFile.Select((w, i) => (w, i)).Where(w => w.w?.Word == minWord);

				// unite document ids
				var combined = new BlockIndexedItem();
				foreach (var file in minFiles)
				{
					combined.Add(file.w);
				}
				// write to files
				resultFile.WriteLine(combined);

				// read next line in all files that had minimum word
				foreach (var file in minFiles)
				{
					currentWordInFile[file.i] = ReadNext(files[file.i]);
				}
			}

			foreach(var file in files)
			{
				file.Dispose();
			}
			resultFile.Dispose();
			File.Delete(BuildBlockFileName(0));
			File.Move("tempMergeResult.txt", BuildBlockFileName(0));
			File.Delete("tempMergeResult.txt");

			blocksStashed = 1;
		}

		private BlockIndexedItem ReadNext(StreamReader reader)
		{
			string row = reader.ReadLine();

			if (row == null)
				return null;

			var item = new BlockIndexedItem(row);
			return item;
		}

		private string BuildBlockFileName(int blockId)
		{
			return $"BlockIndex{blockId}.txt";
		}
		
	}
}
