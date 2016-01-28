using System;

namespace BitPantry.Parsing.Strings.Parsers
{
    /// <summary>
    /// The core parsing interface. All parsers implement this interface.
    /// </summary>
    public interface IParser
    {
        /// <summary>
        /// Determines whether this parser can parse the given type
        /// </summary>
        /// <param name="type">The type to evaluate</param>
        /// <returns>True if this parser can parse the type, false otherwords</returns>
        bool CanParseType(Type type);

        /// <summary>
        /// Parses the given string into the target type
        /// </summary>
        /// <param name="value">The string to parse</param>
        /// <param name="targetType">The target type.</param>
        /// <returns>The parsed value</returns>
        object Parse(string value, Type targetType);
    }
}
