using System.Text.Json.Serialization;

namespace ProductShop.DTOs.Export
{
    public class ExportCategoryDto
    {
        [JsonPropertyName("category")]
        public string Category { get; set; } = null!;

        [JsonPropertyName("productsCount")]
        public int ProductsCount { get; set; }

        [JsonPropertyName("averagePrice")]
        public string AveragePrice { get; set; } = null!;

        [JsonPropertyName("totalRevenue")]
        public string TotalRevenue { get; set; } = null!;
    }
}