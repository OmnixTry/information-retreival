using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordDictionary.Search.Entity
{
    public class SearchOperation
    {
        public string Word { get; set; }
        public BoolOperationEnum Operation { get; set; }

        public bool IsInverted { get; set; }
    }
}
