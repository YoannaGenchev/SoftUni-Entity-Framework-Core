using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Medicines.Utilities
{
    public class XmlHelper
    {
        public T Deserialize<T>(string inputXml, string rootName)
        {
            var xmlRoot = new XmlRootAttribute(rootName);
            var serializer = new XmlSerializer(typeof(T), xmlRoot);

            using var reader = new StringReader(inputXml);
            return (T)serializer.Deserialize(reader)!;
        }

        public string Serialize<T>(T obj, string rootName)
        {
            var sb = new StringBuilder();

            var xmlRoot = new XmlRootAttribute(rootName);
            var serializer = new XmlSerializer(typeof(T), xmlRoot);

            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            using var writer = new StringWriter(sb);
            serializer.Serialize(writer, obj, ns);

            return sb.ToString().TrimEnd();
        }
    }
}
