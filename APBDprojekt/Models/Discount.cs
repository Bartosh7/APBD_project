using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APBDprojekt.Models;

[Table("Discount")]
public class Discount
{
    [Key]
    [Column("DiscountPK")]
    public int DiscountId { get; set; }

    [Column("name")]
    [MaxLength(50)]
    public string Name { get; set; }

    [Column("percentValue", TypeName = "decimal(5, 2)")]
    public decimal PercentValue { get; set; }
    
    [Column("dateFrom")]
    public DateTime DateFrom { get; set; }
    
    [Column("dateTo")]
    public DateTime DateTo { get; set; }
    
    [Column("type")]
    [MaxLength(50)]
    public string Type { get; set; }

    [ForeignKey("Software")]
    [Column("SoftwareFK")]
    public int SoftwareId { get; set; }

    public Software Software { get; set; }

    

}