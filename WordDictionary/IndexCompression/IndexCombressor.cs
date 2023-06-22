using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using WordDictionary.DocumentReaders.Entity;

namespace WordDictionary.IndexCompression
{
	internal class IndexCombressor : IDisposable
	{
		FileStream dictWriter;
		FileStream indexWriter;
		FileStream tableWriter;

		const int _wordsPerBlock = 4;
		int currentWordIndex = 0;
		int currentWordPointer = 0;

		int currentIndexPosition = 0;


		public IndexCombressor(string endDictFileName, string endIndexFileName, string endTebleWriterFileName)
		{
			dictWriter = new FileStream(endDictFileName, FileMode.OpenOrCreate);
			indexWriter = new FileStream(endIndexFileName, FileMode.OpenOrCreate);
			tableWriter = new FileStream(endTebleWriterFileName, FileMode.OpenOrCreate);
		}

		public void CompressIndex(string uncompressedFile)
		{
			using var file = new StreamReader(uncompressedFile);

			string currentLine;
			int count = 0;
			while((currentLine = file.ReadLine())!=null)
			{
				AddLineToCompressedIndex(currentLine);

				count++;
				//if (count % 4 == 0) break;
			}

		}

		public void DecompressIndex()
		{
			dictWriter.Position = 0;
			indexWriter.Position = 0;
			tableWriter.Position = 0;
		}

		public void AddLineToCompressedIndex(string word, int[] docIds)
		{
			// write contents recording
			if(currentWordIndex % 4 == 0) // write pointer to word in dictionary
			{
				var wordPointerBytes = BitConverter.GetBytes(dictWriter.Position);
				tableWriter.Write(wordPointerBytes);
			}
			var indexPointerBytes = BitConverter.GetBytes(indexWriter.Position);
			tableWriter.Write(indexPointerBytes);

			// get byte array for dictionary
			byte length = (byte)word.Length;
			var bytesToWrite = new byte[word.Length+1];
			bytesToWrite[0] = length;
			for (int i = 1; i < bytesToWrite.Length; i++)
			{
				bytesToWrite[i] = (byte)word[i-1];
			}

			dictWriter.Write(bytesToWrite);
			currentWordIndex++;

			// write list bytes
			var bytesForIndex = new List<byte[]>();
			foreach (var item in docIds)
			{
				bytesForIndex.Add(IntToVbr(item));
			}
			foreach (var item in bytesForIndex)
			{
				indexWriter.Write(item);
			}
		}

		public byte[] IntToVbr(Int32 value)
		{
			const byte lastOne = 0b_10000000;
			//Console.WriteLine(Convert.ToString(value, 2));
			var bytes = new List<byte>(7);
			bool isLast = true;
			do
			{
				var bytesConverted = BitConverter.GetBytes(value);
				byte current = 0;
				current |= bytesConverted.FirstOrDefault();
				current <<= 1;
				current >>= 1;
				if(isLast)
				{
					current |= lastOne;
					isLast = false;
				}
				bytes.Add(current);
				value >>= 7;
			} while (value != 0);

			bytes.Reverse();
			/*
			foreach (var item in bytes)
			{
				Console.Write(Convert.ToString(item, 2));
				Console.Write(' ');
			}

			Console.WriteLine();
			*/
			return bytes.ToArray();

		}

		private void AddLineToCompressedIndex(string line)
		{
			var split = line.Split(' ');
			AddLineToCompressedIndex(split[0], split.Skip(1).Select(s => int.Parse(s)).ToArray());
		}

		public void Dispose()
		{
			dictWriter?.Dispose();
			indexWriter?.Dispose();
		}
	}
}
