namespace Invoices.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    using Invoices.Data;
    using Invoices.Data.Models;
    using Invoices.Data.Models.Enums;
    using Invoices.DataProcessor.ImportDto;

    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedClients
            = "Successfully imported client {0}.";

        private const string SuccessfullyImportedInvoices
            = "Successfully imported invoice with number {0}.";

        private const string SuccessfullyImportedProducts
            = "Successfully imported product - {0} with {1} clients.";
        public static string ImportClients(InvoicesContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var xmlRoot = new XmlRootAttribute("Clients");
            var serializer = new XmlSerializer(typeof(ImportClientDto[]), xmlRoot);

            ImportClientDto[] clientDtos;
            using (var reader = new StringReader(xmlString))
            {
                clientDtos = (ImportClientDto[])serializer.Deserialize(reader)!;
            }

            var clientsToAdd = new List<Client>();

            foreach (var clientDto in clientDtos)
            {
                if (!IsValid(clientDto) ||
                    string.IsNullOrWhiteSpace(clientDto.Name) ||
                    string.IsNullOrWhiteSpace(clientDto.NumberVat))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var client = new Client
                {
                    Name = clientDto.Name,
                    NumberVat = clientDto.NumberVat
                };

                var addresses = new List<Address>();

                if (clientDto.Addresses != null)
                {
                    foreach (var addressDto in clientDto.Addresses)
                    {
                        if (!IsValid(addressDto) ||
                            string.IsNullOrWhiteSpace(addressDto.StreetName) ||
                            string.IsNullOrWhiteSpace(addressDto.PostCode) ||
                            string.IsNullOrWhiteSpace(addressDto.City) ||
                            string.IsNullOrWhiteSpace(addressDto.Country) ||
                            addressDto.StreetNumber <= 0)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                        var address = new Address
                        {
                            StreetName = addressDto.StreetName,
                            StreetNumber = addressDto.StreetNumber,
                            PostCode = addressDto.PostCode,
                            City = addressDto.City,
                            Country = addressDto.Country,
                            Client = client
                        };

                        addresses.Add(address);
                    }
                }

                client.Addresses = addresses;

                clientsToAdd.Add(client);
                sb.AppendLine(string.Format(SuccessfullyImportedClients, client.Name));
            }

            context.Clients.AddRange(clientsToAdd);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportInvoices(InvoicesContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var invoiceDtos = JsonConvert
                .DeserializeObject<ImportInvoiceDto[]>(jsonString)!;

            var validClientIds = context.Clients
                .Select(c => c.Id)
                .ToHashSet();

            var invoicesToAdd = new List<Invoice>();

            foreach (var dto in invoiceDtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (!Enum.IsDefined(typeof(CurrencyType), dto.CurrencyType))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (!validClientIds.Contains(dto.ClientId))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (!DateTime.TryParse(dto.IssueDate, CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out var issueDate) ||
                    !DateTime.TryParse(dto.DueDate, CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out var dueDate))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (dueDate < issueDate)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var invoice = new Invoice
                {
                    Number = dto.Number,
                    IssueDate = issueDate,
                    DueDate = dueDate,
                    Amount = dto.Amount,
                    CurrencyType = (CurrencyType)dto.CurrencyType,
                    ClientId = dto.ClientId
                };

                invoicesToAdd.Add(invoice);
                sb.AppendLine(string.Format(SuccessfullyImportedInvoices, invoice.Number));
            }

            context.Invoices.AddRange(invoicesToAdd);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportProducts(InvoicesContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var productDtos = JsonConvert
                .DeserializeObject<ImportProductDto[]>(jsonString)!;

            var validClientIds = context.Clients
                .Select(c => c.Id)
                .ToHashSet();

            var productsToAdd = new List<Product>();

            foreach (var dto in productDtos)
            {
                if (!IsValid(dto) ||
                    !Enum.IsDefined(typeof(CategoryType), dto.CategoryType))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var product = new Product
                {
                    Name = dto.Name,
                    Price = dto.Price,
                    CategoryType = (CategoryType)dto.CategoryType
                };

                var uniqueClientIds = (dto.Clients ?? Array.Empty<int>())
                    .Distinct();

                foreach (var clientId in uniqueClientIds)
                {
                    if (!validClientIds.Contains(clientId))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var productClient = new ProductClient
                    {
                        Product = product,
                        ClientId = clientId
                    };

                    product.ProductsClients.Add(productClient);
                }

                productsToAdd.Add(product);

                sb.AppendLine(string.Format(
                    SuccessfullyImportedProducts,
                    product.Name,
                    product.ProductsClients.Count));
            }

            context.Products.AddRange(productsToAdd);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
