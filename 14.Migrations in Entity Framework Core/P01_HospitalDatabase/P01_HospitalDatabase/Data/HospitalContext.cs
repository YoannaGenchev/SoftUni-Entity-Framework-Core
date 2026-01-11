using Microsoft.EntityFrameworkCore;
using P01_HospitalDatabase.Data.Models;

namespace P01_HospitalDatabase.Data
{
    public class HospitalContext : DbContext
    {
        public HospitalContext() { }

        public HospitalContext(DbContextOptions options) : base(options) { }

        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Visitation> Visitations => Set<Visitation>();
        public DbSet<Diagnose> Diagnoses => Set<Diagnose>();
        public DbSet<Medicament> Medicaments => Set<Medicament>();
        public DbSet<PatientMedicament> PatientsMedicaments => Set<PatientMedicament>();
        public DbSet<Doctor> Doctors => Set<Doctor>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>(e =>
            {
                e.HasKey(p => p.PatientId);

                e.Property(p => p.FirstName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(true);

                e.Property(p => p.LastName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(true);

                e.Property(p => p.Address)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(true);

                e.Property(p => p.Email)
                    .IsRequired()
                    .HasMaxLength(80)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Visitation>(e =>
            {
                e.HasKey(v => v.VisitationId);

                e.Property(v => v.Comments)
                    .HasMaxLength(250)
                    .IsUnicode(true);

                e.HasOne(v => v.Patient)
                    .WithMany(p => p.Visitations)
                    .HasForeignKey(v => v.PatientId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(v => v.Doctor)
                    .WithMany(d => d.Visitations)
                    .HasForeignKey(v => v.DoctorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Diagnose>(e =>
            {
                e.HasKey(d => d.DiagnoseId);

                e.Property(d => d.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(true);

                e.Property(d => d.Comments)
                    .HasMaxLength(250)
                    .IsUnicode(true);

                e.HasOne(d => d.Patient)
                    .WithMany(p => p.Diagnoses)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Medicament>(e =>
            {
                e.HasKey(m => m.MedicamentId);

                e.Property(m => m.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(true);
            });

            modelBuilder.Entity<PatientMedicament>(e =>
            {
                e.HasKey(pm => new { pm.PatientId, pm.MedicamentId });

                e.HasOne(pm => pm.Patient)
                    .WithMany(p => p.Prescriptions)
                    .HasForeignKey(pm => pm.PatientId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(pm => pm.Medicament)
                    .WithMany(m => m.Prescriptions)
                    .HasForeignKey(pm => pm.MedicamentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Doctor>(e =>
            {
                e.HasKey(d => d.DoctorId);

                e.Property(d => d.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(true);

                e.Property(d => d.Specialty)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(true);
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Server=(localdb)\\MSSQLLocalDB;Database=Hospital;Integrated Security=True;TrustServerCertificate=True");
            }
        }
    }
}