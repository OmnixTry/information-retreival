// See https://aka.ms/new-console-template for more information
using LemmaSharp;
using LemmaSharp.Classes;
using System.IO.Enumeration;
using WordDictionary.DictionaryCreation;
using WordDictionary.DictionaryCreation.Impl;
using WordDictionary.DocumentReaders.Impl;
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

// init lemm
Lemmatizer lemmatizer;
string lemmFilePath = "H:\\Programming\\MAG Semester 3\\Information Retreival\\01Dictionary\\WordDictionary\\WordDictionary\\bin\\Debug\\net6.0\\full7z-mlteast-en.lem";
using (var stream = File.OpenRead(lemmFilePath))
{
	lemmatizer = new Lemmatizer(stream);
	var result = lemmatizer.Lemmatize("doing");
}



var dictCreator = new DictionaryCreator(lemmatizer);
var dict = await dictCreator.CreateDictionary(fileName, fileName2, fileName3, fileName4, fileName5, fileName6, fileName7, fileName8, fileName9, fileName10);

Console.WriteLine("Results:");
Console.WriteLine($"Number of Files:	{ dict.Documents.Count }");
Console.WriteLine($"Total Words:		{ dict.Documents.Sum(d => d.WordCount) }");
Console.WriteLine($"Dictionary size:	{ dict.Dictionary.Count }");


var jsonSaver = new JsonDictSaver();
await jsonSaver.SaveFile(dict, "FirstDict.json");
var readDictJson = await jsonSaver.ReadFile("FirstDict.json");
Console.WriteLine("Done Json");

var boolSearcher = new BoolSearcher(lemmatizer);
var indexSearcher = new InvertedIndexSearcher(lemmatizer);



//string querry = "book AND NOT henry OR NOT hellish";
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