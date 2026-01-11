using Cadastre.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Cadastre.Data
{
    public class CadastreContext : DbContext
    {
        public CadastreContext()
        {
        }

        public CadastreContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<District> Districts { get; set; } = null!;
        public DbSet<Property> Properties { get; set; } = null!;
        public DbSet<Citizen> Citizens { get; set; } = null!;
        public DbSet<PropertyCitizen> PropertiesCitizens { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Composite primary key за PropertyCitizen
            modelBuilder.Entity<PropertyCitizen>()
                .HasKey(pc => new { pc.PropertyId, pc.CitizenId });

            // Many-to-Many relationships
            modelBuilder.Entity<PropertyCitizen>()
                .HasOne(pc => pc.Property)
                .WithMany(p => p.PropertiesCitizens)
                .HasForeignKey(pc => pc.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PropertyCitizen>()
                .HasOne(pc => pc.Citizen)
                .WithMany(c => c.PropertiesCitizens)
                .HasForeignKey(pc => pc.CitizenId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}