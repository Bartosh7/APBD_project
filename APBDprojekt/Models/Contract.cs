using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APBDprojekt.Models;

[Table(("Contract"))]
public class Contract
{
    [Key]
    [Column("ContractPK")]
    public int ContracId { get; set; }
    
    [Column("priceWithDiscounts", TypeName = "decimal(10, 2)")]
    public Decimal PriceWithDiscounts { get; set; }

    [Column("alreadyPaid", TypeName = "decimal(10, 2)")]
    public Decimal AlreadyPaid { get; set; }

    [Column("paymentStartTime")]
    public DateTime PaymentStartTime { get; set; }
    
    [Column("paymentEndTime")]
    public DateTime PaymentEndTime { get; set; }

    [Column("endOfSupport")]
    public DateTime EndOfSupport { get; set; }

    [Column("signed")]
    public bool Signed { get; set; }

    [Column("currentSoftwareVersion")]
    public string CurrentSoftwareVersion { get; set; }

    [Column("ClientFK")]
    [ForeignKey("Client")]
    public int ClientId { get; set; }
    public Client Client { get; set; }
    
    [Column("SoftwareFK")]
    [ForeignKey("Software")]
    public int SoftwareId { get; set; }
    public Software Software { get; set; }
    
    public IEnumerable<ContractPayment> ContractPayments { get; set; }
    
    



}