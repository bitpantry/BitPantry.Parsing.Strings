namespace BitPantry.Parsing.Strings.Parsers
{
    /// <summary>
    /// A convenience class that can be returned when a specific target type parser is requested using a generic type 
    /// parameter. This allows return types to be cast directly allowing for more concice code against the parsing API
    /// </summary>
    /// <typeparam name="T">The target type that this parser returns</typeparam>
    public class GenericParser<T> 
    {
        private readonly IParser _parser;
        
        public GenericParser(IParser parser)
        {
            _parser = parser;
        }

        public T Parse(string value)
        {
            return (T)_parser.Parse(value, typeof (T));
        }

    }
}
