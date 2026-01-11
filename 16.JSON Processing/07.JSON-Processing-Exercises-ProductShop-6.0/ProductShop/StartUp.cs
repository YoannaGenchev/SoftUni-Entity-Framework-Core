using System.Text.Json;
using ProductShop.Data;
using ProductShop.Models;
using ProductShop.DTOs.Import;
using ProductShop.DTOs.Export;
using Microsoft.EntityFrameworkCore;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            using ProductShopContext context = new ProductShopContext();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            //string usersJson = File.ReadAllText("../../../Datasets/users.json");
            //Console.WriteLine(ImportUsers(context, usersJson));

            //string productsJson = File.ReadAllText("../../../Datasets/products.json");
            //Console.WriteLine(ImportProducts(context, productsJson));

            //string categoriesJson = File.ReadAllText("../../../Datasets/categories.json");
            //Console.WriteLine(ImportCategories(context, categoriesJson));

            //string categoryProductsJson = File.ReadAllText("../../../Datasets/categories-products.json");
            //Console.WriteLine(ImportCategoryProducts(context, categoryProductsJson));
        }

        //private static bool ImportCategoryProducts(ProductShopContext context, string categoryProductsJson)
        //{
        //    throw new NotImplementedException();
        //}


        /*        public static string ImportUsers(ProductShopContext context, string inputJson)
                {
                    var userDtos = JsonSerializer.Deserialize<List<ImportUserDto>>(inputJson);

                    var users = userDtos
                        .Select(dto => new User
                        {
                            FirstName = dto.FirstName,
                            LastName = dto.LastName,
                            Age = dto.Age
                        })
                        .ToList();

                    context.Users.AddRange(users);
                    context.SaveChanges();

                    return $"Successfully imported {users.Count}";
                }*/

        /*public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var productDtos = JsonSerializer.Deserialize<List<ImportProductDto>>(inputJson);

            var products = productDtos
                .Select(dto => new Product
                {
                    Name = dto.Name,
                    Price = dto.Price,
                    SellerId = dto.SellerId,
                    BuyerId = dto.BuyerId
                })
                .ToList();

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }*/

        /*    public static string ImportCategories(ProductShopContext context, string inputJson)
            {
                var categoryDtos = JsonSerializer.Deserialize<List<ImportCategoryDto>>(inputJson);

                var categories = categoryDtos
                    .Where(dto => dto.Name != null) // Skip null names
                    .Select(dto => new Category
                    {
                        Name = dto.Name!
                    })
                    .ToList();

                context.Categories.AddRange(categories);
                context.SaveChanges();

                return $"Successfully imported {categories.Count}";
            }*/

        /*        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
                {
                    var categoryProductDtos = JsonSerializer.Deserialize<List<ImportCategoryProductDto>>(inputJson);

                    var categoryProducts = categoryProductDtos
                        .Select(dto => new CategoryProduct
                        {
                            CategoryId = dto.CategoryId,
                            ProductId = dto.ProductId
                        })
                        .ToList();

                    context.CategoriesProducts.AddRange(categoryProducts);
                    context.SaveChanges();

                    return $"Successfully imported {categoryProducts.Count}";
                }*/

        /*        public static string GetProductsInRange(ProductShopContext context)
                {
                    var products = context.Products
                        .Where(p => p.Price >= 500 && p.Price <= 1000)
                        .OrderBy(p => p.Price)
                        .Select(p => new ExportProductInRangeDto
                        {
                            Name = p.Name,
                            Price = p.Price,
                            Seller = $"{p.Seller.FirstName} {p.Seller.LastName}"
                        })
                        .AsNoTracking()
                        .ToList();

                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true
                    };

                    return JsonSerializer.Serialize(products, options);
                }*/

        /* public static string GetSoldProducts(ProductShopContext context)
         {
             var users = context.Users
                 .Where(u => u.ProductsSold.Any(p => p.BuyerId != null))
                 .OrderBy(u => u.LastName)
                 .ThenBy(u => u.FirstName)
                 .Select(u => new ExportUserWithSoldProductsDto
                 {
                     FirstName = u.FirstName,
                     LastName = u.LastName,
                     SoldProducts = u.ProductsSold
                         .Where(p => p.BuyerId != null)
                         .Select(p => new ExportSoldProductDto
                         {
                             Name = p.Name,
                             Price = p.Price,
                             BuyerFirstName = p.Buyer!.FirstName,
                             BuyerLastName = p.Buyer.LastName
                         })
                         .ToList()
                 })
                 .AsNoTracking()
                 .ToList();

             var options = new JsonSerializerOptions
             {
                 WriteIndented = true
             };

             return JsonSerializer.Serialize(users, options);
         }*/

        /*       public static string GetCategoriesByProductsCount(ProductShopContext context)
               {
                   var categories = context.Categories
                       .OrderByDescending(c => c.CategoriesProducts.Count)
                       .Select(c => new ExportCategoryDto
                       {
                           Category = c.Name,
                           ProductsCount = c.CategoriesProducts.Count,
                           AveragePrice = c.CategoriesProducts
                               .Average(cp => cp.Product.Price)
                               .ToString("F2"),
                           TotalRevenue = c.CategoriesProducts
                               .Sum(cp => cp.Product.Price)
                               .ToString("F2")
                       })
                       .AsNoTracking()
                       .ToList();

                   var options = new JsonSerializerOptions
                   {
                       WriteIndented = true
                   };

                   return JsonSerializer.Serialize(categories, options);
               }*/

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Any(p => p.BuyerId != null))
                .Select(u => new
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = u.ProductsSold
                        .Where(p => p.BuyerId != null)
                        .Select(p => new
                        {
                            Name = p.Name,
                            Price = p.Price
                        })
                        .ToList()
                })
                .OrderByDescending(u => u.SoldProducts.Count)
                .AsNoTracking()
                .ToList();

            var result = new ExportUsersWithProductsWrapperDto
            {
                UsersCount = users.Count,
                Users = users
                    .Select(u => new ExportUserWithProductsDto
                    {
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Age = u.Age,
                        SoldProducts = new ExportSoldProductsWrapperDto
                        {
                            Count = u.SoldProducts.Count,
                            Products = u.SoldProducts
                                .Select(p => new ExportProductDto
                                {
                                    Name = p.Name,
                                    Price = p.Price
                                })
                                .ToList()
                        }
                    })
                    .ToList()
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            return JsonSerializer.Serialize(result, options);
        }

    }
}