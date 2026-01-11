using System.Text.Json.Serialization;

namespace ProductShop.DTOs.Export
{
    public class ExportSoldProductDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("buyerFirstName")]
        public string BuyerFirstName { get; set; } = null!;

        [JsonPropertyName("buyerLastName")]
        public string BuyerLastName { get; set; } = null!;
    }
}