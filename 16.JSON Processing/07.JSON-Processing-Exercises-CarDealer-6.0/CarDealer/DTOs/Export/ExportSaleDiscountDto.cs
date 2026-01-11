using System.Text.Json.Serialization;

namespace CarDealer.DTOs.Export
{
    public class ExportSaleDiscountDto
    {
        [JsonPropertyName("car")]
        public CarInfoDto Car { get; set; } = null!;

        [JsonPropertyName("customerName")]
        public string CustomerName { get; set; } = null!;

        [JsonPropertyName("discount")]
        public string Discount { get; set; } = null!;

        [JsonPropertyName("price")]
        public string Price { get; set; } = null!;

        [JsonPropertyName("priceWithDiscount")]
        public string PriceWithDiscount { get; set; } = null!;
    }
}