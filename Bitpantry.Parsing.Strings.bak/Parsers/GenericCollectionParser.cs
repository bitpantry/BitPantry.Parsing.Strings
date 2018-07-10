using System;
using System.Collections.Generic;
using System.Linq;
using BitPantry.Parsing.Strings.Parsers.Component;

namespace BitPantry.Parsing.Strings.Parsers
{
    /// <summary>
    /// Parses a string of delimeter separated values as an <see cref="ICollection{T}"/> implementation. The
    /// target type can be any type that implements <see cref="ICollection{T}"/>
    /// </summary>
    public class GenericCollectionParser : IParser
    {
        public virtual bool CanParseType(Type type)
        {
            return type.IsGenericCollectionImplementation() &&
                   StringParsing.GetParser(type.GetGenericCollectionElementType()) != null;
        }

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

    }
}
