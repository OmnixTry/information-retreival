using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordDictionary.DictionaryCreation.Entity;

namespace WordDictionary.Search.Entity
{
    public class BoolSearchContext<TOperand>
    {
        public TOperand CurrentResult { get; set; }
        public BoolOperationEnum OperationEnum { get; set; }
        public TOperand SecondOperand { get; set; }
        public DocumentDictionary DocumentDictionary { get; set; }
    }
}
