using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BitPantry.Parsing.Strings.Configuration;
using BitPantry.Parsing.Strings.Parsers;

namespace BitPantry.Parsing.Strings
{
    /// <summary>
    /// The string parsing utility class. All string parsing operations can be accomplished using this class.
    /// </summary>
    public static class StringParsing
    {
        private static ConcurrentDictionary<Type, IParser> _typeParserDict;
        private static List<IParser> _parsers;

        /// <summary>
        /// An enumeration of all known SingleValueParsers
        /// </summary>
        public static IEnumerator<IParser> Parsers => _parsers.GetEnumerator();

        #region INITIALIZATION

        static StringParsing()
        {
            InitializeParsers();          
        }

        private static void InitializeParsers()
        {
            _parsers = new List<IParser>();
            _typeParserDict = new ConcurrentDictionary<Type, IParser>();
         
            // load standard parsers

            AddParser(typeof(PrimitiveValueParser<bool>));
            AddParser(typeof(PrimitiveValueParser<DateTime>));
            AddParser(typeof(PrimitiveValueParser<string>));
            AddParser(typeof(PrimitiveValueParser<char>));
            AddParser(typeof(PrimitiveValueParser<byte>));
            AddParser(typeof(PrimitiveValueParser<sbyte>));
            AddParser(typeof(PrimitiveValueParser<short>));
            AddParser(typeof(PrimitiveValueParser<ushort>));
            AddParser(typeof(PrimitiveValueParser<int>));
            AddParser(typeof(PrimitiveValueParser<uint>));
            AddParser(typeof(PrimitiveValueParser<long>));
            AddParser(typeof(PrimitiveValueParser<ulong>));
            AddParser(typeof(PrimitiveValueParser<float>));
            AddParser(typeof(PrimitiveValueParser<double>));
            AddParser(typeof(PrimitiveValueParser<decimal>));
            AddParser(typeof(GenericCollectionParser));
            AddParser(typeof(ArrayParser));
            AddParser(typeof(EnumParser));
            AddParser(typeof(KeyValuePairParser));

            // load configured parsers

            var parserAppConfig = (StringValueParserConfiguration)
                System.Configuration.ConfigurationManager.GetSection(
                    Constants.StringParserConfigurationSectionName);
            if (parserAppConfig == null) return;

            foreach (var item in parserAppConfig.Items)
                AddParser(item.Type);
        }

        #endregion

        #region PARSER MANAGEMENT

        /// <summary>
        /// Adds a new parser to the available parsing collection
        /// </summary>
        /// <param name="parserType">The type of parser to add</param>
        /// <exception cref="System.ArgumentException"></exception>
        public static void AddParser(string parserType)
        {
            try
            {
                var type = Type.GetType(parserType);
                if(type == null)
                    throw new ArgumentException($"The type, \"{parserType}\" could not be found");
                AddParser(type);
            }
            catch (Exception ex)
            {
                ThrowLoadException(parserType, ex);
            }
        }

        /// <summary>
        /// Adds a parser to the available parsing collection
        /// </summary>
        /// <param name="parserType">The type of parser to add</param>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="ParserLoadException"></exception>
        public static void AddParser(Type parserType)
        {
            try
            {
                // ensure the type is an implementation of IParser

                if(!parserType.ImplementsIParser())
                    throw new ArgumentException(
                        $"Parser type \"{parserType}\" must implement interface {typeof(IParser).FullName}");

                // ensure parser has parameterless constructor

                if(parserType.GetConstructor(Type.EmptyTypes) == null)
                    throw new ParserLoadException(parserType.FullName,
                        $"{parserType.FullName} does not have a parameterless constructor");

                // instantiate parser

                var parser = (IParser)Activator.CreateInstance(parserType);

                // add parser to collection

                _parsers.Add(parser);
            }
            catch (Exception ex)
            {
                ThrowLoadException(parserType.FullName, ex);
            }
        }

        private static void ThrowLoadException(string parserType, Exception innerException)
        {
            throw new ParserLoadException(
                parserType,
                $"The {typeof (IParser).Name} type \"{parserType}\" could not be loaded. See the inner exception for further details.", innerException);
        }

        #endregion

        #region GET PARSERS

        /// <summary>
        /// Returns the best parser for the given type, or null if no parser could be found.
        /// </summary>
        /// <typeparam name="T">The type of parser to return</typeparam>
        /// <returns>
        /// The generic parser for the provided type
        /// </returns>
        public static GenericParser<T> GetParser<T>()
        {
            var parser = GetParser(typeof (T));
            if (parser == null)
                return null;
            return new GenericParser<T>(parser);
        }

        /// <summary>
        /// Returns the best parser for the given type, or null if no parser could be found.
        /// </summary>
        /// <param name="forType">The type to return a parser for</param>
        /// <returns>Works up the baseType tree from the given type until a matching parser is found</returns>
        public static IParser GetParser(Type forType)
        {
            // check the cache for a parser that has already been selected for the given type

            if (_typeParserDict.ContainsKey(forType))
                return _typeParserDict[forType];

            // starting at the given search type and going up the inheritance chain, find the first matching parser

            var searchType = forType;
            while (searchType != null)
            {
                // select all parsers which indicate they are capable of parsing the given type

                var parsers = _parsers.Where(p => p.CanParseType(forType)).ToList();

                // use first available parser when available

                if (parsers.Any())
                {
                    _typeParserDict.TryAdd(forType, parsers[0]);
                    return parsers[0];
                }

                // if no parser found, move up the type hierarchy for the given type

                searchType = searchType.BaseType;               
            }

            // no parsers found in given type hierarchy

            return null;
        }

        #endregion

        #region PARSING FUNCTIONS

        /// <summary>
        /// Parses the specified value.
        /// </summary>
        /// <typeparam name="TType">The type to parse the string into</typeparam>
        /// <param name="value">The string value to parse</param>
        /// <returns>The parsed value</returns>
        public static TType Parse<TType>(string value) { return GetParser<TType>().Parse(value); }


        /// <summary>
        /// Parses the specified value as the for type.
        /// </summary>
        /// <param name="forType">The type to parse as</param>
        /// <param name="value">the string value to parse</param>
        /// <returns></returns>
        public static object Parse(Type forType, string value) { return GetParser(forType).Parse(value, forType); }

        /// <summary>
        /// Attempts to parse and return the specified value. If it fails, the default value of the requested type is returned
        /// </summary>
        /// <typeparam name="TType">The type to parse the string into</typeparam>
        /// <param name="value">The string value to parse</param>
        /// <returns>The parsed value, or the default of the requested type</returns>
        public static TType SafeParse<TType>(string value)
        {
            try
            {
                return GetParser<TType>().Parse(value);
            }
            catch
            {
                // return empty collection if type is an ICollection or array implementation

                var forType = typeof (TType);
                if (forType.IsGenericCollectionImplementation() || forType.IsArray)
                    return Activator.CreateInstance<TType>();

                // otherwise return the default type value

                return default(TType);
            }
        }

        /// <summary>
        /// Attempts to parse and return the specified value. If it fails, the provided default value is returned
        /// </summary>
        /// <typeparam name="TType">The type to parse the string into</typeparam>
        /// <param name="value">The string value to parse</param>
        /// <param name="defaultValue">The default value</param>
        /// <returns>
        /// The parsed value, or the provided default value
        /// </returns>
        public static TType SafeParse<TType>(string value, TType defaultValue)
        {
            try { return GetParser<TType>().Parse(value); }
            catch { return defaultValue; }
        }

        #endregion
    }
}
