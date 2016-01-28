using System.Configuration;
using System.Xml;
using System.Xml.Serialization;

namespace BitPantry.Parsing.Strings.Configuration
{
    /// <summary>
    /// Generic configuration handler meant to be extended by a concrete configuration block handler
    /// </summary>
    public abstract class ConfigurationHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            var xRoot = new XmlRootAttribute { ElementName = section.Name, IsNullable = true };
            var ser = new XmlSerializer(GetType(), xRoot);
            var xNodeReader = new XmlNodeReader(section);
            return ser.Deserialize(xNodeReader);
        }
    }
}
