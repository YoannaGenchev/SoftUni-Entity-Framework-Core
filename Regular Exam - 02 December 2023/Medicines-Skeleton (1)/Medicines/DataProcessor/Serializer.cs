namespace Medicines.DataProcessor
{
    using System;
    using System.Globalization;
    using System.Linq;

    using Medicines.Data;
    using Medicines.Data.Models.Enums;
    using Medicines.DataProcessor.ExportDtos;
    using Medicines.Utilities;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportPatientsWithTheirMedicines(MedicinesContext context, string date)
        {
            var givenDate = DateTime.Parse(date, CultureInfo.InvariantCulture);

            var patients = context.Patients
                .Where(p => p.PatientsMedicines
                    .Any(pm => pm.Medicine.ProductionDate >= givenDate))
                .Select(p => new ExportPatientDto
                {
                    FullName = p.FullName,
                    AgeGroup = p.AgeGroup.ToString(),
                    Gender = p.Gender.ToString().ToLower(),
                    Medicines = p.PatientsMedicines
                        .Where(pm => pm.Medicine.ProductionDate >= givenDate)
                        .Select(pm => pm.Medicine)
                        .OrderByDescending(m => m.ExpiryDate)
                        .ThenBy(m => m.Price) 
                        .Select(m => new ExportMedicineDto
                        {
                            Name = m.Name,
                            Price = m.Price.ToString("F2", CultureInfo.InvariantCulture),
                            Category = m.Category.ToString().ToLower(),
                            Producer = m.Producer,
                            ExpiryDate = m.ExpiryDate
                                .ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                        })
                        .ToArray()
                })
                .OrderByDescending(p => p.Medicines.Length)
                .ThenBy(p => p.FullName) 
                .ToArray();

            var xmlHelper = new XmlHelper();
            string xmlResult = xmlHelper.Serialize(patients, "Patients");

            return xmlResult;
        }

        public static string ExportMedicinesFromDesiredCategoryInNonStopPharmacies(
            MedicinesContext context,
            int medicineCategory)
        {
            var category = (Category)medicineCategory;

            var medicines = context.Medicines
                .Where(m => m.Category == category && m.Pharmacy.IsNonStop)
                .OrderBy(m => m.Price) 
                .ThenBy(m => m.Name) 
                .Select(m => new
                {
                    Name = m.Name,
                    Price = m.Price.ToString("F2", CultureInfo.InvariantCulture),
                    Pharmacy = new
                    {
                        Name = m.Pharmacy.Name,
                        PhoneNumber = m.Pharmacy.PhoneNumber
                    }
                })
                .ToArray();

            string jsonResult = JsonConvert.SerializeObject(medicines, Formatting.Indented);

            return jsonResult;
        }
    }
}
