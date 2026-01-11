using System.Text.Json.Serialization;

namespace ProductShop.DTOs.Export
{
    public class ExportProductInRangeDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("seller")]
        public string Seller { get; set; } = null!;
    }
}