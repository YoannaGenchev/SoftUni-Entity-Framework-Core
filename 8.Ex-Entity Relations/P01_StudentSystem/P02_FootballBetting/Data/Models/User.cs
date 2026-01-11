using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace P02_FootballBetting.Data.Models
{
    public class User
    {
        public int UserId { get; set; }
        [Required] public string Username { get; set; } = null!;
        public string? Name { get; set; }
        [Required] public string Password { get; set; } = null!;
        [Required] public string Email { get; set; } = null!;
        public decimal Balance { get; set; }


        public ICollection<Bet> Bets { get; set; } = new List<Bet>();
    }
}