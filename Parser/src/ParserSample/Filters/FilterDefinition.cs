using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParserSample.Filters
{
    public class FilterDefinition
    {
        public string Property { get; set; }
        public string Operation { get; set; }
        public int Value { get; set; }
        public string Logical { get; set; }

        public FilterDefinition(string property, string operation, int value, string logical)
        {
            Property = property;
            Operation = operation;
            Value = value;
            Logical = logical;
        }
    }
}
