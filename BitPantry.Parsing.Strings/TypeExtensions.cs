using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BitPantry.Parsing.Strings.Parsers;

namespace BitPantry.Parsing.Strings
{
    /// <summary>
    /// Provides helper extensions for working with types
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Determines whether a type is an implementation of <see cref="ICollection{T}"/>
        /// </summary>
        /// <param name="type">The type to test</param>
        /// <returns>Whether or not the type implements <see cref="ICollection{T}"/></returns>
        /// <remarks>True if the type implements <see cref="ICollection{T}"/>, false otherwise</remarks>
        public static bool IsGenericCollectionImplementation(this Type type)
        {
            if (type.IsArray) // for some reason the return statement below which evaluates interfaces will return true for arrays like char[]
                return false;

            return
                type.GetInterfaces()
                    .Where(i => i.IsGenericType)
                    .Select(i => i.GetGenericTypeDefinition())
                    .Any(i => i == typeof (ICollection<>));
        }

        /// <summary>
        /// Determines whether a type is an implementation of <see cref="IDictionary{TKey,TValue}"/>
        /// </summary>
        /// <param name="type">The type to test</param>
        /// <returns>Whether or not the type implements <see cref="IDictionary{TKey,TValue}"/></returns>
        /// <remarks>True if the type implements <see cref="IDictionary{TKey,TValue}"/>, false otherwise</remarks>
        public static bool IsGenericDictionaryImplementation(this Type type)
        {
            if (type.IsArray) // for some reason the return statement below which evaluates interfaces will return true for arrays like char[]
                return false;

            return
                type.GetInterfaces()
                    .Where(i => i.IsGenericType)
                    .Select(i => i.GetGenericTypeDefinition())
                    .Any(i => i == typeof(IDictionary<,>));
        }

        /// <summary>
        /// Determines whether the given type is a <see cref="KeyValuePair{TKey,TValue}"/>/>
        /// </summary>
        /// <param name="type">The type to test</param>
        /// <returns>True if the type is a <see cref="KeyValuePair{TKey,TValue}"/>, otherwise false</returns>
        /// <remarks>Does not test for extensions, because <see cref="KeyValuePair{TKey,TValue}"/> is sealed</remarks>
        public static bool IsKeyValuePairType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof (KeyValuePair<,>);
        }

        /// <summary>
        /// Determines whether the given type implements the <see cref="IParser"/> interface
        /// </summary>
        /// <param name="type">The type to evaluate</param>
        /// <returns>True if the type implements the <see cref="IParser"/> interface, otherwise false</returns>
        public static bool ImplementsIParser(this Type type)
        {
            return type.GetInterfaces().Any(i => i == typeof(IParser));
        }

        /// <summary>
        /// Returns the element type for a given <see cref="ICollection{T}"/> implementation type
        /// </summary>
        /// <param name="type">The <see cref="ICollection{T}"/> implementation type to return the element type for</param>
        /// <returns>The element type</returns>
        /// <remarks>The type must implement <see cref="ICollection{T}"/></remarks>
        public static Type GetGenericCollectionElementType(this Type type)
        {
            if(!type.IsGenericCollectionImplementation())
                throw new ArgumentException($"Type {type.FullName} is not a generic collection implementation");

            return type
                .GetInterfaces()
                .Single(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof (ICollection<>))
                .GetGenericArguments()
                .Single();
        }

        public static MethodInfo GetGenericCollectionInterfaceMappedAddMethod(this Type type)
        {
            if(!type.IsGenericCollectionImplementation())
                throw new ArgumentException($"Type {type.FullName} is not a generic collection implementation");

            return type.GetInterfaceMap(typeof (ICollection<>).MakeGenericType(type.GetGenericCollectionElementType()))
                .InterfaceMethods.FirstOrDefault(m => m.Name.Equals("Add"));
        }

        /// <summary>
        /// Determines if a type is numeric.  Nullable numeric types are considered numeric.
        /// </summary>
        /// <remarks>
        /// Boolean is not considered numeric.
        /// </remarks>
        public static bool IsNumericType(this Type type)
        {
            if (type == null)
                return false;

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
            }

            if (type.IsNullableType())
                return IsNumericType(Nullable.GetUnderlyingType(type));

            return false;
        }

        /// <summary>
        /// Determines whether given type is nullable
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>True if the given type is nullable, otherwise false</returns>
        public static bool IsNullableType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>); 
        }

    }
}
