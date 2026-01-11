using CarDealer.Data;
using CarDealer.DTOs.Import;
using CarDealer.DTOs.Export;
using CarDealer.Models;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ImportSupplierDto[]), new XmlRootAttribute("Suppliers"));
            ImportSupplierDto[] supplierDtos;
            using (StringReader reader = new StringReader(inputXml))
            {
                supplierDtos = (ImportSupplierDto[])serializer.Deserialize(reader)!;
            }
            var suppliers = supplierDtos.Select(dto => new Supplier { Name = dto.Name, IsImporter = dto.IsImporter }).ToArray();
            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();
            return $"Successfully imported {suppliers.Length}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ImportPartDto[]), new XmlRootAttribute("Parts"));
            ImportPartDto[] partDtos;
            using (StringReader reader = new StringReader(inputXml))
            {
                partDtos = (ImportPartDto[])serializer.Deserialize(reader)!;
            }
            var supplierIds = context.Suppliers.Select(s => s.Id).ToHashSet();
            var parts = partDtos.Where(dto => supplierIds.Contains(dto.SupplierId)).Select(dto => new Part { Name = dto.Name, Price = dto.Price, Quantity = dto.Quantity, SupplierId = dto.SupplierId }).ToArray();
            context.Parts.AddRange(parts);
            context.SaveChanges();
            return $"Successfully imported {parts.Length}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ImportCarDto[]), new XmlRootAttribute("Cars"));
            ImportCarDto[] carDtos;
            using (StringReader reader = new StringReader(inputXml))
            {
                carDtos = (ImportCarDto[])serializer.Deserialize(reader)!;
            }
            var partIds = context.Parts.Select(p => p.Id).ToHashSet();
            var cars = new List<Car>();
            foreach (var carDto in carDtos)
            {
                var car = new Car { Make = carDto.Make, Model = carDto.Model, TraveledDistance = carDto.TraveledDistance };
                var uniquePartIds = carDto.Parts.Select(p => p.Id).Distinct().Where(id => partIds.Contains(id)).ToArray();
                foreach (var partId in uniquePartIds)
                {
                    car.PartsCars.Add(new PartCar { PartId = partId });
                }
                cars.Add(car);
            }
            context.Cars.AddRange(cars);
            context.SaveChanges();
            return $"Successfully imported {cars.Count}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ImportCustomerDto[]), new XmlRootAttribute("Customers"));
            ImportCustomerDto[] customerDtos;
            using (StringReader reader = new StringReader(inputXml))
            {
                customerDtos = (ImportCustomerDto[])serializer.Deserialize(reader)!;
            }
            var customers = customerDtos.Select(dto => new Customer { Name = dto.Name, BirthDate = dto.BirthDate, IsYoungDriver = dto.IsYoungDriver }).ToArray();
            context.Customers.AddRange(customers);
            context.SaveChanges();
            return $"Successfully imported {customers.Length}";
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ImportSaleDto[]), new XmlRootAttribute("Sales"));
            ImportSaleDto[] saleDtos;
            using (StringReader reader = new StringReader(inputXml))
            {
                saleDtos = (ImportSaleDto[])serializer.Deserialize(reader)!;
            }
            var carIds = context.Cars.Select(c => c.Id).ToHashSet();
            var sales = saleDtos.Where(dto => carIds.Contains(dto.CarId)).Select(dto => new Sale { CarId = dto.CarId, CustomerId = dto.CustomerId, Discount = dto.Discount }).ToArray();
            context.Sales.AddRange(sales);
            context.SaveChanges();
            return $"Successfully imported {sales.Length}";
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var cars = context.Cars.Where(c => c.TraveledDistance > 2000000).OrderBy(c => c.Make).ThenBy(c => c.Model).Take(10).Select(c => new ExportCarWithDistanceDto { Make = c.Make, Model = c.Model, TraveledDistance = c.TraveledDistance }).ToArray();
            return SerializeToXml(cars, "cars");
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var cars = context.Cars.Where(c => c.Make == "BMW").OrderBy(c => c.Model).
                ThenByDescending(c => c.TraveledDistance).
                Select(c => new ExportBmwCarDto { Id = c.Id, Model = c.Model, TraveledDistance = c.TraveledDistance }).ToArray();
            return SerializeToXml(cars, "cars");
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers.Where(s => !s.IsImporter).Select(s => new ExportLocalSupplierDto { Id = s.Id, Name = s.Name, PartsCount = s.Parts.Count }).ToArray();
            return SerializeToXml(suppliers, "suppliers");
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars.OrderByDescending(c => c.TraveledDistance).ThenBy(c => c.Model).Take(5).Select(c => new ExportCarWithPartsDto { Make = c.Make, Model = c.Model, TraveledDistance = c.TraveledDistance, Parts = c.PartsCars.Select(pc => new ExportPartDto { Name = pc.Part.Name, Price = pc.Part.Price }).OrderByDescending(p => p.Price).ToArray() }).ToArray();
            return SerializeToXml(cars, "cars");
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales.Any())
                .Select(c => new ExportCustomerSalesDto
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count,
                    SpentMoney = c.Sales.Sum(s => s.Car.PartsCars.Sum(pc =>
                        c.IsYoungDriver
                            ? Math.Round(pc.Part.Price * 0.95m * 100) / 100
                            : pc.Part.Price))
                })
                .OrderByDescending(c => c.SpentMoney)
                .ToArray();

            return SerializeToXml(customers, "customers");
        }


        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var salesData = context.Sales
                .Select(s => new
                {
                    Make = s.Car.Make,
                    Model = s.Car.Model,
                    TraveledDistance = s.Car.TraveledDistance,
                    CustomerName = s.Customer.Name,
                    Discount = s.Discount,
                    Price = s.Car.PartsCars.Sum(pc => pc.Part.Price)
                })
                .ToList();

            var sales = salesData
                .Select(s => new ExportSaleDto
                {
                    Car = new ExportSaleCarDto
                    {
                        Make = s.Make,
                        Model = s.Model,
                        TraveledDistance = s.TraveledDistance
                    },
                    Discount = (int)s.Discount,
                    CustomerName = s.CustomerName,
                    Price = s.Price,
                    PriceWithDiscount = s.Price - (s.Price * s.Discount / 100)
                })
                .ToArray();

            return SerializeToXml(sales, "sales");
        }

        private static string SerializeToXml<T>(T[] objects, string rootName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T[]), new XmlRootAttribute(rootName));
            StringBuilder sb = new StringBuilder();
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            using (StringWriter writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, objects, namespaces);
            }
            return sb.ToString().TrimEnd();
        }
    }
}