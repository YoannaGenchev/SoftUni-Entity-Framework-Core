using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using P01_StudentSystem.Data.Models.Enums;
using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data.Models;

namespace P01_StudentSystem.Data
{
    public class StudentSystemContext : DbContext
    {
        public StudentSystemContext()
        {
        }

        public StudentSystemContext(DbContextOptions<StudentSystemContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Resource> Resources { get; set; } = null!;
        public DbSet<Homework> Homeworks { get; set; } = null!;
        public DbSet<StudentCourse> StudentsCourses { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.;Database=StudentSystem;Integrated Security=true;TrustServerCertificate=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(entity =>
            {
                entity.Property(s => s.Name)
                    .IsUnicode(true);

                entity.Property(s => s.PhoneNumber)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.Property(c => c.Name)
                    .IsUnicode(true);

                entity.Property(c => c.Description)
                    .IsUnicode(true);
            });

            modelBuilder.Entity<Resource>(entity =>
            {
                entity.Property(r => r.Name)
                    .IsUnicode(true);

                entity.Property(r => r.Url)
                    .IsUnicode(false);

                entity.HasOne(r => r.Course)
                    .WithMany(c => c.Resources)
                    .HasForeignKey(r => r.CourseId);
            });

            modelBuilder.Entity<Homework>(entity =>
            {
                entity.Property(h => h.Content)
                    .IsUnicode(false);

                entity.HasOne(h => h.Student)
                    .WithMany(s => s.Homeworks)
                    .HasForeignKey(h => h.StudentId);

                entity.HasOne(h => h.Course)
                    .WithMany(c => c.Homeworks)
                    .HasForeignKey(h => h.CourseId);
            });

            modelBuilder.Entity<StudentCourse>(entity =>
            {
                entity.HasKey(sc => new { sc.StudentId, sc.CourseId });

                entity.HasOne(sc => sc.Student)
                    .WithMany(s => s.StudentsCourses)
                    .HasForeignKey(sc => sc.StudentId);

                entity.HasOne(sc => sc.Course)
                    .WithMany(c => c.StudentsCourses)
                    .HasForeignKey(sc => sc.CourseId);
            });
        }
    }
}