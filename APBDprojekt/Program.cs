using System.Net.Sockets;
using System.Text;
using APBDprojekt.Contexts;
using APBDprojekt.Exceptions;
using APBDprojekt.RequestModels;
using APBDprojekt.Servces;
using APBDprojekt.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IContractPaymentService, ContractPaymentService>();
builder.Services.AddScoped<IAuthService, AuthService>();
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

// Dodaj usługi do kontenera DI
builder.Services.AddControllers();



builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "My API", 
        Version = "v1" 
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        In = ParameterLocation.Header, 
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey 
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        { 
            new OpenApiSecurityScheme 
            { 
                Reference = new OpenApiReference 
                { 
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer" 
                } 
            },
            new string[] { } 
        } 
    });
});


// Konfiguracja JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserPolicy", policy => policy.RequireRole("User", "Admin"));
});

builder.Services.AddHttpClient();

var app = builder.Build();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/login", async (RegisterAndLoginRequestModel model, IAuthService authService,  IValidator<RegisterAndLoginRequestModel> validator) =>
{
    var validate = await validator.ValidateAsync(model);
    if (!validate.IsValid)
    {
        return Results.ValidationProblem(validate.ToDictionary());
    }

    var result = await authService.LoginAsync(model);
    if (result == null)
    {
        return Results.Unauthorized();
    }
    return Results.Ok(result);
});

app.MapPost("/api/register/user", async (RegisterAndLoginRequestModel model, IAuthService authService, IValidator<RegisterAndLoginRequestModel> validator) =>
{
    var validate = await validator.ValidateAsync(model);
    if (!validate.IsValid)
    {
        return Results.ValidationProblem(validate.ToDictionary());
    }

    var result = await authService.RegisterUserAsync(model, "User");
    if (!result)
    {
        return Results.BadRequest("User already exists.");
    }
    return Results.Ok("User registered successfully.");
});



app.MapPost("/api/register/admin", async (RegisterAndLoginRequestModel model, IAuthService authService, IValidator<RegisterAndLoginRequestModel> validator) =>
{
    var validate = await validator.ValidateAsync(model);
    if (!validate.IsValid)
    {
        return Results.ValidationProblem(validate.ToDictionary());
    }

    var result = await authService.RegisterUserAsync(model, "Admin");
    if (!result)
    {
        return Results.BadRequest("User already exists.");
    }
    return Results.Ok("User registered successfully.");
});

app.MapPost("/api/refresh-token", async (string refreshToken, IAuthService authService) =>
{
    var result = await authService.RefreshTokenAsync(refreshToken);
    if (result == null)
    {
        return Results.Unauthorized();
    }
    return Results.Ok(result);
});

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
    }).RequireAuthorization("UserPolicy"); // Dodanie autoryzacji

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
    }).RequireAuthorization("UserPolicy"); // Dodanie autoryzacji

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
}).RequireAuthorization("AdminPolicy"); // Dodanie autoryzacji

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
}).RequireAuthorization("AdminPolicy"); // Dodanie autoryzacji

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
}).RequireAuthorization("AdminPolicy"); // Dodanie autoryzacji

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
}).RequireAuthorization("UserPolicy"); // Dodanie autoryzacji

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
        var result = await service.PostContractPayment(data, idClient, idContract);

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
}).RequireAuthorization("UserPolicy"); // Dodanie autoryzacji

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
}).RequireAuthorization("UserPolicy"); // Dodanie autoryzacji

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
}).RequireAuthorization("UserPolicy"); // Dodanie autoryzacji

app.Run();
