using System.Linq;
using System.Text.RegularExpressions;

namespace BitPantry.Parsing.Strings
{
    internal static class StringExtensions
    {
        /// <summary>
        /// Splits an input string on a given delimiter using a regular expression which considers quoted strings
        /// </summary>
        /// <param name="input">The raw input string to split</param>
        /// <param name="delimiter">The delimiter</param>
        /// <returns>The split input array</returns>
        public static string[] SplitQuotedString(this string input, char delimiter = ' ')
        {
            var csvPreservingQuotedStrings = new Regex($"(\"[^\"]*\"|[^{delimiter}])+|(\\s?)+");
            var values =
                 csvPreservingQuotedStrings.Matches(input)
                .Cast<Match>()
                .Where(m => !string.IsNullOrEmpty(m.Value))
                .Select(m => m.Value);
            return values.ToArray();
        }
    }
}
