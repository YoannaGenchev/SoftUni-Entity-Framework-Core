using System.Text;
using System.Xml.Serialization;
using ProductShop.Data;
using ProductShop.DTOs.Import;
using ProductShop.DTOs.Export;
using ProductShop.Models;
using Microsoft.EntityFrameworkCore;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            using var context = new ProductShopContext();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

        }

        private static T Deserialize<T>(string inputXml, string rootName)
        {
            var serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(rootName));
            using var reader = new StringReader(inputXml);
            return (T)serializer.Deserialize(reader)!;
        }

        private static string Serialize<T>(T obj, string rootName)
        {
            var serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(rootName));
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using var writer = new StringWriter();
            serializer.Serialize(writer, obj, namespaces);
            return writer.ToString();
        }
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Take(10)
                .Select(p => new ExportProductInRangeDto
                {
                    Name = p.Name,
                    Price = p.Price,
                    Buyer = p.Buyer.FirstName + " " + p.Buyer.LastName
                })
                .ToArray();

            return SerializeToXml(products, "Products");
        }

        private static string SerializeToXml<T>(T[] objects, string rootName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T[]),
                new XmlRootAttribute(rootName));

            StringBuilder sb = new StringBuilder();
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using (StringWriter writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, objects, namespaces);
            }

            return sb.ToString().TrimEnd();
        }

    }
}