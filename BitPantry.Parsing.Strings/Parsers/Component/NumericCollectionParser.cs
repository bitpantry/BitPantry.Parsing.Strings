using System;
using System.Collections;
using System.Collections.Generic;

namespace BitPantry.Parsing.Strings.Parsers.Component
{
    /// <summary>
    /// Provides shared functions for parsing numeric lists - these functions are used by multiple parsers
    /// </summary>
    internal static class NumericCollectionParser
    {
        /// <summary>
        /// Parses a list of numbers separated by a comma. This function can also interpret ranges within the
        /// list.
        /// </summary>
        /// <param name="values">The string value representation of the list</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="elementType">The target element type</param>
        /// <returns>
        /// The <see cref="ICollection{T}"/> implementation of <paramref name="targetType" /> containing the parsed values
        /// </returns>
        /// <exception cref="System.ArgumentException">The given target type is not an implementation of <see cref="ICollection{T}"/>, or a parser could not 
        /// be found for the given <paramref name="elementType"/></exception>
        /// <exception cref="System.Exception"><paramref name="values"/> could not be parsed as a an implementation of <see cref="ICollection{T}"/>. See the inner exception for more details</exception>
        /// <example>"1,2,3,4,5,6,7,8,9,10"</example>
        /// <example>"1-10"</example>
        /// <example>"1,2,3-8,9,10"</example>
        internal static object ParseCollection(string[] values, Type targetType, Type elementType)
        {
            try
            {
                if(!targetType.IsGenericCollectionImplementation())
                    throw new ArgumentException($"{targetType.FullName} does not implement {typeof(ICollection<>).FullName}");

                var arr = Activator.CreateInstance(targetType);
                var parser = StringParsing.GetParser(elementType);

                if(parser == null)
                    throw new ArgumentException($"No parser could be found for element type, {elementType.FullName}");

                foreach (var value in values)
                {
                    if (value.Contains("-")) // parse range
                    {
                        var bounds = value.Split('-');
                        var range = Activator.CreateInstance(targetType);

                        // double should handle all floating and integral types, unless
                        // the type is decimal, then we need to handle decimal
                        // explicitly

                        if (elementType == typeof(decimal))
                        {
                            for (var i = (decimal)parser.Parse(bounds[0].Trim(), elementType);
                                i <= (decimal)parser.Parse(bounds[1].Trim(), elementType);
                                i++)
                                targetType.GetMethod("Add")
                                    .Invoke(range, new[] { Convert.ChangeType(i, elementType) });
                        }
                        else
                        {
                            for (var i = (double)Convert.ChangeType(parser.Parse(bounds[0].Trim(), elementType), typeof(double));
                                i <= (double)Convert.ChangeType(parser.Parse(bounds[1].Trim(), elementType), typeof(double));
                                i++)
                                targetType.GetMethod("Add")
                                    .Invoke(range, new[] { Convert.ChangeType(i, elementType) });

                        }

                        foreach (var item in (IList)range)
                            targetType.GetMethod("Add").Invoke(arr, new[] { item });
                    }
                    else
                    {
                        targetType.GetMethod("Add").Invoke(arr, new[] { parser.Parse(value, elementType) });
                    }
                }

                return arr;
            }
            catch (Exception ex)
            {
                throw new Exception($"Values could not be parsed as a {typeof(ICollection<>).FullName}. See the inner exception for more details", ex);
            }
        }
    }
}
