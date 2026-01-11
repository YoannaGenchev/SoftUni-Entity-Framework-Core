using Microsoft.EntityFrameworkCore;
using MusicHub.Data.Models;

namespace MusicHub.Data
{
    public class MusicHubDbContext : DbContext
    {
        public MusicHubDbContext()
        {
        }

        public MusicHubDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Song> Songs => Set<Song>();
        public DbSet<Album> Albums => Set<Album>();
        public DbSet<Performer> Performers => Set<Performer>();
        public DbSet<Producer> Producers => Set<Producer>();
        public DbSet<Writer> Writers => Set<Writer>();
        public DbSet<SongPerformer> SongsPerformers => Set<SongPerformer>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Song>(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.Property(s => s.Name)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(s => s.CreatedOn)
                    .IsRequired();

                entity.Property(s => s.Genre)
                    .IsRequired();

                entity.Property(s => s.Price)
                    .IsRequired()
                    .HasPrecision(18, 2);

                entity.HasOne(s => s.Album)
                    .WithMany(a => a.Songs)
                    .HasForeignKey(s => s.AlbumId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Writer)
                    .WithMany(w => w.Songs)
                    .HasForeignKey(s => s.WriterId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Album>(entity =>
            {
                entity.HasKey(a => a.Id);

                entity.Property(a => a.Name)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(a => a.ReleaseDate)
                    .IsRequired();

                entity.HasOne(a => a.Producer)
                    .WithMany(p => p.Albums)
                    .HasForeignKey(a => a.ProducerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Performer>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.FirstName)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(p => p.LastName)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(p => p.Age)
                    .IsRequired();

                entity.Property(p => p.NetWorth)
                    .IsRequired()
                    .HasPrecision(18, 2);
            });

            builder.Entity<Producer>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(30);
            });

            builder.Entity<Writer>(entity =>
            {
                entity.HasKey(w => w.Id);

                entity.Property(w => w.Name)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            builder.Entity<SongPerformer>(entity =>
            {
                entity.HasKey(sp => new { sp.SongId, sp.PerformerId });

                entity.HasOne(sp => sp.Song)
                    .WithMany(s => s.SongPerformers)
                    .HasForeignKey(sp => sp.SongId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(sp => sp.Performer)
                    .WithMany(p => p.PerformerSongs)
                    .HasForeignKey(sp => sp.PerformerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}