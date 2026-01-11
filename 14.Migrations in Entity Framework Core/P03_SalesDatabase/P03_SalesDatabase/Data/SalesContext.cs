using Microsoft.EntityFrameworkCore;
using P03_SalesDatabase.Data.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace P03_SalesDatabase.Data
{
    public class SalesContext : DbContext
    {
        public SalesContext() { }

        public SalesContext(DbContextOptions options) : base(options) { }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Store> Stores => Set<Store>();
        public DbSet<Sale> Sales => Set<Sale>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(e =>
            {
                e.HasKey(p => p.ProductId);

                e.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(true);

                e.Property(p => p.Price)
                    .HasPrecision(18, 2);
            });

            modelBuilder.Entity<Customer>(e =>
            {
                e.HasKey(c => c.CustomerId);

                e.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(true);

                e.Property(c => c.Email)
                    .IsRequired()
                    .HasMaxLength(80)
                    .IsUnicode(false);

                e.Property(c => c.CreditCardNumber)
                    .IsRequired();
            });

            modelBuilder.Entity<Store>(e =>
            {
                e.HasKey(s => s.StoreId);

                e.Property(s => s.Name)
                    .IsRequired()
                    .HasMaxLength(80)
                    .IsUnicode(true);
            });

            modelBuilder.Entity<Sale>(e =>
            {
                e.HasKey(s => s.SaleId);

                e.HasOne(s => s.Product)
                    .WithMany(p => p.Sales)
                    .HasForeignKey(s => s.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(s => s.Customer)
                    .WithMany(c => c.Sales)
                    .HasForeignKey(s => s.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(s => s.Store)
                    .WithMany(st => st.Sales)
                    .HasForeignKey(s => s.StoreId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Server=(localdb)\\MSSQLLocalDB;Database=SalesDatabase;Integrated Security=True;TrustServerCertificate=True");
            }
        }
    }
}