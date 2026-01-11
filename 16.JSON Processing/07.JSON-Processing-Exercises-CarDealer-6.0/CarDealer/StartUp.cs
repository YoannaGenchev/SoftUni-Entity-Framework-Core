using System.Text.Json;
using CarDealer.Data;
using CarDealer.DTOs.Import;
using CarDealer.DTOs.Export;
using CarDealer.Models;
using Microsoft.EntityFrameworkCore;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            using var context = new CarDealerContext();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Take(10)
                .Select(s => new ExportSaleDiscountDto
                {
                    Car = new CarInfoDto
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TraveledDistance = s.Car.TraveledDistance
                    },
                    CustomerName = s.Customer.Name,
                    Discount = s.Discount.ToString("F2"),
                    Price = s.Car.PartsCars.Sum(pc => pc.Part.Price).ToString("F2"),
                    PriceWithDiscount = (s.Car.PartsCars.Sum(pc => pc.Part.Price) *
                        (1 - s.Discount / 100)).ToString("F2")
                })
                .AsNoTracking()
                .ToList();

            return JsonSerializer.Serialize(sales, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }


    }
}