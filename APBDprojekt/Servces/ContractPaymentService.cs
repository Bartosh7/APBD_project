using APBDprojekt.Contexts;
using APBDprojekt.Exceptions;
using APBDprojekt.Models;
using APBDprojekt.RequestModels;
using APBDprojekt.ResponseModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace APBDprojekt.Servces;

public interface IContractPaymentService
{
    Task<PostContractPaymentResponseModel> PostContractPayment(PostContractPaymentRequestModel data, int idClient,
        int idContract);

    Task<GetProfitResponseModel> GetProfitFromAllProducts(string currency);

    Task<GetProfitResponseModel> GetProfitFromProduct(string currency, int softwareId);
}

public class ContractPaymentService(DatabaseContext context, HttpClient httpClient) : IContractPaymentService
{
    public async Task<PostContractPaymentResponseModel> PostContractPayment(PostContractPaymentRequestModel data,
        int idClient, int idContract)
    {
        var client = await context.Clients.Where(e => e.ClientId == idClient).FirstOrDefaultAsync();
        if (client is null)
        {
            throw new NotFoundException($"There's no client with id={idClient}");
        }

        var contract = await context.Contracts.Where(e => e.ContracId == idContract).FirstOrDefaultAsync();
        if (contract is null)
        {
            throw new NotFoundException($"There's no contract with id={idContract}");
        }

        if (contract.ClientId != idClient)
        {
            throw new NotFoundException($"There's no contract with id={idContract} for client with id={idClient}");
        }

        if (contract.Signed)
        {
            throw new AlreadySignedException("This contract is already fully paid and signed");
        }

        if (contract.AlreadyPaid + data.AmountOfPayment > contract.PriceWithDiscounts)
        {
            throw new TooBigPaymentException("Payment was higher than required");
        }

        if (contract.AlreadyPaid + data.AmountOfPayment == contract.PriceWithDiscounts)
        {
            contract.Signed = true;
        }

        var paymentToAdd = new ContractPayment
        {
            Amount = data.AmountOfPayment,
            ClientId = idClient,
            ContractId = idContract,
            Date = DateTime.Now
        };

        context.ContractPayments.Add(paymentToAdd);
        contract.AlreadyPaid += paymentToAdd.Amount;

        await context.SaveChangesAsync();

        var response = new PostContractPaymentResponseModel
        {
            ClientId = paymentToAdd.ClientId,
            SoftwareId = contract.SoftwareId,
            ContractPaymentId = paymentToAdd.ContractPaymentId,
            AlreadyPaid = contract.AlreadyPaid,
            Price = contract.PriceWithDiscounts,
            LastPayment = paymentToAdd.Amount
        };

        return response;
    }


    public async Task<GetProfitResponseModel> GetProfitFromAllProducts(string currency)
    {
        
        var actualProfit = context.Contracts.Where(e => e.Signed).Sum(e => e.AlreadyPaid);
        var possibleProfit = context.Contracts.Where(e => !e.Signed && e.PaymentEndTime >= DateTime.Now)
            .Sum(e => e.AlreadyPaid);


        Decimal exchangeRate = await GetExchangeRateAsync("PLN", currency);

        if (exchangeRate == 0)
        {
            throw new NotFoundException($"There's no currency: {currency}");
        }


        var response = new GetProfitResponseModel
        {
            ActualProfit = actualProfit * exchangeRate,
            ExpectedProfit = (actualProfit + possibleProfit) * exchangeRate
        };
        return response;
    }

    public async Task<GetProfitResponseModel> GetProfitFromProduct(string currency, int softwareId)
    {
        
        var software = await context.Softwares.Where(e => e.SoftwareId == softwareId).FirstOrDefaultAsync();
        if (software is null)
        {
            throw new NotFoundException($"There's no software with id={softwareId}");
        }
        
        
        var actualProfit = context.Contracts.Where(e => e.Signed && e.SoftwareId == softwareId).Sum(e => e.AlreadyPaid);
        var possibleProfit = context.Contracts
            .Where(e => !e.Signed && e.PaymentEndTime >= DateTime.Now && e.SoftwareId == softwareId)
            .Sum(e => e.AlreadyPaid);


        Decimal exchangeRate = await GetExchangeRateAsync("PLN", currency);

        if (exchangeRate == 0)
        {
            throw new NotFoundException($"There's no currency: {currency}");
        }


        var response = new GetProfitResponseModel
        {
            ActualProfit = actualProfit * exchangeRate,
            ExpectedProfit = (actualProfit + possibleProfit) * exchangeRate
        };
        return response;
    }


    public async Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency)
    {
        string apiKey = "30f6ecb32f2198a327d7bb03";
        var response =
            await httpClient.GetStringAsync($"https://v6.exchangerate-api.com/v6/{apiKey}/latest/{fromCurrency}");
        var rates = JObject.Parse(response)["conversion_rates"];
        return rates!.Value<decimal>(toCurrency);
    }
}