using System.Net.Sockets;
using APBDprojekt.Contexts;
using APBDprojekt.Exceptions;
using APBDprojekt.RequestModels;
using APBDprojekt.Servces;
using APBDprojekt.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IContractPaymentService, ContractPaymentService>();
builder.Services.AddValidatorsFromAssemblyContaining<PostClientCompanyValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PostClientPersonValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PutClientPersonValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PutClientCompanyValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PostContractValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PostContractPaymentValidator>();

builder.Services.AddDbContext<DatabaseContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddHttpClient(); //todo check

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("api/clientPerson",
    async (PostClientPersonRequestModel data, IClientService service,
        IValidator<PostClientPersonRequestModel> validator) =>
    {
        var validate = await validator.ValidateAsync(data);
        if (!validate.IsValid)
        {
            return Results.ValidationProblem(validate.ToDictionary());
        }

        await service.PostClientPerson(data);

        return Results.Created();
    });

app.MapPost("api/clientCompany",
    async (PostClientCompanyRequestModel data, IClientService service,
        IValidator<PostClientCompanyRequestModel> validator) =>
    {
        var validate = await validator.ValidateAsync(data);
        if (!validate.IsValid)
        {
            return Results.ValidationProblem(validate.ToDictionary());
        }

        await service.PostClientCompany(data);

        return Results.Created();
    });

app.MapDelete("api/clientPerson/{id:int}", async (IClientService service, int id) =>
    {
        try
        {
            await service.DeleteClientPerson(id);
            return Results.NoContent();
        }
        catch (NotFoundException e)
        {
            return Results.NotFound(e.Message);
        }
    }
);
app.MapPut("api/clientPerson/{id:int}", async (PutClientPersonRequestModel data, IClientService service,
    IValidator<PutClientPersonRequestModel> validator, int id) =>
{
    var validate = await validator.ValidateAsync(data);
    if (!validate.IsValid)
    {
        return Results.ValidationProblem(validate.ToDictionary());
    }

    try
    {
        await service.PutClientPerson(data, id);
        return Results.NoContent();
    }
    catch (NotFoundException e)
    {
        return Results.NotFound(e.Message);
    }
});

app.MapPut("api/clientCompany/{id:int}", async (PutClientCompanyRequestModel data, IClientService service,
    IValidator<PutClientCompanyRequestModel> validator, int id) =>
{
    var validate = await validator.ValidateAsync(data);
    if (!validate.IsValid)
    {
        return Results.ValidationProblem(validate.ToDictionary());
    }

    try
    {
        await service.PutClientCompany(data, id);
        return Results.NoContent();
    }
    catch (NotFoundException e)
    {
        return Results.NotFound(e.Message);
    }
});

app.MapPost("api/contracts/client/{idClient:int}/software/{idSoftware:int}", async (PostContractModelRequestModel data,
    IContractService service, IValidator<PostContractModelRequestModel> validator, int idClient, int idSoftware) =>
{
    var validate = await validator.ValidateAsync(data);
    if (!validate.IsValid)
    {
        return Results.ValidationProblem(validate.ToDictionary());
    }

    try
    {
        var result = await service.PostContract(data, idClient, idSoftware);

        return Results.Created($"/{result}", result);
    }
    catch (NotFoundException e)
    {
        return Results.NotFound(e.Message);
    }
    catch (BadSoftwareBuyTypeException e)
    {
        return Results.BadRequest(e.Message);
    }
});

app.MapPost("api/contractPayment/client/{idClient:int}/contract/{idContract:int}", async (
    PostContractPaymentRequestModel data,
    IContractPaymentService service, IValidator<PostContractPaymentRequestModel> validator, int idClient,
    int idContract) =>
{
    var validate = await validator.ValidateAsync(data);
    if (!validate.IsValid)
    {
        return Results.ValidationProblem(validate.ToDictionary());
    }

    try
    {
        var result = await service.PutContractPayment(data, idClient, idContract);

        return Results.Created($"/{result}", result);
    }
    catch (NotFoundException e)
    {
        return Results.NotFound(e.Message);
    }
    catch (AlreadySignedException e)
    {
        return Results.BadRequest(e.Message);
    }
    catch (TooBigPaymentException e)
    {
        return Results.BadRequest(e.Message);
    }
});

app.MapGet("api/profit/{currency}", async (string currency,
    IContractPaymentService service) =>
{
    try
    {
        var result = await service.GetProfitFromAllProducts(currency);
        return Results.Ok(result);
    }
    catch (NotFoundException e)
    {
        return Results.NotFound(e.Message);;
    }
    
    
    
});

app.MapGet("api/profit/{currency}/software/{softwareId:int}", async (string currency, int softwareId,
    IContractPaymentService service) =>
{
    try
    {
        var result = await service.GetProfitFromProduct(currency, softwareId);
        return Results.Ok(result);
    }
    catch (NotFoundException e)
    {
        return Results.NotFound(e.Message);;
    }
    
    
    
});





app.Run();