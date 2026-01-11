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
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var usersData = context.Users
                .Where(u => u.ProductsSold.Any())
                .OrderByDescending(u => u.ProductsSold.Count)
                .Take(10)
                .Select(u => new
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = u.ProductsSold
                        .OrderByDescending(p => p.Price)
                        .Select(p => new { p.Name, p.Price })
                        .ToArray()
                })
                .AsNoTracking()
                .ToArray();

            var result = new ExportUsersWithProductsDto
            {
                Count = context.Users.Count(u => u.ProductsSold.Any()),
                Users = usersData.Select(u => new ExportUserDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new ExportSoldProductsCountDto
                    {
                        Count = u.SoldProducts.Length,
                        Products = u.SoldProducts.Select(p => new ExportProductDto
                        {
                            Name = p.Name,
                            Price = p.Price
                        }).ToArray()
                    }
                }).ToArray()
            };

            return Serialize(result, "Users");
        }
    }
}