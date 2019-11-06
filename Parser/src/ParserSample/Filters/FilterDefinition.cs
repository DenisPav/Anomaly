namespace ParserSample.Filters
{
    public class FilterDefinition
    {
        public string Property { get; set; }
        public string Operation { get; set; }
        public object Value { get; set; }
        public string Logical { get; set; }

        public FilterDefinition(string property, string operation, object value, string logical)
        {
            Property = property;
            Operation = operation;
            Value = value;
            Logical = logical;
        }
    }
}
