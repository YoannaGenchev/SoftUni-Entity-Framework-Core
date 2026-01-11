using System.Text.Json.Serialization;

namespace CarDealer.DTOs.Export
{
    public class ExportCustomerSalesDto
    {
        [JsonPropertyName("fullName")]
        public string FullName { get; set; } = null!;

        [JsonPropertyName("boughtCars")]
        public int BoughtCars { get; set; }

        [JsonPropertyName("spentMoney")]
        public decimal SpentMoney { get; set; }
    }
}