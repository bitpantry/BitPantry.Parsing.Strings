using System.Configuration;
using System.Linq;

namespace BitPantry.Parsing.Strings
{
    // CONVERSION NOTE: Previously this was pulled from the XML configuration file, but configuration of these
    //   values has been removed for now to get to .netstandard faster.

    /// <summary>
    /// A facade for interacting with the app / web configuration file
    /// </summary>
    static class AppConfigFacade
    {
        /// <summary>
        /// Returns the string splitter delimiter for strings with multiple values
        /// </summary>
        public static char CollectionStringSpliter => ',';


        /// <summary>
        /// Returns the string splitter delimiter for a key value pair
        /// </summary>
        public static char KeyValuePairStringSplitter => '=';

        /// <summary>
        /// Returns the string splitter delimiter for a dictionary
        /// </summary>
        public static char DictionaryStringSpliter => ';';

    }
}
