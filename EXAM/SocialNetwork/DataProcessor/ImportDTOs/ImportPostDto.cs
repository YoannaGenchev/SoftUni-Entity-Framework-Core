using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.DataProcessor.ImportDTOs
{
    public class ImportPostDto
    {
        [JsonProperty("Content")]
        [Required]
        [MinLength(5)]
        [MaxLength(300)]
        public string Content { get; set; } = null!;

        [JsonProperty("CreatedAt")]
        [Required]
        public string CreatedAt { get; set; } = null!;

        [JsonProperty("CreatorId")]
        [Required]
        public int CreatorId { get; set; }
    }
}
