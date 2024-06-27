using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APBDprojekt.Models;

[Table("ClientCompany")]
public class ClientCompany : Client
{
    [Column("name")]
    [MaxLength(50)]
    public string Name { get; set; }
        
    [Column("KRS")]
    [MaxLength(11)]
    public string KRS { get; set; }
}
