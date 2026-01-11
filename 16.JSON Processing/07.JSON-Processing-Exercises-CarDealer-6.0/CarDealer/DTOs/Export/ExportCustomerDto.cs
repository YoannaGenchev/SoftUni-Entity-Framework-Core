using System.Text.Json.Serialization;

namespace CarDealer.DTOs.Export
{
    public class ExportCustomerDto
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("BirthDate")]
        public string BirthDate { get; set; } = null!;

        [JsonPropertyName("IsYoungDriver")]
        public bool IsYoungDriver { get; set; }
    }
}