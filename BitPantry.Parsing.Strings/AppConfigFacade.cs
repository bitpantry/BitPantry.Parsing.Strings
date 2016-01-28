using System.Configuration;
using System.Linq;

namespace BitPantry.Parsing.Strings
{
    /// <summary>
    /// A facade for interacting with the app / web configuration file
    /// </summary>
    static class AppConfigFacade
    {
        /// <summary>
        /// Returns the string splitter delimiter for strings with multiple values
        /// </summary>
        public static char CollectionStringSpliter { get { return GetString("BitPantry.Parsing.CollectionStringSpliter", ",").First(); } }


        /// <summary>
        /// Returns the string splitter delimiter for a key value pair
        /// </summary>
        public static char KeyValuePairStringSplitter { get { return GetString("BitPantry.Parsing.KeyValuePairStringSplitter", "=").First(); } }

        /// <summary>
        /// Returns the string splitter delimiter for a dictionary
        /// </summary>
        public static char DictionaryStringSpliter { get { return GetString("BitPantry.Parsing.DictionaryStringSpliter", ";").First(); } }


        static string GetString(string key, string defaultValue = null)
        {
            var value = ConfigurationManager.AppSettings.Get(key);
            return value ?? defaultValue;
        }
    }
}
