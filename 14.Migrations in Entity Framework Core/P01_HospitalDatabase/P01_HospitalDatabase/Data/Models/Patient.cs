using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P01_HospitalDatabase.Data.Models
{
    public class Patient
    {
        public int PatientId { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = null!;

        [Required]
        [MaxLength(250)]
        public string Address { get; set; } = null!;

        [Required]
        [MaxLength(80)]
        public string Email { get; set; } = null!;

        public bool HasInsurance { get; set; }

        public ICollection<Visitation> Visitations { get; set; } = new List<Visitation>();
        public ICollection<Diagnose> Diagnoses { get; set; } = new List<Diagnose>();
        public ICollection<PatientMedicament> Prescriptions { get; set; } = new List<PatientMedicament>();
    }
}
