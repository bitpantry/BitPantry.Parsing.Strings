using System;

namespace BitPantry.Parsing.Strings.Parsers
{
    public class KeyValuePairParser : IParser
    {
        public virtual bool CanParseType(Type type)
        {
            return
                type.IsKeyValuePairType() &&
                StringParsing.GetParser(type.GetGenericArguments()[0]) != null &&
                StringParsing.GetParser(type.GetGenericArguments()[1]) != null;
        }

        public virtual object Parse(string value, Type targetType)
        {
            if(!CanParseType(targetType))
                throw new ArgumentException($"Cannot parse type, {targetType.FullName}");

            var pieces = value.SplitQuotedString(AppConfigFacade.KeyValuePairStringSplitter);
            if(pieces.Length != 2)
                throw new ArgumentException($"The given value, \"{value}\", cannot be parsed into a {targetType.FullName} - ensure that spliting the value on the delimiter ({AppConfigFacade.KeyValuePairStringSplitter} results in two, and only two, values");

            var genericArguments = targetType.GetGenericArguments();
            var arg1 = StringParsing.GetParser(genericArguments[0]).Parse(pieces[0], genericArguments[0]);
            var arg2 = StringParsing.GetParser(genericArguments[1]).Parse(pieces[1], genericArguments[1]);

            return Activator.CreateInstance(targetType, arg1, arg2);
        }
    }
}
