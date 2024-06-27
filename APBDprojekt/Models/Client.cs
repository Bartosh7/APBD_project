using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APBDprojekt.Models;

[Table("Client")]
public class Client
{
    [Key]
    [Column("ClientPK")]
    public int ClientId { get; set; }
        
    [Column("address")]
    [MaxLength(50)]
    public string Address { get; set; }
        
    [Column("email")]
    [MaxLength(50)]
    public string Email { get; set; }
        
    [Column("telephoneNumber")]
    [MaxLength(9)]
    public string TelephoneNumber { get; set; }

    [Column("has5PercentDiscount")]
    public bool Has5PercentDiscount { get; set; }
    
    public IEnumerable<Contract> Contracts { get; set; }
    
    public IEnumerable<ContractPayment> ContractPayments { get; set; }

    public IEnumerable<Subscription> Subscriptions { get; set; }


    
}