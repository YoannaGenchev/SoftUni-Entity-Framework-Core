using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace P03_SalesDatabase.Data.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(80)]
        public string Email { get; set; } = null!;

        [Required]
        public string CreditCardNumber { get; set; } = null!;

        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}