using System.Collections.Generic;
using System.Xml.Serialization;

namespace BitPantry.Parsing.Strings.Configuration
{
    /// <summary>
    /// Configuration handler for custom string parsers
    /// </summary>
    public class StringValueParserConfiguration : ConfigurationHandler
    {
        [XmlArray("parsers")]
        [XmlArrayItem("add")]
        public List<StringValueParserConfigurationItem> Items { get; set; }
    }
}
