namespace Medicines.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;
    using Medicines.Data;
    using Medicines.Data.Models;
    using Medicines.Data.Models.Enums;
    using Medicines.DataProcessor.ImportDtos;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid Data!";
        private const string SuccessfullyImportedPharmacy = "Successfully imported pharmacy - {0} with {1} medicines.";
        private const string SuccessfullyImportedPatient = "Successfully imported patient - {0} with {1} medicines.";
        private static object pDto;

        public static string ImportPharmacies(MedicinesContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var xmlSerializer = new XmlSerializer(typeof(ImportPharmacyDto[]), new XmlRootAttribute("Pharmacies"));
            var pharmacyDtos = (ImportPharmacyDto[])xmlSerializer.Deserialize(new StringReader(xmlString))!;

            var validPharmacies = new List<Pharmacy>();

            foreach (var pharmacyDto in pharmacyDtos)
            {
                if (!IsValid(pharmacyDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                // Validate IsNonStop boolean value
                if (pharmacyDto.IsNonStop.ToLower() != "true" && pharmacyDto.IsNonStop.ToLower() != "false")
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var pharmacy = new Pharmacy
                {
                    Name = pharmacyDto.Name,
                    PhoneNumber = pharmacyDto.PhoneNumber,
                    IsNonStop = bool.Parse(pharmacyDto.IsNonStop)
                };

                var validMedicines = new List<Medicine>();

                foreach (var medicineDto in pharmacyDto.Medicines)
                {
                    if (!IsValid(medicineDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    // Parse dates
                    DateTime productionDate;
                    DateTime expiryDate;

                    if (!DateTime.TryParseExact(medicineDto.ProductionDate, "yyyy-MM-dd",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out productionDate))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (!DateTime.TryParseExact(medicineDto.ExpiryDate, "yyyy-MM-dd",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out expiryDate))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }


                    // Check if production date is before expiry date
                    if (productionDate >= expiryDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    // Check for duplicate medicine (same name and producer in current pharmacy)
                    if (validMedicines.Any(m => m.Name == medicineDto.Name && m.Producer == medicineDto.Producer))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var medicine = new Medicine
                    {
                        Name = medicineDto.Name,
                        Price = medicineDto.Price,
                        Category = (Category)medicineDto.Category,
                        ProductionDate = productionDate,
                        ExpiryDate = expiryDate,
                        Producer = medicineDto.Producer,
                        Pharmacy = pharmacy
                    };

                    validMedicines.Add(medicine);
                }

                pharmacy.Medicines = validMedicines;
                validPharmacies.Add(pharmacy);
                sb.AppendLine(string.Format(SuccessfullyImportedPharmacy, pharmacy.Name, validMedicines.Count));
            }

            context.Pharmacies.AddRange(validPharmacies);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportPatients(MedicinesContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var patientDtos = JsonConvert.DeserializeObject<ImportPatientDto[]>(jsonString)!;

            var validPatients = new List<Patient>();

            foreach (var patientDto in patientDtos)
            {
                if (!IsValid(patientDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var patient = new Patient
                {
                    FullName = patientDto.FullName,
                    AgeGroup = (AgeGroup)patientDto.AgeGroup,
                    Gender = (Gender)patientDto.Gender
                };

                var validMedicineIds = new HashSet<int>();
                foreach (int medId in Dto.Medicines)
                {
                    // Добави тази проверка:
                    if (!context.Medicines.Any(m => m.Id == medId))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (patient.PatientsMedicines.Any(x => x.MedicineId == medId))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    PatientMedicine patientMedicine = new PatientMedicine()
                    {
                        Patient = patient,
                        MedicineId = medId,
                    };

                    patient.PatientsMedicines.Add(patientMedicine);
                }
                foreach (var medicineId in patientDto.Medicines)
                {
                    // Check if medicine exists
                    if (!context.Medicines.Any(m => m.Id == medicineId))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    // Check for duplicates
                    if (validMedicineIds.Contains(medicineId))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var patientMedicine = new PatientMedicine
                    {
                        Patient = patient,
                        MedicineId = medicineId
                    };

                    patient.PatientsMedicines.Add(patientMedicine);
                    validMedicineIds.Add(medicineId);
                }

                validPatients.Add(patient);
                sb.AppendLine(string.Format(SuccessfullyImportedPatient, patient.FullName, patient.PatientsMedicines.Count));
            }

            context.Patients.AddRange(validPatients);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
