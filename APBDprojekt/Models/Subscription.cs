using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APBDprojekt.Models;

public class Subscription
{
    [Key]
    [Column("SubscriptionPK")]
    public int SubscriptionId { get; set; }

    [Column("priceWithDiscounts", TypeName = "decimal(10, 2)")]
    public Decimal PriceWithDiscounts { get; set; }
    
    [Column("subscriptionStartTime")]
    public DateTime SubscriptionStartTime { get; set; }
    
    [Column("subscriptionEndTime")]
    public DateTime SubscriptionEndTime { get; set; }
    
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
    

}