using APBDprojekt.Contexts;
using APBDprojekt.Exceptions;
using APBDprojekt.Models;
using APBDprojekt.RequestModels;
using APBDprojekt.Servces;
using Microsoft.EntityFrameworkCore;

namespace APBDprojekt.Tests;

public class ContractServiceTests
    {
        private readonly DbContextOptions<DatabaseContext> _dbContextOptions;

        public ContractServiceTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private DatabaseContext CreateContext() => new DatabaseContext(_dbContextOptions);

        [Fact]
        public async Task CalculateDiscount_ShouldReturnBestDiscount()
        {
            // Arrange
            var softwareId = 1;
            var type = "contract";
            var currentDate = DateTime.Now;

            var discounts = new List<Discount>
            {
                new Discount { DiscountId = 1, SoftwareId = softwareId, Name = "A", PercentValue = 10, DateFrom = currentDate.AddDays(-1), DateTo = currentDate.AddDays(1), Type = type },
                new Discount { DiscountId = 2, SoftwareId = softwareId, Name = "B", PercentValue = 15, DateFrom = currentDate.AddDays(-1), DateTo = currentDate.AddDays(1), Type = type },
                new Discount { DiscountId = 3, SoftwareId = softwareId, Name = "C", PercentValue = 30, DateFrom = currentDate.AddDays(-1), DateTo = currentDate.AddDays(1), Type = "subscription" },
            };

            await using (var context = CreateContext())
            {
                context.Discounts.AddRange(discounts);
                await context.SaveChangesAsync();
            }

            var contractService = new ContractService(CreateContext());

            // Act
            var bestDiscount = await contractService.CalculateDiscount(softwareId, type);

            // Assert
            Assert.Equal(15, bestDiscount);
        }


        [Fact]
        public async Task PostContract_ShouldCreateContract()
        {
            // Arrange
            var clientId = 1;
            var client = new ClientPerson
            {
                ClientId = 1,
                Name = "string",
                Surname = "string",
                Email = "string@gmail.com",
                Address = "string",
                TelephoneNumber = "123456789",
                PeselNumber = "12345678910",
                Has5PercentDiscount = true
            };
            
            var softwareId = 1;
            var software = new Software { 
                SoftwareId = softwareId, 
                Name = "string", 
                CanBuy = true, 
                PurchasePrice = 1000, 
                CurrentVersion = "1.0",
                Category = "string",
                Description = "string"
            };
            
            await using (var context = CreateContext())
            {
                context.ClientPersons.Add(client);
                context.Softwares.Add(software);
                await context.SaveChangesAsync();
            }
            
            var contractService = new ContractService(CreateContext());
            
            var requestModel = new PostContractModelRequestModel
            {
                PaymentStartTime = DateTime.Now,
                PaymentEndTime = DateTime.Now.AddYears(1),
                YearsOfVersionSupport = 1
            };

            // Act
            var response = await contractService.PostContract(requestModel, clientId, softwareId);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(clientId, response.ClientId);
            Assert.Equal(softwareId, response.SoftwareId);
            Assert.Equal(950, response.PriceWithDiscounts); 
            Assert.Equal(requestModel.PaymentStartTime, response.PaymentStartTime);
            Assert.Equal(requestModel.PaymentEndTime, response.PaymentEndTime);
            Assert.Equal("1.0", response.CurrentSoftwareVersion);
            Assert.Equal(requestModel.PaymentStartTime.AddYears(requestModel.YearsOfVersionSupport), response.EndOfSupport);
        } 
        
        [Fact]
        public async Task PostContract_ShouldThrowNotFoundException_WhenClientNotFound()
        {
            // Arrange
            var softwareId = 1;
            

            var software = new Software { 
                SoftwareId = softwareId, 
                Name = "string", 
                CanBuy = true, 
                PurchasePrice = 1000, 
                CurrentVersion = "1.0",
                Category = "string",
                Description = "string" };
            
            await using (var context = CreateContext())
            {
                context.Softwares.Add(software);
                await context.SaveChangesAsync();
            }

            
            var contractService = new ContractService(CreateContext());

            
            var requestModel = new PostContractModelRequestModel
            {
                PaymentStartTime = DateTime.Now,
                PaymentEndTime = DateTime.Now.AddYears(1),
                YearsOfVersionSupport = 2
            };

            // Act & Assert
            
            await Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await contractService.PostContract(requestModel, 999, softwareId);
            });
        }
        
        [Fact]
        public async Task PostContract_ShouldThrowNotFoundException_WhenSoftwareNotFound()
        {
            // Arrange
            var clientId = 1;
            var softwareNotExistId = 999;

            var client = new ClientPerson {  ClientId = 1,
                Name = "string",
                Surname = "string",
                Email = "string@gmail.com",
                Address = "string",
                TelephoneNumber = "123456789",
                PeselNumber = "12345678910",
                Has5PercentDiscount = true };

            
            await using (var context = CreateContext())
            {
                context.Clients.Add(client);
                await context.SaveChangesAsync();
            }
            
            var contractService = new ContractService(CreateContext());
            
            var requestModel = new PostContractModelRequestModel
            {
                PaymentStartTime = DateTime.Now,
                PaymentEndTime = DateTime.Now.AddYears(1),
                YearsOfVersionSupport = 2
            };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await contractService.PostContract(requestModel, clientId, softwareNotExistId);
            });
        }
        
        [Fact]
        public async Task PostContract_ShouldThrowBadSoftwareBuyTypeException_WhenSoftwareCannotBeBought()
        {
            // Arrange
            var clientId = 1;
            var softwareId = 1;

            var client = new ClientPerson {  
                ClientId = 1,
                Name = "string",
                Surname = "string",
                Email = "string@gmail.com",
                Address = "string",
                TelephoneNumber = "123456789",
                PeselNumber = "12345678910",
                Has5PercentDiscount = true };
            
            var software = new Software { 
                SoftwareId = softwareId, 
                Name = "string", 
                CanBuy = false, 
                PurchasePrice = 1000, 
                CurrentVersion = "1.0",
                Category = "string",
                Description = "string" };
            
            await using (var context = CreateContext())
            {
                context.Clients.Add(client);
                context.Softwares.Add(software);
                await context.SaveChangesAsync();
            }
            
            var contractService = new ContractService(CreateContext());
            
            var requestModel = new PostContractModelRequestModel
            {
                PaymentStartTime = DateTime.Now,
                PaymentEndTime = DateTime.Now.AddYears(1),
                YearsOfVersionSupport = 2
            };

            // Act & Assert
            await Assert.ThrowsAsync<BadSoftwareBuyTypeException>(async () =>
            {
                await contractService.PostContract(requestModel, clientId, softwareId);
            });
        }
        

        
    }