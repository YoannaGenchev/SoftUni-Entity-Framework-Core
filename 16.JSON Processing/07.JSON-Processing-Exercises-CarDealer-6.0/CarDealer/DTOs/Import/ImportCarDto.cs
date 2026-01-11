using System.Text.Json.Serialization;

namespace CarDealer.DTOs.Import
{
    public class ImportCarDto
    {
        [JsonPropertyName("make")]
        public string Make { get; set; } = null!;

        [JsonPropertyName("model")]
        public string Model { get; set; } = null!;

        [JsonPropertyName("traveledDistance")]
        public long TraveledDistance { get; set; }

        [JsonPropertyName("partsId")]
        public List<int> PartsId { get; set; } = new List<int>();
    }
}