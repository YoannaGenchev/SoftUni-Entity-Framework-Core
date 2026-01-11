using System.Text.Json.Serialization;

namespace ProductShop.DTOs.Import
{
    public class ImportCategoryProductDto
    {
        [JsonPropertyName("CategoryId")]
        public int CategoryId { get; set; }

        [JsonPropertyName("ProductId")]
        public int ProductId { get; set; }
    }
}