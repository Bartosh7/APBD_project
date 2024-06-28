using APBDprojekt.Contexts;
using APBDprojekt.Exceptions;
using APBDprojekt.Models;
using APBDprojekt.RequestModels;
using APBDprojekt.Servces;
using Microsoft.EntityFrameworkCore;

namespace APBDprojekt.Tests;

public class ContractPaymentsServiceTests
{
    private readonly DbContextOptions<DatabaseContext> _dbContextOptions;

    public ContractPaymentsServiceTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }
    
    private DatabaseContext CreateContext() => new DatabaseContext(_dbContextOptions);
    
    
    [Fact]
        public async Task PostContractPayment_ShouldProcessPaymentSuccessfully()
        {
            // Arrange
            var clientId = 1;
            var contractId = 1;

            var client = new ClientPerson {  
                ClientId = 1,
                Name = "string",
                Surname = "string",
                Email = "string@gmail.com",
                Address = "string",
                TelephoneNumber = "123456789",
                PeselNumber = "12345678910",
                Has5PercentDiscount = true };

            var contract = new Contract
            {
                ContracId = contractId,
                ClientId = clientId,
                SoftwareId = 1,
                PriceWithDiscounts = 1000,
                AlreadyPaid = 0,
                Signed = false,
                CurrentSoftwareVersion = "1.0"
            };

            await using (var context = CreateContext())
            {
                context.ClientPersons.Add(client);
                context.Contracts.Add(contract);
                await context.SaveChangesAsync();
            }

            var contractPaymentService = new ContractPaymentService(CreateContext(), null);

            var paymentRequest = new PostContractPaymentRequestModel
            {
                AmountOfPayment = 1000
            };

            // Act
            var response = await contractPaymentService.PostContractPayment(paymentRequest, clientId, contractId);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(clientId, response.ClientId);
            Assert.Equal(contract.SoftwareId, response.SoftwareId);
            Assert.Equal(contractId, response.ContractPaymentId);
            Assert.Equal(1000, response.LastPayment);
            Assert.Equal(1000, response.AlreadyPaid);

            using (var context = CreateContext())
            {
                var updatedContract = await context.Contracts.FirstOrDefaultAsync(c => c.ContracId == contractId);
                Assert.NotNull(updatedContract);
                Assert.Equal(1000, updatedContract.AlreadyPaid);
                Assert.True(updatedContract.Signed); 
            }
        }
        
    [Fact]
    public async Task PostContractPayment_ShouldThrowNotFoundException_WhenClientNotFound()
    {
        // Arrange
        var contractPaymentService = new ContractPaymentService(CreateContext(), null);
        var clientId = 1;
        var contractId = 1;
        
        
        
        var contract = new Contract
        {
            ContracId = contractId,
            ClientId = clientId,
            SoftwareId = 1,
            PriceWithDiscounts = 1000,
            AlreadyPaid = 0,
            Signed = false,
            CurrentSoftwareVersion = "1.0"
        };
        
        await using (var context = CreateContext())
        {
            context.Contracts.Add(contract);
            await context.SaveChangesAsync();
        }

        // Act & Assert
        
        await Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await contractPaymentService.PostContractPayment(new PostContractPaymentRequestModel(), clientId, contractId);
        });
    }


    [Fact]
    public async Task PostContractPayment_ShouldThrowNotFoundException_WhenContractNotFound()
    {
        // Arrange
        var contractPaymentService = new ContractPaymentService(CreateContext(), null);
        var clientId = 1;
        var contractId = 1;
        
        var client = new ClientPerson {  
            ClientId = 1,
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

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await contractPaymentService.PostContractPayment(new PostContractPaymentRequestModel(), clientId,
                contractId);
        });
    }
    
    [Fact]
    public async Task PostContractPayment_ShouldThrowNotFoundException_WhenContractNotBelongsToClient()
    {
        // Arrange
        var contractPaymentService = new ContractPaymentService(CreateContext(), null);
        var clientId = 1;
        var contractId = 1;
        
        
        var client = new ClientPerson {  
            ClientId = 1,
            Name = "string",
            Surname = "string",
            Email = "string@gmail.com",
            Address = "string",
            TelephoneNumber = "123456789",
            PeselNumber = "12345678910",
            Has5PercentDiscount = true };

        var contract = new Contract
        {
            ContracId = contractId,
            ClientId = 2,
            SoftwareId = 1,
            PriceWithDiscounts = 1000,
            AlreadyPaid = 0,
            Signed = false,
            CurrentSoftwareVersion = "1.0"
        };
        
        await using (var context = CreateContext())
        {
            context.Clients.Add(client);
            context.Contracts.Add(contract);
            await context.SaveChangesAsync();
        }

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await contractPaymentService.PostContractPayment(new PostContractPaymentRequestModel(), clientId, contractId);
        });
    }
    
    [Fact]
    public async Task PostContractPayment_ShouldThrowAlreadySignedException_WhenContractAlreadySigned()
    {
        // Arrange
        var contractPaymentService = new ContractPaymentService(CreateContext(), null);
        var clientId = 1;
        var contractId = 1;

        var client = new ClientPerson {  
            ClientId = 1,
            Name = "string",
            Surname = "string",
            Email = "string@gmail.com",
            Address = "string",
            TelephoneNumber = "123456789",
            PeselNumber = "12345678910",
            Has5PercentDiscount = true };

        var contract = new Contract
        {
            ContracId = contractId,
            ClientId = 1,
            SoftwareId = 1,
            PriceWithDiscounts = 1000,
            AlreadyPaid = 1000,
            Signed = true,
            CurrentSoftwareVersion = "1.0"
        };
        
        await using (var context = CreateContext())
        {
            context.Clients.Add(client);
            context.Contracts.Add(contract);
            await context.SaveChangesAsync();
        }

        // Act & Assert
        await Assert.ThrowsAsync<AlreadySignedException>(async () =>
        {
            await contractPaymentService.PostContractPayment(new PostContractPaymentRequestModel(), clientId, contractId);
        });
    }
    
    
    [Fact]
    public async Task PostContractPayment_ShouldThrowTooBigPaymentException_WhenPaymentExceedsPriceWithDiscounts()
    {
        // Arrange
        var contractPaymentService = new ContractPaymentService(CreateContext(), null);
        var clientId = 1;
        var contractId = 1;
        
        var client = new ClientPerson {  
            ClientId = 1,
            Name = "string",
            Surname = "string",
            Email = "string@gmail.com",
            Address = "string",
            TelephoneNumber = "123456789",
            PeselNumber = "12345678910",
            Has5PercentDiscount = true };

        var contract = new Contract
        {
            ContracId = contractId,
            ClientId = 1,
            SoftwareId = 1,
            PriceWithDiscounts = 1000,
            AlreadyPaid = 500,
            Signed = false,
            CurrentSoftwareVersion = "1.0"
        };
        
        
        await using (var context = CreateContext())
        {
            context.Clients.Add(client);
            context.Contracts.Add(contract);
            await context.SaveChangesAsync();
        }

        
        var paymentRequest = new PostContractPaymentRequestModel
        {
            AmountOfPayment = 600
        };

        // Act & Assert
        await Assert.ThrowsAsync<TooBigPaymentException>(async () =>
        {
            await contractPaymentService.PostContractPayment(paymentRequest, clientId, contractId);
        });
    }
    
    
    
    //Nie wiem jak napisac testy, z api do przeliczania walut :< 
    
}