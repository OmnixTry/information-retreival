using LemmaSharp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using WordDictionary.IndexBuildingAlgo;
using WordDictionary.Search.Impl;

namespace WordDictionary.IndexCompression
{
	internal class RunLab8
	{
		Lemmatizer lemmatizer;

		public RunLab8()
		{
			string lemmFilePath = "H:\\Programming\\MAG Semester 3\\Information Retreival\\01Dictionary\\WordDictionary\\WordDictionary\\bin\\Debug\\net6.0\\full7z-mlteast-en.lem";
			using (var stream = File.OpenRead(lemmFilePath))
			{
				lemmatizer = new Lemmatizer(stream);
			}
		}
		public async Task RunCompress(string[] fileNames)
		{
			var compressor = new IndexCombressor("endDictFile.bin", "endIndexFile.bin", "endTebleWriterFile.bin");
			compressor.CompressIndex("SmallIndexBackup.txt");
			//compressor.DecompressIndex();
			
			
			//var result = compressor.IntToVbr(566);
			
			
			
			/*
			var file = new FileStream("dummyFile", FileMode.OpenOrCreate);
			int big = 99999;
			var bytes = BitConverter.GetBytes(big);
			file.Write(bytes, 0, bytes.Length);
			file.Position = 0;

			var bytesRead = new byte[4];
			file.Read(bytesRead);
			var finalInt = BitConverter.ToInt32(bytesRead);
			*/
		}

		public async Task RunCompressLargeDb()
		{
			var compressor = new IndexCombressor("endDictFile.bin", "endIndexFile.bin", "endTebleWriterFile.bin");
			compressor.CompressIndex("BiggestDbBackup.txt");
			
		}

		public void RunDeCompress() 
		{
			var compressor = new IndexCombressor("endDictFile.bin", "endIndexFile.bin", "endTebleWriterFile.bin");
			var index = compressor.DecompressIndex();

			var indexSearcher = new InvertedIndexSearcher(lemmatizer);
			var res = indexSearcher.SearchOnMatrix("alice", index);

			foreach (var item in res)
			{
				Console.WriteLine(item);
			}
		}
	}
}
