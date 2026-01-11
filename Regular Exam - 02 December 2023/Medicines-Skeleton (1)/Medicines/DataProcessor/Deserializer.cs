using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;

using Medicines.Data;
using Medicines.Data.Models;
using Medicines.Data.Models.Enums;
using Medicines.DataProcessor.ImportDtos;
using Medicines.Utilities;

using Newtonsoft.Json;

namespace Medicines.DataProcessor
{
    public class Deserializer
    {
        private const string ErrorMessage = "Invalid Data!";

        private const string SuccessfullyImportedPharmacy =
            "Successfully imported pharmacy - {0} with {1} medicines.";

        private const string SuccessfullyImportedPatient =
            "Successfully imported patient - {0} with {1} medicines.";

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResults, true);
        }

        public static string ImportPharmacies(MedicinesContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var xmlHelper = new XmlHelper();

            var pharmacyDtos =
                xmlHelper.Deserialize<ImportPharmacyDto[]>(xmlString, "Pharmacies");

            var pharmaciesToAdd = new List<Pharmacy>();

            foreach (var dto in pharmacyDtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (!bool.TryParse(dto.IsNonStop, out bool isNonStop))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var pharmacy = new Pharmacy
                {
                    Name = dto.Name,
                    PhoneNumber = dto.PhoneNumber,
                    IsNonStop = isNonStop
                };

                var medicines = new List<Medicine>();

                foreach (var medDto in dto.Medicines)
                {
                    if (!IsValid(medDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (!decimal.TryParse(
                            medDto.Price,
                            NumberStyles.Number,
                            CultureInfo.InvariantCulture,
                            out decimal price))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (price < 0.01m || price > 1000m)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (!int.TryParse(medDto.Category, out int categoryInt) ||
                        !Enum.IsDefined(typeof(Category), categoryInt))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var category = (Category)categoryInt;

                    bool isProdValid = DateTime.TryParseExact(
                        medDto.ProductionDate,
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out DateTime productionDate);

                    bool isExpiryValid = DateTime.TryParseExact(
                        medDto.ExpiryDate,
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out DateTime expiryDate);

                    if (!isProdValid || !isExpiryValid || productionDate >= expiryDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (medicines.Any(m =>
                            m.Name == medDto.Name &&
                            m.Producer == medDto.Producer))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var medicine = new Medicine
                    {
                        Name = medDto.Name,
                        Price = price,
                        Category = category,
                        ProductionDate = productionDate,
                        ExpiryDate = expiryDate,
                        Producer = medDto.Producer,
                        Pharmacy = pharmacy
                    };

                    medicines.Add(medicine);
                }

                pharmacy.Medicines = medicines;
                pharmaciesToAdd.Add(pharmacy);

                sb.AppendLine(string.Format(
                    SuccessfullyImportedPharmacy,
                    pharmacy.Name,
                    pharmacy.Medicines.Count));
            }

            context.Pharmacies.AddRange(pharmaciesToAdd);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportPatients(MedicinesContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var patientDtos =
                JsonConvert.DeserializeObject<ImportPatientDto[]>(jsonString)!;

            var patientsToAdd = new List<Patient>();

            var validMedicineIds = context.Medicines
                .Select(m => m.Id)
                .ToHashSet();

            foreach (var dto in patientDtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (!int.TryParse(dto.AgeGroup, out int ageGroupInt) ||
                    !Enum.IsDefined(typeof(AgeGroup), ageGroupInt))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (!int.TryParse(dto.Gender, out int genderInt) ||
                    !Enum.IsDefined(typeof(Gender), genderInt))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var patient = new Patient
                {
                    FullName = dto.FullName,
                    AgeGroup = (AgeGroup)ageGroupInt,
                    Gender = (Gender)genderInt
                };

                var addedMedicineIds = new HashSet<int>();

                foreach (var medicineId in dto.Medicines)
                {
                    if (addedMedicineIds.Contains(medicineId))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (!validMedicineIds.Contains(medicineId))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    addedMedicineIds.Add(medicineId);

                    var patientMedicine = new PatientMedicine
                    {
                        Patient = patient,
                        MedicineId = medicineId
                    };

                    patient.PatientsMedicines.Add(patientMedicine);
                }

                patientsToAdd.Add(patient);

                sb.AppendLine(string.Format(
                    SuccessfullyImportedPatient,
                    patient.FullName,
                    patient.PatientsMedicines.Count));
            }

            context.Patients.AddRange(patientsToAdd);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }
    }
}
