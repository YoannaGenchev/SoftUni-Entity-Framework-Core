using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace P02_FootballBetting.Data.Models
{
    public class Color
    {
        public int ColorId { get; set; }
        [Required]
        public string Name { get; set; } = null!;


        public ICollection<Team> PrimaryKitTeams { get; set; } = new List<Team>();
        public ICollection<Team> SecondaryKitTeams { get; set; } = new List<Team>();
    }
}