using P02_FootballBetting.Data.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace P02_FootballBetting.Data.Models
{
    public class Town
    {
        public int TownId { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public int CountryId { get; set; }
        public Country Country { get; set; } = null!;

        public ICollection<Team> Teams { get; set; } = new List<Team>();
        public ICollection<Player> Players { get; set; } = new List<Player>(); 
    }
}