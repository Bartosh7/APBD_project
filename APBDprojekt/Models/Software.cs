using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APBDprojekt.Models;
[Table("Software")]
public class Software
{
    [Key]
    [Column("SoftwarePk")]
    public int SoftwareId { get; set; }
    
    [Column("name")]
    [MaxLength(50)]
    public string Name { get; set; }
    public string Description { get; set; }
    
    [Column("currentVersion")]
    [MaxLength(50)]
    public string CurrentVersion { get; set; }
    
    [Column("category")]
    [MaxLength(50)]
    public string Category { get; set; }
    
    
    [Column("purchasePrice", TypeName = "decimal(10, 2)")]
    public Decimal? PurchasePrice { get; set; }
    
    [Column("subscriptionPrice", TypeName = "decimal(10, 2)")]
    public Decimal? SubscriptionPrice { get; set; }
    
    [Column("canBuy")]
    public bool CanBuy { get; set; }
    
    [Column("canSubscribe")]
    public bool CanSubscribe { get; set; }

    public IEnumerable<Contract> Contracts { get; set; }
    
    public IEnumerable<Subscription> Subscriptions { get; set; }


   

}