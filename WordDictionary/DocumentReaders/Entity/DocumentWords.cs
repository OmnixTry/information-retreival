﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordDictionary.DocumentReaders.Entity
{
    public class DocumentWords
    {
        public string BookName { get; set; }
        public string Author { get; set; }
        public string[] FlattenedParagraphs { get; set; }
        //public string[] UniqueWords { get; set; }
        //public int WordCount { get; set; }

    }
}
