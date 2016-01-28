using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BitPantry.Parsing.Strings.Parsers.Component;

namespace BitPantry.Parsing.Strings.Parsers
{
    /// <summary>
    /// Parses a list of delimeter separated values as an array that can be parsed by a given SingleValueParser. The
    /// target type must be an array
    /// </summary>
    public class ArrayParser : IParser
    {
        public bool CanParseType(Type type)
        {
            return type.IsArray && StringParsing.GetParser(type.GetElementType()) != null;
        }

        public object Parse(string value, Type targetType)
        {
            // TODO: make the splitter some sort of provider as well. May want to use a regex or pipe
            return Parse(value.SplitQuotedString(AppConfigFacade.CollectionStringSpliter), targetType);
        }

        /// <summary>
        /// Parses the list of values as an array of type provided by the SingleValueParser
        /// </summary>
        /// <param name="values">The element values to parse</param>
        /// <param name="targetType">The target type to parse to</param>
        /// <returns>
        /// The parsed array
        /// </returns>
        /// <exception cref="System.InvalidOperationException">If the given value and targetType cannot be parsed</exception>
        protected virtual object Parse(string[] values, Type targetType)
        {
            if(!CanParseType(targetType))
                throw new InvalidOperationException($"Cannot parse {nameof(targetType)}, \"{targetType.FullName}\"");

            var elementType = targetType.GetElementType();

            if (elementType.IsNumericType())
            {
                var list = (IList)NumericCollectionParser.ParseCollection(values, typeof (List<>).MakeGenericType(elementType),
                    elementType);

                var arr = Activator.CreateInstance(elementType.MakeArrayType(), list.Count);
                for (var i = 0; i < list.Count; i++)
                    ((Array)arr).SetValue(list[i], i);

                return arr;
            }

            return StandardParse(values, targetType, elementType);
        }

        protected virtual object StandardParse(string[] values, Type targetType, Type elementType)
        {
            var parser = StringParsing.GetParser(elementType);

            var arr = Activator.CreateInstance(elementType.MakeArrayType(), values.Count());
            for (var i = 0; i < values.Length; i++)
                ((Array)arr).SetValue(parser.Parse(values[i], elementType), i);

            return arr;
        }

    }
}