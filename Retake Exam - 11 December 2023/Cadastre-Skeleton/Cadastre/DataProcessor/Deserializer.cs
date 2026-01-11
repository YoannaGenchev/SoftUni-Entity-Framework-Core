using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;
using Cadastre.Data;
using Cadastre.Data.Enumerations;
using Cadastre.Data.Models;
using Cadastre.DTOs.Import; 
using Newtonsoft.Json;


namespace Cadastre.DataProcessor
{

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid Data!";
        private const string SuccessfullyImportedDistrict = "Successfully imported district - {0} with {1} properties.";
        private const string SuccessfullyImportedCitizen = "Succefully imported citizen - {0} {1} with {2} properties.";

        public static string ImportDistricts(CadastreContext dbContext, string xmlDocument)
        {
            StringBuilder sb = new StringBuilder();

            XmlSerializer serializer = new XmlSerializer(typeof(ImportDistrictDto[]), new XmlRootAttribute("Districts"));
            ImportDistrictDto[] districtDtos;

            using (StringReader reader = new StringReader(xmlDocument))
            {
                districtDtos = (ImportDistrictDto[])serializer.Deserialize(reader)!;
            }

            List<District> districts = new List<District>();

            foreach (var districtDto in districtDtos)
            {
                // Validate District
                if (!IsValid(districtDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                // Check if Region is valid enum
                if (!Enum.TryParse<Region>(districtDto.RegionName, out Region region))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                // Check for duplicate district name
                if (districts.Any(d => d.Name == districtDto.Name) ||
                    dbContext.Districts.Any(d => d.Name == districtDto.Name))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                District district = new District
                {
                    Name = districtDto.Name,
                    PostalCode = districtDto.PostalCode,
                    Region = region
                };

                // Process Properties
                foreach (var propertyDto in districtDto.Properties)
                {
                    if (!IsValid(propertyDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    // Parse DateTime
                    if (!DateTime.TryParseExact(propertyDto.DateOfAcquisition, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfAcquisition))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    // Check for duplicate PropertyIdentifier
                    if (district.Properties.Any(p => p.PropertyIdentifier == propertyDto.PropertyIdentifier) ||
                        dbContext.Properties.Any(p => p.PropertyIdentifier == propertyDto.PropertyIdentifier))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    // Check for duplicate Address
                    if (district.Properties.Any(p => p.Address == propertyDto.Address) ||
                        dbContext.Properties.Any(p => p.Address == propertyDto.Address))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Property property = new Property
                    {
                        PropertyIdentifier = propertyDto.PropertyIdentifier,
                        Area = propertyDto.Area,
                        Details = propertyDto.Details,
                        Address = propertyDto.Address,
                        DateOfAcquisition = dateOfAcquisition
                    };

                    district.Properties.Add(property);
                }

                districts.Add(district);
                sb.AppendLine(string.Format(SuccessfullyImportedDistrict, district.Name, district.Properties.Count));
            }

            dbContext.Districts.AddRange(districts);
            dbContext.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(ImportPropertyDto propertyDto)
        {
            throw new NotImplementedException();
        }

        private static bool IsValid(ImportDistrictDto districtDto)
        {
            throw new NotImplementedException();
        }

        public static string ImportCitizens(CadastreContext dbContext, string jsonDocument)
        {
            StringBuilder sb = new StringBuilder();

            ImportCitizenDto[] citizenDtos = JsonConvert.DeserializeObject<ImportCitizenDto[]>(jsonDocument)!;

            List<Citizen> citizens = new List<Citizen>();

            foreach (var citizenDto in citizenDtos)
            {
                if (!IsValid(citizenDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                // Parse BirthDate
                if (!DateTime.TryParseExact(citizenDto.BirthDate, "dd-MM-yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthDate))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                // Validate MaritalStatus
                if (!Enum.TryParse<MaritalStatus>(citizenDto.MaritalStatus, out MaritalStatus maritalStatus))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Citizen citizen = new Citizen
                {
                    FirstName = citizenDto.FirstName,
                    LastName = citizenDto.LastName,
                    BirthDate = birthDate,
                    MaritalStatus = maritalStatus
                };

                // Add Properties
                foreach (var propertyId in citizenDto.Properties)
                {
                    PropertyCitizen propertyCitizen = new PropertyCitizen
                    {
                        Citizen = citizen,
                        PropertyId = propertyId
                    };

                    citizen.PropertiesCitizens.Add(propertyCitizen);
                }

                citizens.Add(citizen);
                sb.AppendLine(string.Format(SuccessfullyImportedCitizen, citizen.FirstName, citizen.LastName, citizen.PropertiesCitizens.Count));
            }

            dbContext.Citizens.AddRange(citizens);
            dbContext.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(ImportCitizenDto citizenDto)
        {
            throw new NotImplementedException();
        }
    }
}