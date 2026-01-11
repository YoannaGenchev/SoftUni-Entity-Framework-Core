using System.Text.Json.Serialization;

namespace CarDealer.DTOs.Export
{
    public class ExportCarWithPartsDto
    {
        [JsonPropertyName("car")]
        public CarInfoDto Car { get; set; } = null!;

        [JsonPropertyName("parts")]
        public List<PartInfoDto> Parts { get; set; } = new List<PartInfoDto>();
    }

    public class CarInfoDto
    {
        [JsonPropertyName("Make")]
        public string Make { get; set; } = null!;

        [JsonPropertyName("Model")]
        public string Model { get; set; } = null!;

        [JsonPropertyName("TraveledDistance")]
        public long TraveledDistance { get; set; }
    }

    public class PartInfoDto
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("Price")]
        public string Price { get; set; } = null!;
    }
}