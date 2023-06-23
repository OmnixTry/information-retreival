using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using WordDictionary.DictionaryCreation.Entity;
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

		public DocumentDictionary DecompressIndex()
		{
			dictWriter.Position = 0;
			indexWriter.Position = 0;
			tableWriter.Position = 0;

			var dict = ReadDict();
			dictWriter.Position = 0;

			var table = ReadTable();

			var index = GetIndex(table);

			var resIndex = new DocumentDictionary();
			resIndex.Dictionary = new HashSet<string>(dict);
			resIndex.Index = index;

			return resIndex;
		}

		private List<string> ReadDict()
		{
			var words = new List<string>();

			var length = new byte[1];

			while(dictWriter.Read(length) != 0)
			{
				int wordLen = (int)length[0];
				var wordBuff = new byte[wordLen];
				dictWriter.Read(wordBuff);
				words.Add(string.Concat(wordBuff.Select(w => (char)w)));
			}

			return words;

		}
		private Dictionary<string, long> ReadTable()
		{
			long wordPosition = 0;
			var positionBuff = BitConverter.GetBytes(dictWriter.Position);

			var table = new Dictionary<string, long>();


			tableWriter.Position = 0;
			while (tableWriter.Read(positionBuff) != 0)
			{
				var position = BitConverter.ToInt64(positionBuff);
				var words = GetMultipleWordsAtPosition(position, _wordsPerBlock);


				long currentIndexPosition = 0;
				var indexPositionBuff = BitConverter.GetBytes(currentIndexPosition);
				

				for (int i = 0; i < _wordsPerBlock; i++)
				{
					tableWriter.Read(indexPositionBuff);
					currentIndexPosition = BitConverter.ToInt64(indexPositionBuff);
					table[words[i]] = currentIndexPosition;
				}
			}

			return table;
		}
		private Dictionary<string, LinkedList<int>> GetIndex(Dictionary<string, long> table)
		{
			var result = new Dictionary<string, LinkedList<int>>();

			indexWriter.Position = 0;
			var tableRows = table.ToArray();
			for (int i = 0; i < tableRows.Length - 1; i++)
			{
				var position = tableRows[i].Value;
				var lengthOfList = tableRows[i + 1].Value - position;
				if (tableRows[i + 1].Value == 0) break;
				var buff = new byte[lengthOfList];
				indexWriter.Read(buff);
				result[tableRows[i].Key] = GetDocIds(buff);
			}

			return result;
		}

		private LinkedList<int> GetDocIds(byte[] buff)
		{
			var result = new LinkedList<int>();
			byte lastByte = 0b_10000000;
			
			List<byte> currentNumBytes = new List<byte>();

			for (int i = 0; i < buff.Length; i++)
			{
				currentNumBytes.Add(buff[i]);
				if ((buff[i] & lastByte) != 0)
				{
					result.AddLast(ConstructIntFormBytes(currentNumBytes));
					currentNumBytes.Clear();
				}
			}


			return result;
		}
		
		private int ConstructIntFormBytes(List<byte> bytes)
		{
			var remove1 = 0b_01111111;
			int res = 0;
			foreach(var currByte in bytes)
			{
				res <<= 7;
				var sevenBits = currByte & remove1;
				res |= sevenBits;
			}

			return res;
		}

		private string[] GetMultipleWordsAtPosition(long position, int count)
		{
			dictWriter.Position = position;
			var words = new string[count];
			for (int i = 0; i < count; i++)
			{
				words[i] = GetWordOnCurrentPosition();
			}

			return words;
		}
		private string GetWordOnCurrentPosition()
		{
			var length = new byte[1];
			
			dictWriter.Read(length);
			
			int wordLen = (int)length[0];
			var wordBuff = new byte[wordLen];
			dictWriter.Read(wordBuff);
			return string.Concat(wordBuff.Select(w => (char)w));
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
