using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APBDprojekt.Models;
[Table("ContractPayment")]
public class ContractPayment
{
    [Column("ContractPaymentPK")]
    [Key]
    public int ContractPaymentId { get; set; }

    [Column("amount", TypeName = "decimal(10, 2)")]
    public decimal Amount { get; set; }

    [Column("DateOfPayment")]
    public DateTime Date { get; set; }

    [ForeignKey("Client")]
    [Column("ClientFK")]
    public int ClientId { get; set; }
    public Client Client { get; set; }
    
    [ForeignKey("Contract")]
    [Column("ContractFK")]
    public int ContractId { get; set; }
    public Contract Contract { get; set; }
    
    
    
    
}