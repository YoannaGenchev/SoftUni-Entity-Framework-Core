using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Cadastre.DTOs.Import
{
    [XmlType("Property")]
    public class ImportPropertyDto
    {
        [Required]
        [MinLength(16)]
        [MaxLength(20)]
        [XmlElement("PropertyIdentifier")]
        public string PropertyIdentifier { get; set; } = null!;

        [Required]
        [Range(0, int.MaxValue)]
        [XmlElement("Area")]
        public int Area { get; set; }

        [MinLength(5)]
        [MaxLength(500)]
        [XmlElement("Details")]
        public string? Details { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(200)]
        [XmlElement("Address")]
        public string Address { get; set; } = null!;

        [Required]
        [XmlElement("DateOfAcquisition")]
        public string DateOfAcquisition { get; set; } = null!;
    }
}