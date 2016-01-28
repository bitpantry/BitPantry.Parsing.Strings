using System;

namespace BitPantry.Parsing.Strings.Parsers
{
    /// <summary>
    /// This exception occurs when a string parser load exception occurs
    /// </summary>
    public class ParserLoadException : Exception
    {
        /// <summary>
        /// The parser type being loaded
        /// </summary>
        public string ParserType { get; set; }

        public ParserLoadException(string parserType, string message) : base(message) {  ParserType = parserType; }
        public ParserLoadException(string parserType, string message, Exception innerEx) : base(message, innerEx) { ParserType = parserType; }
    }
}
