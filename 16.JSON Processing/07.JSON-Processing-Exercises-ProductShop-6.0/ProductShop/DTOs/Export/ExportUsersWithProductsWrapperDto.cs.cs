using System.Text.Json.Serialization;

    namespace ProductShop.DTOs.Export
    {
        public class ExportUsersWithProductsWrapperDto
        {
            [JsonPropertyName("usersCount")]
            public int UsersCount { get; set; }

            [JsonPropertyName("users")]
            public List<ExportUserWithProductsDto> Users { get; set; } = new List<ExportUserWithProductsDto>();
        }

        public class ExportUserWithProductsDto
        {
            [JsonPropertyName("firstName")]
            public string? FirstName { get; set; }

            [JsonPropertyName("lastName")]
            public string LastName { get; set; } = null!;

            [JsonPropertyName("age")]
            public int? Age { get; set; }

            [JsonPropertyName("soldProducts")]
            public ExportSoldProductsWrapperDto SoldProducts { get; set; } = null!;
        }

        public class ExportSoldProductsWrapperDto
        {
            [JsonPropertyName("count")]
            public int Count { get; set; }

            [JsonPropertyName("products")]
            public List<ExportProductDto> Products { get; set; } = new List<ExportProductDto>();
        }

        public class ExportProductDto
        {
            [JsonPropertyName("name")]
            public string Name { get; set; } = null!;

            [JsonPropertyName("price")]
            public decimal Price { get; set; }
        }
    }

