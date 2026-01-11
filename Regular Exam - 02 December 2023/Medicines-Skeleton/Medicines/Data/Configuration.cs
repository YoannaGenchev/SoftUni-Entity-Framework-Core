namespace Medicines.Data
{
    namespace Medicines.Data
    {
        using global::Medicines.Data.Models;
        using Microsoft.EntityFrameworkCore;

        public class MedicinesContext : DbContext
        {
            public class Configuration
            {
                public static string ConnectionString = @"Server=.;Database=Medicines;Integrated Security=true;TrustServerCertificate=true";
            }
            public MedicinesContext()
            {
            }

            public MedicinesContext(DbContextOptions options)
                : base(options)
            {
            }

            public DbSet<Pharmacy> Pharmacies { get; set; } = null!;
            public DbSet<Medicine> Medicines { get; set; } = null!;
            public DbSet<Patient> Patients { get; set; } = null!;
            public DbSet<PatientMedicine> PatientsMedicines { get; set; } = null!;

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                if (!optionsBuilder.IsConfigured)
                {
                    optionsBuilder
                        .UseSqlServer(Configuration.ConnectionString);
                }
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                // Configure PatientMedicine composite primary key
                modelBuilder.Entity<PatientMedicine>()
                    .HasKey(pm => new { pm.PatientId, pm.MedicineId });

                // Configure Patient -> PatientMedicine relationship
                modelBuilder.Entity<PatientMedicine>()
                    .HasOne(pm => pm.Patient)
                    .WithMany(p => p.PatientsMedicines)
                    .HasForeignKey(pm => pm.PatientId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Configure Medicine -> PatientMedicine relationship
                modelBuilder.Entity<PatientMedicine>()
                    .HasOne(pm => pm.Medicine)
                    .WithMany(m => m.PatientsMedicines)
                    .HasForeignKey(pm => pm.MedicineId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Configure Pharmacy -> Medicine relationship
                modelBuilder.Entity<Medicine>()
                    .HasOne(m => m.Pharmacy)
                    .WithMany(p => p.Medicines)
                    .HasForeignKey(m => m.PharmacyId)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}
