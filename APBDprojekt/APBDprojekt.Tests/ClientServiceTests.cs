using APBDprojekt.Contexts;
using APBDprojekt.Exceptions;
using APBDprojekt.Models;
using APBDprojekt.RequestModels;
using APBDprojekt.Servces;
using Microsoft.EntityFrameworkCore;

namespace APBDprojekt.Tests;

public class ClientServiceTests
{
    private readonly DbContextOptions<DatabaseContext> _dbContextOptions;

    public ClientServiceTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    private DatabaseContext CreateContext() => new DatabaseContext(_dbContextOptions);

    [Fact]
    public async Task AddIndividualClientAsync_ShouldAddClient()
    {
        // Arrange
        await using var context = CreateContext();
        var clientService = new ClientService(context);

        var model = new PostClientPersonRequestModel
        {
            Name = "string",
            Surname = "string",
            Email = "string@gmail.com",
            Address = "string",
            TelephoneNumber = "123456789",
            PeselNumber = "12345678910"
        };

        // Act
        await clientService.PostClientPerson(model);

        // Assert
        var client = await context.ClientPersons.FirstOrDefaultAsync();
        Assert.NotNull(client);
        Assert.Equal(model.Email, client.Email);
        Assert.Equal(model.Address, client.Address);
        Assert.Equal(model.TelephoneNumber, client.TelephoneNumber);
        Assert.Equal(model.Name, client.Name);
        Assert.Equal(model.Surname, client.Surname);
        Assert.Equal(model.PeselNumber, client.PeselNumber);
    }
    
    
    [Fact]
    public async Task AddCompanyClientAsync_ShouldAddClient()
    {
        // Arrange
        await using var context = CreateContext();
        var clientService = new ClientService(context);

        var model = new PostClientCompanyRequestModel
        {
            Name = "string",
            Email = "stringy@gmail.com",
            Address = "string",
            TelephoneNumber = "987654321",
            KRS = "123456789"
        };

        // Act
        await clientService.PostClientCompany(model);

        // Assert
        var client = await context.ClientCompanies.FirstOrDefaultAsync();
        Assert.NotNull(client);
        Assert.Equal(model.Name, client.Name);
        Assert.Equal(model.Email, client.Email);
        Assert.Equal(model.Address, client.Address);
        Assert.Equal(model.TelephoneNumber, client.TelephoneNumber);
        Assert.Equal(model.KRS, client.KRS);
    }
    
    [Fact]
    public async Task DeleteClientPerson_ShouldMarkClientAsDeleted()
    {
        // Arrange
        
        await using var context = CreateContext();
        var initialClient = new ClientPerson
        {
            Name = "string",
            Surname = "string",
            Email = "string@gmail.com",
            Address = "string",
            TelephoneNumber = "123456789",
            PeselNumber = "12345678910"
        };
        context.ClientPersons.Add(initialClient);
        await context.SaveChangesAsync();

        
        var clientService = new ClientService(context);

        // Act
        await clientService.DeleteClientPerson(initialClient.ClientId);

        // Assert
        
        var deletedClient = await context.ClientPersons.FirstOrDefaultAsync(c => c.ClientId == initialClient.ClientId);
        Assert.NotNull(deletedClient);
        Assert.True(deletedClient.IsDeleted);
    }
    
    [Fact]
    public async Task DeleteClientPerson_ThrowsNotFoundException_WhenClientNotFound()
    {
        // Arrange
        await using var context = CreateContext();
        var clientService = new ClientService(context);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await clientService.DeleteClientPerson(999); 
        });
        
        
        
    }
    
    [Fact]
        public async Task PutClientPerson_ShouldUpdateClient()
        {
            // Arrange
            await using var context = CreateContext();
            var initialClient = new ClientPerson
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
            context.ClientPersons.Add(initialClient);
            await context.SaveChangesAsync();
            
            var clientService = new ClientService(context);

            // Act
            var updatedModel = new PutClientPersonRequestModel
            {
                Name = "string2",
                Surname = "string2",
                Email = "string@gmail.com2",
                Address = "string2",
                TelephoneNumber = "222222222"
            };
            await clientService.PutClientPerson(updatedModel, initialClient.ClientId);

            // Assert
            var updatedClient = await context.ClientPersons.FirstOrDefaultAsync(c => c.ClientId == initialClient.ClientId);
            Assert.NotNull(updatedClient);
            Assert.Equal(updatedModel.Name, updatedClient.Name);
            Assert.Equal(updatedModel.Surname, updatedClient.Surname);
            Assert.Equal(updatedModel.Email, updatedClient.Email);
            Assert.Equal(updatedModel.Address, updatedClient.Address);
            Assert.Equal(updatedModel.TelephoneNumber, updatedClient.TelephoneNumber);
        }

        [Fact]
        public async Task PutClientPerson_ThrowsNotFoundException_WhenClientNotFound()
        {
            // Arrange
            await using var context = CreateContext();
            var clientService = new ClientService(context);

            // Act & Assert
            var updatedModel = new PutClientPersonRequestModel
            {
                Name = "string2",
                Surname = "string2",
                Email = "string@gmail.com2",
                Address = "string2",
                TelephoneNumber = "222222222"
            };
            await Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await clientService.PutClientPerson(updatedModel, 999); 
            });
        }
        
        
        [Fact]
        public async Task PutClientCompany_ShouldUpdateClient()
        {
            // Arrange
            
            await using var context = CreateContext();
            var initialClient = new ClientCompany
            {
                ClientId = 1,
                Name = "string",
                Email = "stringy@gmail.com",
                Address = "string",
                TelephoneNumber = "987654321",
                KRS = "123456789"
            };
            context.ClientCompanies.Add(initialClient);
            await context.SaveChangesAsync();

            var clientService = new ClientService(context);

            // Act
            var updatedModel = new PutClientCompanyRequestModel
            {
                Name = "string2",
                Email = "stringy@gmail.com2",
                Address = "string2",
                TelephoneNumber = "222222222"
            };
            await clientService.PutClientCompany(updatedModel, initialClient.ClientId);

            // Assert
            var updatedClient = await context.ClientCompanies.FirstOrDefaultAsync(c => c.ClientId == initialClient.ClientId);
            Assert.NotNull(updatedClient);
            Assert.Equal(updatedModel.Name, updatedClient.Name);
            Assert.Equal(updatedModel.Email, updatedClient.Email);
            Assert.Equal(updatedModel.Address, updatedClient.Address);
            Assert.Equal(updatedModel.TelephoneNumber, updatedClient.TelephoneNumber);
        }
        
        
        [Fact]
        public async Task PutClientCompany_ThrowsNotFoundException_WhenClientNotFound()
        {
            // Arrange
            await using var context = CreateContext();
            var clientService = new ClientService(context);

            // Act & Assert
            var updatedModel = new PutClientCompanyRequestModel
            {
                Name = "string2",
                Email = "stringy@gmail.com2",
                Address = "string2",
                TelephoneNumber = "222222222"
            };
            await Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await clientService.PutClientCompany(updatedModel, 999); 
            });
        }
}