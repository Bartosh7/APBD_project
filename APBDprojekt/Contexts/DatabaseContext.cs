using APBDprojekt.Models;
using Microsoft.EntityFrameworkCore;

namespace APBDprojekt.Contexts;

public class DatabaseContext : DbContext
{
    public DbSet<Client> Clients { get; set; }
    public DbSet<ClientPerson> ClientPersons { get; set; }
    public DbSet<ClientCompany> ClientCompanies { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<ContractPayment> ContractPayments { get; set; }
    public DbSet<Discount> Discounts { get; set; }
   
    public DbSet<Software> Softwares { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    
    
    protected DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>()
            .ToTable("Client");
            
        modelBuilder.Entity<ClientPerson>()
            .ToTable("ClientPerson");

        modelBuilder.Entity<ClientCompany>()
            .ToTable("ClientCompany");
        
        
        
        
        modelBuilder.Entity<ContractPayment>()
            .HasOne(p => p.Client)
            .WithMany(c => c.ContractPayments)
            .HasForeignKey(p => p.ClientId)
            .OnDelete(DeleteBehavior.NoAction);


        modelBuilder.Entity<Software>().HasData(new Software
        {
            SoftwareId = 1,
            Name = "Rider",
            CanBuy = true,
            CanSubscribe = false,
            Category = "contract",
            CurrentVersion = "1.0",
            Description = "C#",
            PurchasePrice = 1000,
            

        });
        
        modelBuilder.Entity<Discount>().HasData(new Discount
        {
            DiscountId = 1,
            Name = "Student",
            DateFrom = DateTime.Parse("2023-01-01"),
            DateTo = DateTime.Parse("2024-12-31"),
            PercentValue = 50.0m,
            Type = "contract",
            SoftwareId = 1
        });

        modelBuilder.Entity<Discount>().HasData(new Discount
        {
            DiscountId = 2,
            Name = "Sale",
            DateFrom = DateTime.Parse("2023-01-01"),
            DateTo = DateTime.Parse("2024-12-31"),
            PercentValue = 30.0m,
            Type = "contract",
            SoftwareId = 1
            
        });

        modelBuilder.Entity<Discount>().HasData(new Discount
        {
            DiscountId = 3,
            Name = "Summer",
            DateFrom = DateTime.Parse("2022-01-01"),
            DateTo = DateTime.Parse("2022-12-31"),
            PercentValue = 80.0m,
            Type = "contract",
            SoftwareId = 1
        });
        

        base.OnModelCreating(modelBuilder);
    }
}