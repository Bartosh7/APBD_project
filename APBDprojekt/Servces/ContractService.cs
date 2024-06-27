using APBDprojekt.Contexts;
using APBDprojekt.Exceptions;
using APBDprojekt.Models;
using APBDprojekt.RequestModels;
using APBDprojekt.ResponseModels;
using Microsoft.EntityFrameworkCore;

namespace APBDprojekt.Servces;

public interface IContractService
{
    Task<PostContractModelResponseModel> PostContract(PostContractModelRequestModel data, int idClient, int idSoftware);
}

public class ContractService(DatabaseContext context) : IContractService
{
    public async Task<PostContractModelResponseModel> PostContract(PostContractModelRequestModel data, int idClient, int idSoftware)
    {
        var client = await context.Clients.Where(e => e.ClientId == idClient).FirstOrDefaultAsync();
        if (client is null)
        {
            throw new NotFoundException($"There's no client with id={idClient}");
        }
        
        var software = await context.Softwares.Where(e => e.SoftwareId == idSoftware).FirstOrDefaultAsync();
        if (software is null)
        {
            throw new NotFoundException($"There's no software with id={idSoftware}");
        }

        if (!software.CanBuy)
        {
            throw new BadSoftwareBuyTypeException($"That software with id={idSoftware} can not be buy");
        }
        
        var discount = await CalculateDiscount(idSoftware, "contract");

        if (client.Has5PercentDiscount)
        {
            discount += 5;
        }

        decimal discountedPrice = (decimal)((software.PurchasePrice + 1000 * (data.YearsOfVersionSupport - 1)) * ((100 - discount) / 100))!;


        var newContract = new Contract
        {
            ClientId = idClient,
            SoftwareId = idSoftware,
            PriceWithDiscounts = discountedPrice,
            AlreadyPaid = 0,
            PaymentStartTime = data.PaymentStartTime,
            PaymentEndTime = data.PaymentEndTime,
            Signed = false,
            CurrentSoftwareVersion = software.CurrentVersion,
            EndOfSupport = data.PaymentStartTime.AddYears(data.YearsOfVersionSupport)
        };

       context.Contracts.Add(newContract);

        await context.SaveChangesAsync();

        var response = new PostContractModelResponseModel
        {
            PriceWithDiscounts = newContract.PriceWithDiscounts,
            PaymentStartTime = newContract.PaymentStartTime,
            PaymentEndTime = newContract.PaymentEndTime,
            EndOfSupport = newContract.EndOfSupport,
            CurrentSoftwareVersion = newContract.CurrentSoftwareVersion,
            ClientId = newContract.ClientId,
            SoftwareId = newContract.SoftwareId
        };

        return response;
    }

    private async Task<decimal> CalculateDiscount(int idSoftware, string type)
    {
        var discountsList = await context.Discounts
            .Where(e => e.SoftwareId == idSoftware && e.DateFrom <= DateTime.Now && e.DateTo >= DateTime.Now && e.Type==type)
            .ToListAsync();

        decimal bestDisccouunt = 0;

        if (discountsList.Count > 0)
        {
            bestDisccouunt = discountsList.Max(e => e.PercentValue);
        }
        
        return bestDisccouunt;
    }
}