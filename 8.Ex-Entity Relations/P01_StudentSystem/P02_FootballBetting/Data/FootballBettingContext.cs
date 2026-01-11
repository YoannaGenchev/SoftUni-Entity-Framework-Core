using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using P02_FootballBetting.Data.Models;

namespace P02_FootballBetting.Data
{
    public class FootballBettingContext : DbContext
    {
        public FootballBettingContext(DbContextOptions options) : base(options) { }

        public FootballBettingContext() { }

        public DbSet<Team> Teams => Set<Team>();
        public DbSet<Color> Colors => Set<Color>();
        public DbSet<Town> Towns => Set<Town>();
        public DbSet<Country> Countries => Set<Country>();
        public DbSet<Player> Players => Set<Player>();
        public DbSet<Position> Positions => Set<Position>();
        public DbSet<PlayerStatistic> PlayersStatistics => Set<PlayerStatistic>();
        public DbSet<Game> Games => Set<Game>();
        public DbSet<Bet> Bets => Set<Bet>();
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>(e =>
            {
                e.HasKey(t => t.TeamId);

                e.Property(t => t.Name)
                    .IsRequired()
                    .IsUnicode(true);

                e.Property(t => t.LogoUrl)
                    .IsRequired()
                    .IsUnicode(false);

                e.Property(t => t.Initials)
                    .IsRequired()
                    .IsUnicode(false)
                    .HasMaxLength(3);

                e.Property(t => t.Budget)
                    .HasPrecision(18, 2);

                e.HasOne(t => t.PrimaryKitColor)
                    .WithMany(c => c.PrimaryKitTeams)
                    .HasForeignKey(t => t.PrimaryKitColorId)
                    .OnDelete(DeleteBehavior.NoAction);

                e.HasOne(t => t.SecondaryKitColor)
                    .WithMany(c => c.SecondaryKitTeams)
                    .HasForeignKey(t => t.SecondaryKitColorId)
                    .OnDelete(DeleteBehavior.NoAction);

                e.HasOne(t => t.Town)
                    .WithMany(to => to.Teams)
                    .HasForeignKey(t => t.TownId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Color>(e =>
            {
                e.HasKey(c => c.ColorId);

                e.Property(c => c.Name)
                    .IsRequired();
            });

            modelBuilder.Entity<Town>(e =>
            {
                e.HasKey(t => t.TownId);

                e.Property(t => t.Name)
                    .IsRequired();

                e.HasOne(t => t.Country)
                    .WithMany(c => c.Towns)
                    .HasForeignKey(t => t.CountryId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Country>(e =>
            {
                e.HasKey(c => c.CountryId);

                e.Property(c => c.Name)
                    .IsRequired();
            });

            modelBuilder.Entity<Player>(e =>
            {
                e.HasKey(p => p.PlayerId);

                e.Property(p => p.Name)
                    .IsRequired()
                    .IsUnicode(true);

                e.HasOne(p => p.Position)
                    .WithMany(pos => pos.Players)
                    .HasForeignKey(p => p.PositionId)
                    .OnDelete(DeleteBehavior.NoAction);

                e.HasOne(p => p.Team)
                    .WithMany(t => t.Players)
                    .HasForeignKey(p => p.TeamId)
                    .OnDelete(DeleteBehavior.NoAction);

                e.HasOne(p => p.Town)
                    .WithMany(t => t.Players)
                    .HasForeignKey(p => p.TownId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Position>(e =>
            {
                e.HasKey(p => p.PositionId);

                e.Property(p => p.Name)
                    .IsRequired();
            });

            modelBuilder.Entity<Game>(e =>
            {
                e.HasKey(g => g.GameId);

                e.Property(g => g.HomeTeamBetRate)
                    .HasPrecision(18, 2);

                e.Property(g => g.AwayTeamBetRate)
                    .HasPrecision(18, 2);

                e.Property(g => g.DrawBetRate)
                    .HasPrecision(18, 2);

                e.HasOne(g => g.HomeTeam)
                    .WithMany(t => t.HomeGames)
                    .HasForeignKey(g => g.HomeTeamId)
                    .OnDelete(DeleteBehavior.NoAction);

                e.HasOne(g => g.AwayTeam)
                    .WithMany(t => t.AwayGames)
                    .HasForeignKey(g => g.AwayTeamId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<PlayerStatistic>(e =>
            {
                e.HasKey(ps => new { ps.GameId, ps.PlayerId });

                e.HasOne(ps => ps.Game)
                    .WithMany(g => g.PlayersStatistics)
                    .HasForeignKey(ps => ps.GameId)
                    .OnDelete(DeleteBehavior.NoAction);

                e.HasOne(ps => ps.Player)
                    .WithMany(p => p.PlayersStatistics)
                    .HasForeignKey(ps => ps.PlayerId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Bet>(e =>
            {
                e.HasKey(b => b.BetId);

                e.Property(b => b.Amount)
                    .HasPrecision(18, 2);

                e.Property(b => b.Prediction)
                    .IsRequired()
                    .IsUnicode(false);

                e.HasOne(b => b.Game)
                    .WithMany(g => g.Bets)
                    .HasForeignKey(b => b.GameId)
                    .OnDelete(DeleteBehavior.NoAction);

                e.HasOne(b => b.User)
                    .WithMany(u => u.Bets)
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(u => u.UserId);

                e.Property(u => u.Username)
                    .IsRequired();

                e.Property(u => u.Password)
                    .IsRequired();

                e.Property(u => u.Email)
                    .IsRequired();

                e.Property(u => u.Balance)
                    .HasPrecision(18, 2);
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Server=(localdb)\\MSSQLLocalDB;Database=FootballBetting;Integrated Security=True;TrustServerCertificate=True");
            }
        }
    }
}