using P02_FootballBetting.Data.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace P02_FootballBetting.Data.Models
{
    public class Position
    {
        public int PositionId { get; set; }
        [Required]
        public string Name { get; set; } = null!;


        public ICollection<Player> Players { get; set; } = new List<Player>();
    }
}