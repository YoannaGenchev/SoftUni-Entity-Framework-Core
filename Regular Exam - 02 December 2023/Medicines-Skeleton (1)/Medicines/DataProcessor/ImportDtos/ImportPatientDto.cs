using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Medicines.DataProcessor.ImportDtos
{
    public class ImportPatientDto
    {
        [JsonProperty("FullName")]
        [Required]
        [MinLength(5)]
        [MaxLength(100)]
        public string FullName { get; set; } = null!;

        [JsonProperty("AgeGroup")]
        [Required]
        public string AgeGroup { get; set; } = null!;

        [JsonProperty("Gender")]
        [Required]
        public string Gender { get; set; } = null!;

        [JsonProperty("Medicines")]
        public int[] Medicines { get; set; } = Array.Empty<int>();
    }
}
