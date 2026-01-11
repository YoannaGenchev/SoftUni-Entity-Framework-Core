using Microsoft.EntityFrameworkCore.Migrations;
using P01_StudentSystem.Data.Models.Enums;
using P01_StudentSystem.Migrations;
using System;
using System.ComponentModel.DataAnnotations;

namespace P01_StudentSystem.Data.Models
{
    public class Resource
    {
        [Key]
        public int ResourceId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        [Required]
        public string Url { get; set; } = null!;

        [Required]
        public ResourceType ResourceType { get; set; }

        [Required]
        public int CourseId { get; set; }

        public virtual Course Course { get; set; } = null!;
    }
}
