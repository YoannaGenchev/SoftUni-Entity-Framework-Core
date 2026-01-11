using System;
using System.ComponentModel.DataAnnotations;


namespace P02_FootballBetting.Data.Models
{
    public class Bet
    {
        public int BetId { get; set; }
        public decimal Amount { get; set; }
        [Required]
        public string Prediction { get; set; } = null!; // non-unicode via Fluent
        public DateTime DateTime { get; set; }


        public int UserId { get; set; }
        public User User { get; set; } = null!;


        public int GameId { get; set; }
        public Game Game { get; set; } = null!;
    }
}