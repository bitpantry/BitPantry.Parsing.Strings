using System.Xml.Serialization;

namespace BitPantry.Parsing.Strings.Configuration
{
    /// <summary>
    /// An specific parser in the string value parser configuration section
    /// </summary>
    [XmlRoot]
    public class StringValueParserConfigurationItem
    {
        [XmlAttribute("type")]
        public string Type { get; set; }
    }
}
