using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Invoices.DataProcessor.ImportDto
{
    public class ImportInvoiceDto
    {
        [JsonProperty("Number")]
        [Range(1000000000, 1500000000)]
        public int Number { get; set; }

        [JsonProperty("IssueDate")]
        [Required]
        public string IssueDate { get; set; } = null!;

        [JsonProperty("DueDate")]
        [Required]
        public string DueDate { get; set; } = null!;

        [JsonProperty("Amount")]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Amount { get; set; }

        [JsonProperty("CurrencyType")]
        [Required]
        public int CurrencyType { get; set; }

        [JsonProperty("ClientId")]
        [Required]
        public int ClientId { get; set; }
    }
}
