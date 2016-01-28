using System;

namespace BitPantry.Parsing.Strings.Parsers
{
    // TODO: make this parse from the integer representation as well?

    /// <summary>
    /// A string value parser which parses enumerations from their string value representations
    /// </summary>
    public class EnumParser : IParser
    {
        public bool CanParseType(Type type)
        {
            return type.IsEnum;
        }

        public object Parse(string value, Type targetType)
        {
            if (targetType == null) throw new ArgumentNullException(nameof(targetType));

            try
            {
                if(!CanParseType(targetType))
                    throw new InvalidOperationException($"Cannot parse value, \"{value}\", and type, \"{targetType.FullName}\"");

                var enumValue = Enum.Parse(targetType, value);

                if(!Enum.IsDefined(targetType, enumValue))
                    throw new ArgumentException($"The enum, \"{targetType}\" does not define a value for \"{value}\"");

                return  enumValue;
            }
            catch (ArgumentException)
            {
                throw new ArgumentOutOfRangeException(
                    $"The value, \"{value}\" does not exist for enumeration, \"{targetType.FullName}\"");
            }
        }
    }
}
