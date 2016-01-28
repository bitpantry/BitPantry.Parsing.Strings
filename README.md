# String Value Parsing

The {{BitPantry.Parsing.Strings}} library can parse all .NET primitive types.

* bool
* DateTime
* string
* char
* byte
* sbyte
* short
* ushort
* int
* uint
* long
* ulong
* float
* double
* decimal 
* enum _(from the string or value representation)_

The library can also parse arrays and implementations of ICollection<> (including IDictionary<,> implementations as an ICollection<KeyValuePair<,>> implementation) out of the box.

Below are a few examples.

```javascript
var result = StringParsing.Parse<bool>("1"); // returns true
var result = StringParsing.Parse<bool>("yes"); // returns true
var result = StringParsing.Parse<bool>("false"); // returns true
var result = StringParsing.Parse<DateTime>("10/12/1979"); // returns a DateTime
var result = StringParsing.Parse<List<string>>("hello,goodbye"); // returns a list of the given strings
var result = StringParsing.Parse<List<int>>("1,2,3,4"); // list with values 1,2,3,4
var result = StringParsing.Parse<List<int>>("1,2-5,6,7"); // list with values 1,2,3,4,5,6,7
var result = StringParsing.Parse<Dictionary<string, int>("numberOne=1;numberTwo=2"); // Dictionary with two KeyValuePairs
```

_ See the included unit tests for more examples _

In addition to the _StringParsing.Parse_ function listed above, the _StringParsing.SafeParse_ functions provide convenience functionality for gracefully handling / ignoring parsing errors.

```javascript
        /// <summary>
        /// Attempts to parse and return the specified value. If it fails, the default value of the requested type is returned
        /// </summary>
        /// <typeparam name="TType">The type to parse the string into</typeparam>
        /// <param name="value">The string value to parse</param>
        /// <returns>The parsed value, or the default of the requested type</returns>
        public static TType SafeParse<TType>(string value)
```

```javascript
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
```

# Implementing Custom Parsers
By implementing the `BitPantry.Parsing.Strings.Parsers.IParser` interface, you can create your own custom parsers.

_ It will probably be helpful if you read the How it Works section below before implementing your own parsers _

```javascript
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
```

Your custom parsers can be added to the available parsers collection by calling one of the available `AddStringParser` functions on the `StringParsing` utility class.

* `public static void AddStringParser(string parserType)`
* `public static void AddStringParser(Type parserType)`

Or, you can add the custom _BitPantryParsing_ configuration section.

```
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
    <section name="*BitPantryParsing*" type="BitPantry.AssemblyPatcher.Configuration.AssemblyPatcherConfiguration, BitPantry.AssemblyPatcher" />
  </configSections>
```

```
  <BitPantryParsing>
    <parsers>
      <add type="<your type>" />
    </parsers>
  </BitPantryParsing>
```

# Briefly - How It Works
Note that the `Parse` function above accepts a string value of the value to parse as well as a `targetType` generic parameter. 

When a parser is requested, the logic works up the target type's inheritance graph to return the first appropriate parser (the first one who's ` CanParseFunction ` returns ` true `)

For example, the ` GenericCollectionParser ` will parse any implementation of ` ICollection<>`. So, the request ` StringParsing.Parse<Dictionary<string, int>>("keyName=1"); ` will be filled by the ` GenericCollectionParser ` which understands that the target type is an implementation of ` ICollection<KeyValuePair<string, int>> `. In addition, before committing to being able to parse a type (via the ` CanParseType ` function) the ` GenericCollectionParser ` ensures that there are parsers available for the specified element type (`KeyValuePair<string, int>`). The ` KeyValuePair<string, int> ` element type can be parsed using the out-of-the box ` KeyValuePairParser `, the ` PrimitiveValueParser<string> `, and the ` PrimitiveValueParser<int> `. 

A walk through of the ` GenericCollectionParser ` can be used to illustrate. The following code is a listing for the ` GenericCollectionParser.CanParseType ` function. This function ensures that the ` targetType ` is an implementation of ` ICollection<> ` and that a parser is also available for the collection's specified element type.

{{
        public virtual bool CanParseType(Type type)
        {
            return type.IsGenericCollectionImplementation() &&
                   StringParsing.GetParser(type.GetGenericCollectionElementType()) != null;
        }
}}

{{ type.GetGenericCollectionElementType() }} returns the element type for the given {{ ICollection<> }} implementation target type (which is {{ KeyValuePair<string, int> }}).

So, {{ StringParsing.GetParser(type.GetGenericCollectionElementType()) }} returns the {{ KeyValuePairParser }} which was selected because it confirmed that it could parse the type {{ KeyValuePair<string, int> }} - below is a listing for {{ KeyValuePairParser.CanParseType }}.

{{
        public virtual bool CanParseType(Type type)
        {
            return
                type.IsKeyValuePairType() &&
                StringParsing.GetParser(type.GetGenericArguments()[0]) != null &&
                StringParsing.GetParser(type.GetGenericArguments()[1]) != null;
        }
}}

As seen above, the {{ KeyValuePairParser.CanParseType }} function ensures that parsers are available for both the key and value types specified by the given {{ KeyValuePair<,> }} target type. It finds {{ PrimitiveValueParser<string> }} and {{ PrimitiveValueParser<int> }} for the job.

Finally, once all the parsers are identified and have confirmed that they can work together to parse the overall target type, the {{ GenericCollectionParser.Parse }} function listed below performs the actual parsing for the dictionary.

{{
        public virtual object Parse(string value, Type targetType = null)
        {
            if (targetType == null) throw new ArgumentNullException(nameof(targetType));

            if (!CanParseType(targetType))
                throw new ArgumentException($"Cannot parse collection for type {targetType.FullName}");

            // TODO: make the splitter some sort of provider as well. May want to use a regex or pipe

            var values = targetType.IsGenericDictionaryImplementation()
                ? value.SplitQuotedString(AppConfigFacade.DictionaryStringSpliter)
                : value.SplitQuotedString(AppConfigFacade.CollectionStringSpliter);

            var elementType = targetType.GetGenericCollectionElementType();

            return elementType.IsNumericType() 
                ? NumericCollectionParser.ParseCollection(values, targetType, elementType) 
                : StandardParse(values, targetType); 
        }

        /// <summary>
        /// Parses the given values into the given target <see cref="ICollection{T}"/> implementation
        /// </summary>
        /// <param name="values">The values to parse as elements of the resulting collection</param>
        /// <param name="targetType">The target <see cref="ICollection{T}"/> implementation type</param>
        /// <returns>The resulting collection</returns>
        protected virtual object StandardParse(string[] values, Type targetType)
        {
            var elementType = targetType.GetGenericCollectionElementType();
            var parser = StringParsing.GetParser(elementType);

            var collection = Activator.CreateInstance(targetType);
            var method = targetType.GetGenericCollectionInterfaceMappedAddMethod();
            foreach (var item in values.Select(i => parser.Parse(i, elementType)))
                method.Invoke(collection, new[] { item });

            return collection;
        }
}}

Notice that the {{ StandardParse }} function instantiates the {{ targetType }} which is the {{ Dictionary<string, int> }} but interacts with it using the {{ ICollection<> }} interface. Thus, every implementation of {{ ICollection<> }} should be parsable by the {{ GenericCollectionParser }} as long as parsers are also available for the specified element type.


