using APBDprojekt.Contexts;
using APBDprojekt.Exceptions;
using APBDprojekt.Models;
using APBDprojekt.RequestModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace APBDprojekt.Servces;

public interface IClientService
{
    Task PostClientPerson(PostClientPersonRequestModel data);
    Task PostClientCompany(PostClientCompanyRequestModel data);
    Task DeleteClientPerson(int id);
    Task PutClientPerson(PutClientPersonRequestModel data, int id);
    Task PutClientCompany(PutClientCompanyRequestModel data, int id);
}

public class ClientService(DatabaseContext context) : IClientService
{
    public async Task PostClientPerson(PostClientPersonRequestModel data)
    {
        context.ClientPersons.Add(new ClientPerson
        {
            Name = data.Name,
            Surname = data.Surname,
            PeselNumber = data.PeselNumber,
            Address = data.Address,
            Email = data.Email,
            TelephoneNumber = data.TelephoneNumber,
            Has5PercentDiscount = false
            
        });

        await context.SaveChangesAsync();
    }

    public async Task PostClientCompany(PostClientCompanyRequestModel data)
    {
        context.ClientCompanies.Add(new ClientCompany
        {
            Name = data.Name,
            KRS = data.KRS,
            Address = data.Address,
            Email = data.Email,
            TelephoneNumber = data.TelephoneNumber,
            Has5PercentDiscount = false
        });
        
        await context.SaveChangesAsync();
    }

    public async Task DeleteClientPerson(int id)
    {
        var clientPerson = await context.ClientPersons.Where(e => e.ClientId == id).FirstOrDefaultAsync();

        if (clientPerson is null)
        {
            throw new NotFoundException($"There's no client with id={id}");

        }

        clientPerson.IsDeleted = true;
        
        await context.SaveChangesAsync();
    }

    public async Task PutClientPerson(PutClientPersonRequestModel data, int id)
    {
        
        var clientPerson = await context.ClientPersons.Where(e => e.ClientId == id).FirstOrDefaultAsync();
        if (clientPerson is null)
        {
            throw new NotFoundException($"There's no client with id={id}");

        }
        
        if (!data.Name.IsNullOrEmpty())
        {
            clientPerson.Name = data.Name;
        }
        
        if (!data.Surname.IsNullOrEmpty())
        {
            clientPerson.Surname = data.Surname;
        }
        
        if (!data.TelephoneNumber.IsNullOrEmpty())
        {
            clientPerson.TelephoneNumber = data.TelephoneNumber;
        }
        
        if (!data.Email.IsNullOrEmpty())
        {
            clientPerson.Email = data.Email;
        }
        
        if (!data.Address.IsNullOrEmpty())
        {
            clientPerson.Address = data.Address;
        }
        
        await context.SaveChangesAsync();
    }

    public async Task PutClientCompany(PutClientCompanyRequestModel data, int id)
    {
        var clientCompany = await context.ClientCompanies.Where(e => e.ClientId == id).FirstOrDefaultAsync();
        if (clientCompany is null)
        {
            throw new NotFoundException($"There's no client with id={id}");

        }
        
        if (!data.Name.IsNullOrEmpty())
        {
            clientCompany.Name = data.Name;
        }
        
        
        
        if (!data.TelephoneNumber.IsNullOrEmpty())
        {
            clientCompany.TelephoneNumber = data.TelephoneNumber;
        }
        
        if (!data.Email.IsNullOrEmpty())
        {
            clientCompany.Email = data.Email;
        }
        
        if (!data.Address.IsNullOrEmpty())
        {
            clientCompany.Address = data.Address;
        }
        
        await context.SaveChangesAsync();
    }
}