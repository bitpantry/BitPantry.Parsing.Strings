using System;
using System.Linq;

namespace BitPantry.Parsing.Strings.Parsers
{
    /// <summary>
    /// Parses a string value as a given single value primitive type
    /// </summary>
    /// <typeparam name="TType">The target type</typeparam>
    public class PrimitiveValueParser<TType> : IParser
    {
        public virtual bool CanParseType(Type type)
        {
            return (Nullable.GetUnderlyingType(type) ?? type) == typeof(TType);
        }
       
        public virtual object Parse(string value, Type targetType = null)
        {
            if(value == null)
            {
                if (targetType.IsNullableType())
                    return null;

                throw new ArgumentException($"Cannot parse null value as type {targetType}");
            }
            
            targetType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            object returnValue;

            if (targetType == typeof(bool)) { returnValue = ParseBool(value); }
            else if (targetType == typeof(DateTime)) { returnValue = ParseDateTime(value); }
            else if (targetType == typeof(string)) { returnValue = value; }
            else if (targetType == typeof(char)) { returnValue = value.FirstOrDefault(); }
            else if (targetType == typeof(byte)) { returnValue = ParseNumber<byte>(value); }
            else if (targetType == typeof(sbyte)) { returnValue = ParseNumber<sbyte>(value); }
            else if (targetType == typeof(short)) { returnValue = ParseNumber<short>(value); }
            else if (targetType == typeof(ushort)) { returnValue = ParseNumber<ushort>(value); }
            else if (targetType == typeof(int)) { returnValue = ParseNumber<int>(value); }
            else if (targetType == typeof(uint)) { returnValue = ParseNumber<uint>(value); }
            else if (targetType == typeof(long)) { returnValue = ParseNumber<long>(value); }
            else if (targetType == typeof(ulong)) { returnValue = ParseNumber<ulong>(value); }
            else if (targetType == typeof(float)) { returnValue = ParseNumber<float>(value); }
            else if (targetType == typeof(double)) { returnValue = ParseNumber<double>(value); }
            else if (targetType == typeof(decimal)) { returnValue = ParseNumber<decimal>(value); }
            else
            {
                throw new ArgumentException(
                    string.Format("Cannot parse value as {0}. The conversion is not defined. :: value={1}",
                       targetType.FullName, value));
            }

            return Convert.ChangeType(returnValue, typeof(TType));
        }

        /// <summary>
        /// Parses the given string as the provided generic numeric type
        /// </summary>
        /// <typeparam name="TNumberType">The type of numeric type to parse the number as</typeparam>
        /// <param name="value">The string value of the number to parse</param>
        /// <returns>The parsed value as the data type specified</returns>
        protected virtual TNumberType ParseNumber<TNumberType>(string value)
        {
            // check parameters

            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Cannot parse number for null or empty value");

            // double should handle all floating and integral types, unless the
            // type is decimal, then we need to handle decimal explicitly

            object output = null;

            if (typeof(TNumberType) == typeof(decimal)) // handle decimal type
            {
                decimal outDecimal;
                if (decimal.TryParse(value, out outDecimal))
                    output = outDecimal;
            }
            else // if not a decimal, then treat as double for parsing
            {
                double outDouble;
                if (double.TryParse(value, out outDouble))
                    output = outDouble;
            }

            // evaluate failed parse with null default value

            if (output == null)
                throw new ArgumentException($"Cannot parse value \"{value}\" as a number.");
            else
                return (TNumberType)Convert.ChangeType(output, typeof(TNumberType));
        }

        /// <summary>
        /// Parses a string as a boolean value
        /// </summary>
        /// <param name="value">The string to parse</param>
        /// <returns>The boolean representation of the string</returns>
        /// <example>The function can parse the following values as true - "1", "true", "yes"</example>
        /// <example>The function can parse the following values as false - "0", "false", "no"</example>
        /// <remarks>If the value cannot be parsed, or does not fall into one of the accepted values, the function returns false as a default</remarks>
        protected virtual bool ParseBool(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            if (value.Trim() == "1" || value.Trim().Equals("yes", StringComparison.OrdinalIgnoreCase)
                || value.Trim().Equals("TRUE", StringComparison.OrdinalIgnoreCase))
                return true;
            else if (value.Trim() == "0" || value.Trim().Equals("no", StringComparison.OrdinalIgnoreCase)
                || value.Trim().Equals("FALSE", StringComparison.OrdinalIgnoreCase))
                return false;

            return false;
        }

        /// <summary>
        /// Parses a string as a DateTime value
        /// </summary>
        /// <param name="value">The string value to parse as a DateTime</param>
        /// <returns>The DateTime representation of the string</returns>
        /// <remarks>The function uses the standard DateTime.ParseValue functionality to parse the string</remarks>
        protected virtual DateTime ParseDateTime(string value)
        {
            try
            {
                return DateTime.Parse(value);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Could not parse string value, \"{value}\" as a DateTime", ex);
            }
        }


    }
}
