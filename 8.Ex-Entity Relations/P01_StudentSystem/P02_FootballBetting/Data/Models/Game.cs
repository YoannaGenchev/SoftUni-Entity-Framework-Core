using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace P02_FootballBetting.Data.Models
{
    public class Game
    {
        public int GameId { get; set; }


        public int HomeTeamId { get; set; }
        public Team HomeTeam { get; set; } = null!;


        public int AwayTeamId { get; set; }
        public Team AwayTeam { get; set; } = null!;


        public int HomeTeamGoals { get; set; }
        public int AwayTeamGoals { get; set; }
        public decimal HomeTeamBetRate { get; set; }
        public decimal AwayTeamBetRate { get; set; }
        public decimal DrawBetRate { get; set; }
        public DateTime DateTime { get; set; }
        public string? Result { get; set; }


        public ICollection<PlayerStatistic> PlayersStatistics { get; set; } = new List<PlayerStatistic>();
        public ICollection<Bet> Bets { get; set; } = new List<Bet>();
    }
}