using System.Text.Json.Serialization;

namespace CarDealer.DTOs.Export
{
    public class ExportLocalSupplierDto
    {
        [JsonPropertyName("Id")]
        public int Id { get; set; }

        [JsonPropertyName("Name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("PartsCount")]
        public int PartsCount { get; set; }
    }
}