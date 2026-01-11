using System.Text.Json.Serialization;

namespace ProductShop.DTOs.Export
{
    public class ExportUserWithSoldProductsDto
    {
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = null!;

        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = null!;

        [JsonPropertyName("soldProducts")]
        public List<ExportSoldProductDto> SoldProducts { get; set; } = new List<ExportSoldProductDto>();
    }
}
