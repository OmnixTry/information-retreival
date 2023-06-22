// See https://aka.ms/new-console-template for more information
using LemmaSharp;
using LemmaSharp.Classes;
using System.IO.Enumeration;
using System.Numerics;
using WordDictionary.DictionaryCreation;
using WordDictionary.DictionaryCreation.Impl;
using WordDictionary.DocumentReaders.Impl;
using WordDictionary.IndexBuildingAlgo;
using WordDictionary.IndexCompression;
using WordDictionary.JokerSearch;
using WordDictionary.JokerSearch.SuffixTree;
using WordDictionary.LuceneSearch;
using WordDictionary.PostgresIntegration;
using WordDictionary.Search.Impl;

Console.WriteLine("Hello, World!");

const string fileName = "(Sherlock Holmes 6) Doyle, Arthur Conan - The Return of Sherlock Holmes.fb2";
const string fileName2 = "[Phryne Fisher 9 ] greenwood, Kerry - Raisins and Almonds.fb2";
const string fileName3 = "[Sherlock Holmes 1 Sherlock Holmes Novels 1] Конан Дойл, Артур Игнациус - A Study in Scarlet (Wildside Press) - libgen.li.fb2";
const string fileName4 = "[Sherlock Holmes 1Sherlock Holmes 3 Sherlock Holmes short story collections 1] Конан Дойл, Артур Игнациус - The Adventures of Sherlock Holmes (Barnes & Noble Books) - libgen.li.fb2";
const string fileName5 = "[Sherlock Holmes short story collections 2] Конан Дойл, Артур Игнациус - Memoirs of Sherlock Holmes (Wildside Press LLC) - libgen.li.fb2";
const string fileName6 = "16855053.fb2";
const string fileName7 = "16882097.fb2";
const string fileName8 = "O. Henry, - New Yorkers. Short Stories (2012-6-2) - libgen.li.fb2";
const string fileName9 = "O. Henry, - The Four Million (14.10.2013, MOST Publishing) - libgen.li.fb2";
const string fileName10 = "15641636.fb2";
const string fileName11 = "279770.fb2";

//string[] fileNames = new string[] { fileName, fileName2, fileName3, fileName4, fileName5, fileName6, fileName7, fileName8, fileName9, fileName10 };
string[] fileNames = new string[] { fileName, fileName2, fileName3, fileName4, fileName5, fileName6, fileName7, fileName8, fileName9, fileName10 };

/*
Lemmatizer lemmatizer;
var jsonSaver = new JsonDictSaver();

string lemmFilePath = "H:\\Programming\\MAG Semester 3\\Information Retreival\\01Dictionary\\WordDictionary\\WordDictionary\\bin\\Debug\\net6.0\\full7z-mlteast-en.lem";
using (var stream = File.OpenRead(lemmFilePath))
{
	lemmatizer = new Lemmatizer(stream);
}
*/
//
// LAB 2
//




/*
var dictCreator = new DictionaryCreator(lemmatizer);
var dict = await dictCreator.CreateDictionary(fileName, fileName2, fileName3, fileName4, fileName5, fileName6, fileName7, fileName8, fileName9, fileName10);

Console.WriteLine("Results:");
Console.WriteLine($"Number of Files:	{ dict.Documents.Count }");
Console.WriteLine($"Total Words:		{ dict.Documents.Sum(d => d.WordCount) }");
Console.WriteLine($"Dictionary size:	{ dict.Dictionary.Count }");


jsonSaver = new JsonDictSaver();
await jsonSaver.SaveFile(dict, "FirstDict.json");
var readDictJson = await jsonSaver.ReadFile("FirstDict.json");
Console.WriteLine("Done Json");

var boolSearcher = new BoolMatrixSearcher(lemmatizer);
var indexSearcher = new InvertedIndexSearcher(lemmatizer);


string querry = "heir OR hemorrhage AND NOT hence";
var res = boolSearcher.SearchOnMatrix(querry, dict);
Console.WriteLine(querry);
Console.WriteLine("Matrix");
foreach (var item in res)
{
	Console.Write($"{item} ");
}
Console.WriteLine();

res = indexSearcher.SearchOnMatrix(querry, dict);
Console.WriteLine("Index");
foreach (var item in res)
{
	Console.Write($"{item} ");
}
Console.WriteLine();
*/

//
// LAB 3
//

// pairwise

/*
var dictCreator = new PairwiseDictionaryCreator(lemmatizer);

var dictionary = await dictCreator.CreateDictionary(fileNames);

await jsonSaver.SaveFile(dictionary, "pairwiseDict.json");

var searcher = new PairwiseSearcher(new InvertedIndexSearcher(lemmatizer), lemmatizer);

var res = searcher.SearchDocuments("in her hand gripped", dictionary);
foreach (var item in res)
{
	Console.WriteLine(item);
}
*/

// positional

/*
var positionalDictCreator = new PositionalDictionaryCreator(lemmatizer);
var positionalIndex = await positionalDictCreator.CreateDictionary(fileNames);
var positionalSearcher = new PositionalIndexSearcher(lemmatizer);
await jsonSaver.SaveFile((object)positionalIndex, "positionalIndex.json");
//var resPositional = positionalSearcher.SearchByQuerry("while /2 holmes", positionalIndex, out var pp1Index);
var resPositional = positionalSearcher.SearchByQuerry("presume /2 you /2 looked", positionalIndex, out var pp1Index);
foreach (var item in resPositional)
{
	Console.WriteLine(item);
}

foreach (var item in pp1Index)
{
	var doc = positionalIndex.Documents.First(d => d.Id == item.docId);
	var words = positionalDictCreator.AllWords(doc.DocumentWords);

	// debug here to take a look at sentances!!!
	var close = words.Skip(item.index - 10).Take(20).ToArray();
	
	Console.WriteLine(item.docId + " " + item.index);
	Console.WriteLine(string.Join(' ', close));
}
*/

// Lab 4

//var runLab4 = new RunLab4();
//await runLab4.Run1(fileNames);
//await runLab4.Run2(fileNames);
//await runLab4.Run3(fileNames);

//var runLab5 = new RunLab5();
//await runLab5.LoadIntoDb(fileNames);
//await runLab5.Run();

//var runLab6 = new RunLab6();
//await runLab6.Run(fileNames);

//var runLab7 = new RunLab7();
//await runLab7.Run(fileNames);

var runLab8 = new RunLab8();
await runLab8.Run(fileNames);
