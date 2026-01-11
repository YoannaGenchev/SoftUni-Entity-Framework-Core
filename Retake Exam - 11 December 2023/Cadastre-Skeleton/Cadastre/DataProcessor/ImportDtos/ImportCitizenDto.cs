using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Cadastre.DTOs.Import
{
    public class ImportCitizenDto
    {
        [Required]
        [MinLength(2)]
        [MaxLength(30)]
        [JsonProperty("FirstName")]
        public string FirstName { get; set; } = null!;

        [Required]
        [MinLength(2)]
        [MaxLength(30)]
        [JsonProperty("LastName")]
        public string LastName { get; set; } = null!;

        [Required]
        [JsonProperty("BirthDate")]
        public string BirthDate { get; set; } = null!;

        [Required]
        [JsonProperty("MaritalStatus")]
        public string MaritalStatus { get; set; } = null!;

        [JsonProperty("Properties")]
        public int[] Properties { get; set; } = null!;
    }
}