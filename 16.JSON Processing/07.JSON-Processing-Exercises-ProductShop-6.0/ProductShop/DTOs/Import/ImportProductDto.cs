using System.Text.Json.Serialization;

namespace ProductShop.DTOs.Import
{
    public class ImportProductDto
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("Price")]
        public decimal Price { get; set; }

        [JsonPropertyName("SellerId")]
        public int SellerId { get; set; }

        [JsonPropertyName("BuyerId")]
        public int? BuyerId { get; set; }
    }
}