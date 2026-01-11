using Cadastre.DataProcessor.ExportDtos;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Cadastre.DTOs.Import
{
    [XmlType("District")]
    public class ImportDistrictDto
    {
        [Required]
        [MinLength(2)]
        [MaxLength(80)]
        [XmlElement("Name")]
        public string Name { get; set; } = null!;

        [Required]
        [RegularExpression(@"^[A-Z]{2}-\d{5}$")]
        [XmlElement("PostalCode")]
        public string PostalCode { get; set; } = null!;

        [Required]
        [XmlAttribute("Region")]
        public string RegionName { get; set; } = null!;

        [XmlArray("Properties")]
        [XmlArrayItem("Property")]
        public ImportPropertyDto[] Properties { get; set; } = null!;
    }
}